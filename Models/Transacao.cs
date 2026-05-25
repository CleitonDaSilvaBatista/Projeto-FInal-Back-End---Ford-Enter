using System.ComponentModel.DataAnnotations;

namespace SistemaBancarioSprint3.Models;

public class Transacao
{
    public int Id { get; set; }
    [Required] public TipoTransacao Tipo { get; set; }
    [Range(0.01, double.MaxValue)] public decimal Valor { get; set; }
    public decimal Taxa { get; set; }
    public DateTime Data { get; set; } = DateTime.UtcNow;
    public int ContaId { get; set; }
    public Conta? Conta { get; set; }
}
