using System.Security.Cryptography;
using SistemaBancarioSprint3.DTOs;
using SistemaBancarioSprint3.Models;
using SistemaBancarioSprint3.Repositories;

namespace SistemaBancarioSprint3.Services;

public class CartaoService
{
    private readonly ICartaoRepository _cartoes;
    private readonly IUsuarioRepository _usuarios;

    public CartaoService(ICartaoRepository cartoes, IUsuarioRepository usuarios)
    {
        _cartoes = cartoes;
        _usuarios = usuarios;
    }

    public async Task<List<CartaoResponseDto>> ListarAsync(int usuarioId)
    {
        var cartoes = await _cartoes.ListarPorUsuarioAsync(usuarioId);
        return cartoes.Select(ToResponse).ToList();
    }

    public async Task<CartaoResponseDto> SolicitarAsync(int usuarioId)
    {
        var usuario = await _usuarios.ObterPorIdAsync(usuarioId)
            ?? throw new KeyNotFoundException("Usuario nao encontrado.");

        string numero;
        do
        {
            numero = GerarNumeroCartao();
        } while (await _cartoes.ObterPorNumeroAsync(numero) is not null);

        var cartao = new Cartao
        {
            UsuarioId = usuarioId,
            NomeTitular = usuario.Nome.ToUpperInvariant(),
            Numero = numero,
            Validade = GerarValidade(),
            Cvv = RandomNumberGenerator.GetInt32(100, 1000).ToString(),
            Limite = GerarLimite()
        };

        await _cartoes.AdicionarAsync(cartao);
        return ToResponse(cartao);
    }

    public async Task RemoverAsync(int cartaoId, int usuarioId)
    {
        var cartao = await _cartoes.ObterPorIdAsync(cartaoId)
            ?? throw new KeyNotFoundException("Cartao nao encontrado.");

        if (cartao.UsuarioId != usuarioId)
            throw new UnauthorizedAccessException("Voce nao tem acesso a este cartao.");

        await _cartoes.RemoverAsync(cartao);
    }

    private static CartaoResponseDto ToResponse(Cartao c) =>
        new(c.Id, c.NomeTitular, c.Numero, c.Validade, c.Cvv, c.Limite, c.CriadoEm);

    private static string GerarNumeroCartao()
    {
        var grupos = new[]
        {
            "4" + RandomNumberGenerator.GetInt32(100, 1000),
            RandomNumberGenerator.GetInt32(1000, 10000).ToString(),
            RandomNumberGenerator.GetInt32(1000, 10000).ToString(),
            RandomNumberGenerator.GetInt32(1000, 10000).ToString()
        };

        return string.Join(" ", grupos);
    }

    private static string GerarValidade()
    {
        var data = DateTime.UtcNow.AddYears(5);
        return $"{data.Month:00}/{data:yy}";
    }

    private static decimal GerarLimite()
    {
        var opcoes = new[] { 500m, 1000m, 1500m, 2000m, 3000m, 5000m };
        return opcoes[RandomNumberGenerator.GetInt32(opcoes.Length)];
    }
}
