using Microsoft.EntityFrameworkCore;
using SistemaBancarioSprint3.Data;
using SistemaBancarioSprint3.Models;

namespace SistemaBancarioSprint3.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;
    public UsuarioRepository(AppDbContext context) => _context = context;
    public Task<Usuario?> ObterPorEmailAsync(string email) => _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    public Task<Usuario?> ObterPorIdAsync(int id) => _context.Usuarios.FindAsync(id).AsTask();
    public async Task AdicionarAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
    }
}
