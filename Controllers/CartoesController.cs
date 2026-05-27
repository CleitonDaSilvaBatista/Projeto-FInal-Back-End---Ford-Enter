using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaBancarioSprint3.Services;

namespace SistemaBancarioSprint3.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CartoesController : ControllerBase
{
    private readonly CartaoService _service;
    public CartoesController(CartaoService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar() => Ok(await _service.ListarAsync(UsuarioId()));

    [HttpPost("solicitar")]
    public async Task<IActionResult> Solicitar()
    {
        try
        {
            var cartao = await _service.SolicitarAsync(UsuarioId());
            return CreatedAtAction(nameof(Listar), new { id = cartao.Id }, cartao);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remover(int id)
    {
        try
        {
            await _service.RemoverAsync(id, UsuarioId());
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { erro = ex.Message }); }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
    }

    private int UsuarioId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
