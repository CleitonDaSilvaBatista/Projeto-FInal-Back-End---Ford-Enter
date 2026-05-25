using Microsoft.EntityFrameworkCore;
using SistemaBancarioSprint3.Models;

namespace SistemaBancarioSprint3.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Conta> Contas => Set<Conta>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Conta>().Property(c => c.Saldo).HasPrecision(18, 2);
        modelBuilder.Entity<Transacao>().Property(t => t.Valor).HasPrecision(18, 2);
        modelBuilder.Entity<Transacao>().Property(t => t.Taxa).HasPrecision(18, 2);
    }
}
