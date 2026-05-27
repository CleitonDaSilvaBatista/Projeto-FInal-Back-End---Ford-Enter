namespace SistemaBancarioSprint3.DTOs;

public record CartaoResponseDto(
    int Id,
    string NomeTitular,
    string Numero,
    string Validade,
    string Cvv,
    decimal Limite,
    DateTime CriadoEm
);
