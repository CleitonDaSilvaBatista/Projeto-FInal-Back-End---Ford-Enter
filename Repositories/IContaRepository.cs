using SistemaBancarioSprint3.Models;

namespace SistemaBancarioSprint3.Repositories;

public interface IContaRepository
{
    Task<List<Conta>> ListarPorUsuarioAsync(int usuarioId);
    Task<Conta?> ObterPorIdAsync(int id);
    Task AdicionarAsync(Conta conta);
    Task AtualizarAsync(Conta conta);
    Task RemoverAsync(Conta conta);
    Task<List<Conta>> ListarTodasAsync();
    Task<bool> ExisteTipoParaUsuarioAsync(int usuarioId, TipoConta tipo);
}
