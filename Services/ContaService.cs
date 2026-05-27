using SistemaBancarioSprint3.DTOs;
using SistemaBancarioSprint3.Models;
using SistemaBancarioSprint3.Repositories;

namespace SistemaBancarioSprint3.Services;

public class ContaService
{
    private readonly IContaRepository _contas;
    private readonly ITransacaoRepository _transacoes;

    public ContaService(IContaRepository contas, ITransacaoRepository transacoes)
    {
        _contas = contas;
        _transacoes = transacoes;
    }

    public async Task<ContaResponseDto> CriarAsync(int usuarioId, CriarContaDto dto)
    {
        if (!Enum.IsDefined(typeof(TipoConta), dto.Tipo))
            throw new InvalidOperationException("Tipo de conta invalido.");

        if (dto.SaldoInicial < 0)
            throw new InvalidOperationException("O saldo inicial nao pode ser negativo.");

        if (await _contas.ExisteTipoParaUsuarioAsync(usuarioId, dto.Tipo))
            throw new InvalidOperationException("Voce ja possui uma conta deste tipo.");

        var conta = new Conta { Tipo = dto.Tipo, Saldo = dto.SaldoInicial, UsuarioId = usuarioId };
        await _contas.AdicionarAsync(conta);
        return new ContaResponseDto(conta.Id, conta.Tipo, conta.Saldo, conta.UsuarioId);
    }

    public async Task<List<ContaResponseDto>> ListarAsync(int usuarioId)
    {
        var contas = await _contas.ListarPorUsuarioAsync(usuarioId);
        return contas.Select(c => new ContaResponseDto(c.Id, c.Tipo, c.Saldo, c.UsuarioId)).ToList();

    }

    public async Task<List<ContaResponseDto>> ListarTodasAsync()
{
    var contas = await _contas.ListarTodasAsync();

    return contas.Select(c =>
        new ContaResponseDto(c.Id, c.Tipo, c.Saldo, c.UsuarioId)
    ).ToList();
}

    public async Task<ContaResponseDto> ObterAsync(int contaId, int usuarioId)
    {
        var conta = await ValidarContaDoUsuario(contaId, usuarioId);
        return new ContaResponseDto(conta.Id, conta.Tipo, conta.Saldo, conta.UsuarioId);
    }

    public async Task<TransacaoResponseDto> DepositarAsync(int contaId, int usuarioId, decimal valor)
    {
        if (valor <= 0) throw new InvalidOperationException("O valor deve ser maior que zero.");
        var conta = await ValidarContaDoUsuario(contaId, usuarioId);
        conta.Saldo += valor;
        var transacao = new Transacao { ContaId = conta.Id, Tipo = TipoTransacao.Deposito, Valor = valor, Taxa = 0 };
        await _transacoes.AdicionarAsync(transacao);
        await _contas.AtualizarAsync(conta);
        return new TransacaoResponseDto(transacao.Id, transacao.Tipo, transacao.Valor, transacao.Taxa, transacao.Data, conta.Saldo);
    }

    public async Task<TransacaoResponseDto> SacarAsync(int contaId, int usuarioId, decimal valor)
    {
        if (valor <= 0) throw new InvalidOperationException("O valor deve ser maior que zero.");
        var conta = await ValidarContaDoUsuario(contaId, usuarioId);
        var taxa = CalcularTaxa(conta.Tipo, valor);
        var total = valor + taxa;
        if (conta.Saldo < total) throw new InvalidOperationException($"Saldo insuficiente. Total com taxa: {total:C}");
        conta.Saldo -= total;
        var transacao = new Transacao { ContaId = conta.Id, Tipo = TipoTransacao.Saque, Valor = valor, Taxa = taxa };
        await _transacoes.AdicionarAsync(transacao);
        await _contas.AtualizarAsync(conta);
        return new TransacaoResponseDto(transacao.Id, transacao.Tipo, transacao.Valor, transacao.Taxa, transacao.Data, conta.Saldo);
    }

    public async Task<List<TransacaoResponseDto>> ExtratoAsync(int contaId, int usuarioId)
    {
        var conta = await ValidarContaDoUsuario(contaId, usuarioId);
        var lista = await _transacoes.ListarPorContaAsync(conta.Id);
        return lista.Select(t => new TransacaoResponseDto(t.Id, t.Tipo, t.Valor, t.Taxa, t.Data, conta.Saldo)).ToList();
    }

    public async Task RemoverAsync(int contaId, int usuarioId)
    {
        var conta = await ValidarContaDoUsuario(contaId, usuarioId);
        await _contas.RemoverAsync(conta);
    }
    public async Task AdminRemoverAsync(int contaId)
{
    var conta = await _contas.ObterPorIdAsync(contaId);

    if (conta == null)
        throw new KeyNotFoundException("Conta nao encontrada.");

    await _contas.RemoverAsync(conta);
}



    public async Task TransferirAsync(int origemId, int usuarioId, int destinoId, decimal valor)
    {
        if (valor <= 0) throw new InvalidOperationException("O valor deve ser maior que zero.");
        if (origemId == destinoId) throw new InvalidOperationException("A conta de destino deve ser diferente da conta de origem.");

        var origem = await ValidarContaDoUsuario(origemId, usuarioId);
        var destino = await _contas.ObterPorIdAsync(destinoId) ?? throw new KeyNotFoundException("Conta destino nao encontrada.");

        if (origem.Saldo < valor) throw new InvalidOperationException("Saldo insuficiente.");

        origem.Saldo -= valor;
        destino.Saldo += valor;

        await _contas.AtualizarAsync(origem);
        await _contas.AtualizarAsync(destino);

        await _transacoes.AdicionarAsync(new Transacao
        {
            ContaId = origem.Id,
            Tipo = TipoTransacao.Saque,
            Valor = valor,
            Taxa = 0
        });

        await _transacoes.AdicionarAsync(new Transacao
        {
            ContaId = destino.Id,
            Tipo = TipoTransacao.Deposito,
            Valor = valor,
            Taxa = 0
        });
    }

    public async Task<string> AplicarRendimentoAsync(int contaId, int usuarioId)
    {
        var conta = await ValidarContaDoUsuario(contaId, usuarioId);
        conta.Saldo += conta.Saldo * 0.01m;
        await _contas.AtualizarAsync(conta);
        return "Rendimento aplicado com sucesso.";
    }

    private async Task<Conta> ValidarContaDoUsuario(int contaId, int usuarioId)
    {
        var conta = await _contas.ObterPorIdAsync(contaId) ?? throw new KeyNotFoundException("Conta nao encontrada.");
        if (conta.UsuarioId != usuarioId) throw new UnauthorizedAccessException("Voce nao tem acesso a esta conta.");
        return conta;
    }

    private static decimal CalcularTaxa(TipoConta tipo, decimal valor) => tipo switch
    {
        TipoConta.Corrente => 2.50m,
        TipoConta.Empresarial => valor * 0.01m,
        _ => 0m
    };
}
