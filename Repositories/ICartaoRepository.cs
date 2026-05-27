using SistemaBancarioSprint3.Models;

namespace SistemaBancarioSprint3.Repositories;

public interface ICartaoRepository
{
    Task<List<Cartao>> ListarPorUsuarioAsync(int usuarioId);
    Task<Cartao?> ObterPorIdAsync(int id);
    Task<Cartao?> ObterPorNumeroAsync(string numero);
    Task AdicionarAsync(Cartao cartao);
    Task RemoverAsync(Cartao cartao);
}
