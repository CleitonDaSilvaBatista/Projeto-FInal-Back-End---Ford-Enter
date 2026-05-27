using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaBancarioSprint3.DTOs;
using SistemaBancarioSprint3.Services;

namespace SistemaBancarioSprint3.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ContasController : ControllerBase
{
    private readonly ContaService _service;
    public ContasController(ContaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _service.ListarAsync(UsuarioId()));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id)
    {
        try { return Ok(await _service.ObterAsync(id, UsuarioId())); }
        catch (KeyNotFoundException ex) { return NotFound(new { erro = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
    }

    [HttpPost]
    public async Task<IActionResult> Criar(CriarContaDto dto)
    {
        try
        {
            var conta = await _service.CriarAsync(UsuarioId(), dto);
            return CreatedAtAction(nameof(Obter), new { id = conta.Id }, conta);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpPost("{id:int}/depositar")]
    public async Task<IActionResult> Depositar(int id, TransacaoDto dto)
    {
        try { return Ok(await _service.DepositarAsync(id, UsuarioId(), dto.Valor)); }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException) { return BadRequest(new { erro = ex.Message }); }
    }

    [HttpPost("{id:int}/sacar")]
    public async Task<IActionResult> Sacar(int id, TransacaoDto dto)
    {
        try { return Ok(await _service.SacarAsync(id, UsuarioId(), dto.Valor)); }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException) { return BadRequest(new { erro = ex.Message }); }
    }

    [HttpGet("{id:int}/extrato")]
    public async Task<IActionResult> Extrato(int id)
    {
        try { return Ok(await _service.ExtratoAsync(id, UsuarioId())); }
        catch (Exception ex) when (ex is UnauthorizedAccessException or KeyNotFoundException) { return BadRequest(new { erro = ex.Message }); }
    }



    [HttpPost("{id:int}/transferir")]
    public async Task<IActionResult> Transferir(int id, [FromBody] TransferenciaRequest request)
    {
        try
        {
            await _service.TransferirAsync(id, UsuarioId(), request.ContaDestinoId, request.Valor);
            return Ok(new { mensagem = "Transferência realizada com sucesso." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpPost("{id:int}/rendimento")]
    public async Task<IActionResult> Rendimento(int id)
    {
        try
        {
            var mensagem = await _service.AplicarRendimentoAsync(id, UsuarioId());
            return Ok(new { mensagem });
        }
        catch (Exception ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        try { await _service.RemoverAsync(id, UsuarioId()); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { erro = ex.Message }); }
    }

    private int UsuarioId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

