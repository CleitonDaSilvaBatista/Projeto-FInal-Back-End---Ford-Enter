using SistemaBancarioSprint3.Models;

namespace SistemaBancarioSprint3.Repositories;

public interface ITransacaoRepository
{
    Task AdicionarAsync(Transacao transacao);
    Task<List<Transacao>> ListarPorContaAsync(int contaId);
}
