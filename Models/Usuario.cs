using System.ComponentModel.DataAnnotations;

namespace SistemaBancarioSprint3.Models;

public class Usuario
{
    public int Id { get; set; }
    [Required, MaxLength(120)] public string Nome { get; set; } = string.Empty;
    [Required, EmailAddress, MaxLength(160)] public string Email { get; set; } = string.Empty;
    [Required] public string SenhaHash { get; set; } = string.Empty;
    [Required, MaxLength(30)] public string Perfil { get; set; } = "Cliente";
    public List<Conta> Contas { get; set; } = new();
    public List<Cartao> Cartoes { get; set; } = new();
}
