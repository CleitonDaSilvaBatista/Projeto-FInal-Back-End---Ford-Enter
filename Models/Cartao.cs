using System.ComponentModel.DataAnnotations;

namespace SistemaBancarioSprint3.Models;

public class Cartao
{
    public int Id { get; set; }

    [Required, MaxLength(120)]
    public string NomeTitular { get; set; } = string.Empty;

    [Required, MaxLength(19)]
    public string Numero { get; set; } = string.Empty;

    [Required, MaxLength(5)]
    public string Validade { get; set; } = string.Empty;

    [Required, MaxLength(3)]
    public string Cvv { get; set; } = string.Empty;

    public decimal Limite { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
}
