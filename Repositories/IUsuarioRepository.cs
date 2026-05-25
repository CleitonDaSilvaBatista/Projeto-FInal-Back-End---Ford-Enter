using SistemaBancarioSprint3.Models;

namespace SistemaBancarioSprint3.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<Usuario?> ObterPorIdAsync(int id);
    Task AdicionarAsync(Usuario usuario);
}
