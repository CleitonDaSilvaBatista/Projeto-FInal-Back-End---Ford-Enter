using Microsoft.AspNetCore.Mvc;
using SistemaBancarioSprint3.DTOs;
using SistemaBancarioSprint3.Services;

namespace SistemaBancarioSprint3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;
    public AuthController(AuthService service) => _service = service;

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar(RegistroDto dto)
    {
        try { return Ok(await _service.RegistrarAsync(dto)); }
        catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try { return Ok(await _service.LoginAsync(dto)); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(new { erro = ex.Message }); }
    }
}
