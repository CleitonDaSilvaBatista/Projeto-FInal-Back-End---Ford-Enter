using System.ComponentModel.DataAnnotations;

namespace SistemaBancarioSprint3.Models;

public class Conta
{
    public int Id { get; set; }
    [Required] public TipoConta Tipo { get; set; }
    public decimal Saldo { get; set; }
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public List<Transacao> Transacoes { get; set; } = new();
}
