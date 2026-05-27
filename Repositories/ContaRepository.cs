using Microsoft.EntityFrameworkCore;
using SistemaBancarioSprint3.Data;
using SistemaBancarioSprint3.Models;

namespace SistemaBancarioSprint3.Repositories;

public class ContaRepository : IContaRepository
{
    private readonly AppDbContext _context;
    public ContaRepository(AppDbContext context) => _context = context;
    public Task<List<Conta>> ListarPorUsuarioAsync(int usuarioId) => _context.Contas.Where(c => c.UsuarioId == usuarioId).ToListAsync();
    public Task<Conta?> ObterPorIdAsync(int id) => _context.Contas.Include(c => c.Transacoes).FirstOrDefaultAsync(c => c.Id == id);
    public async Task AdicionarAsync(Conta conta) { _context.Contas.Add(conta); await _context.SaveChangesAsync(); }
    public async Task AtualizarAsync(Conta conta) { _context.Contas.Update(conta); await _context.SaveChangesAsync(); }
    public async Task RemoverAsync(Conta conta) { _context.Contas.Remove(conta); await _context.SaveChangesAsync(); }
    public async Task<List<Conta>> ListarTodasAsync()
{
    return await _context.Contas.ToListAsync();
}

    public Task<bool> ExisteTipoParaUsuarioAsync(int usuarioId, TipoConta tipo) =>
        _context.Contas.AnyAsync(c => c.UsuarioId == usuarioId && c.Tipo == tipo);
}
