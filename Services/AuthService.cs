using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SistemaBancarioSprint3.DTOs;
using SistemaBancarioSprint3.Models;
using SistemaBancarioSprint3.Repositories;

namespace SistemaBancarioSprint3.Services;

public class AuthService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<Usuario> _hasher = new();

    public AuthService(IUsuarioRepository usuarios, IConfiguration config)
    {
        _usuarios = usuarios;
        _config = config;
    }

    public async Task<TokenDto> RegistrarAsync(RegistroDto dto)
    {
        if (await _usuarios.ObterPorEmailAsync(dto.Email) is not null)
            throw new InvalidOperationException("E-mail ja cadastrado.");

        var perfil = dto.Email.Contains("admin", StringComparison.OrdinalIgnoreCase) ? "Admin" : "Cliente";
        var usuario = new Usuario { Nome = dto.Nome, Email = dto.Email, Perfil = perfil };
        usuario.SenhaHash = _hasher.HashPassword(usuario, dto.Senha);
        await _usuarios.AdicionarAsync(usuario);
        return GerarToken(usuario);
    }

    public async Task<TokenDto> LoginAsync(LoginDto dto)
    {
        var usuario = await _usuarios.ObterPorEmailAsync(dto.Email) ?? throw new UnauthorizedAccessException("Credenciais invalidas.");
        var resultado = _hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, dto.Senha);
        if (resultado == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Credenciais invalidas.");
        return GerarToken(usuario);
    }

    private TokenDto GerarToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nome),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Perfil)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: creds);

        return new TokenDto(new JwtSecurityTokenHandler().WriteToken(token), usuario.Nome, usuario.Email, usuario.Perfil);
    }
}
