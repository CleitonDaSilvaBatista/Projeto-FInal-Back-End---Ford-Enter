using Microsoft.EntityFrameworkCore;
using SistemaBancarioSprint3.Data;
using SistemaBancarioSprint3.Models;

namespace SistemaBancarioSprint3.Repositories;

public class CartaoRepository : ICartaoRepository
{
    private readonly AppDbContext _context;
    public CartaoRepository(AppDbContext context) => _context = context;

    public Task<List<Cartao>> ListarPorUsuarioAsync(int usuarioId) =>
        _context.Cartoes.Where(c => c.UsuarioId == usuarioId).OrderByDescending(c => c.CriadoEm).ToListAsync();

    public Task<Cartao?> ObterPorIdAsync(int id) =>
        _context.Cartoes.FirstOrDefaultAsync(c => c.Id == id);

    public Task<Cartao?> ObterPorNumeroAsync(string numero) =>
        _context.Cartoes.FirstOrDefaultAsync(c => c.Numero == numero);

    public async Task AdicionarAsync(Cartao cartao)
    {
        _context.Cartoes.Add(cartao);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(Cartao cartao)
    {
        _context.Cartoes.Remove(cartao);
        await _context.SaveChangesAsync();
    }
}
