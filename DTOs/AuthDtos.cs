using System.ComponentModel.DataAnnotations;

namespace SistemaBancarioSprint3.DTOs;

public record RegistroDto(
    [Required] string Nome,
    [Required, EmailAddress] string Email,
    [Required, MinLength(6)] string Senha
);

public record LoginDto(
    [Required, EmailAddress] string Email,
    [Required] string Senha
);

public record TokenDto(string Token, string Nome, string Email, string Perfil);
