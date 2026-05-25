using System.ComponentModel.DataAnnotations;
using SistemaBancarioSprint3.Models;

namespace SistemaBancarioSprint3.DTOs;

public record CriarContaDto([Required] TipoConta Tipo, [Range(0, double.MaxValue)] decimal SaldoInicial);
public record ContaResponseDto(int Id, TipoConta Tipo, decimal Saldo, int UsuarioId);
public record TransacaoDto([Range(0.01, double.MaxValue)] decimal Valor);
public record TransacaoResponseDto(int Id, TipoTransacao Tipo, decimal Valor, decimal Taxa, DateTime Data, decimal SaldoAtual);
