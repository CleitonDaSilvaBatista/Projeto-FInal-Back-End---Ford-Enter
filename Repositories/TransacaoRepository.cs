using Microsoft.EntityFrameworkCore;
using SistemaBancarioSprint3.Data;
using SistemaBancarioSprint3.Models;

namespace SistemaBancarioSprint3.Repositories;

public class TransacaoRepository : ITransacaoRepository
{
    private readonly AppDbContext _context;
    public TransacaoRepository(AppDbContext context) => _context = context;
    public async Task AdicionarAsync(Transacao transacao) { _context.Transacoes.Add(transacao); await _context.SaveChangesAsync(); }
    public Task<List<Transacao>> ListarPorContaAsync(int contaId) => _context.Transacoes.Where(t => t.ContaId == contaId).OrderByDescending(t => t.Data).ToListAsync();
}
