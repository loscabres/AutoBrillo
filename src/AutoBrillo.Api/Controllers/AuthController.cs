using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoBrillo.Api.Contratos;
using AutoBrillo.Model.Entidades;
using AutoBrillo.Repository.Repositorios;
using AutoBrillo.Repository.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AutoBrillo.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IUserRepository usuarios, PasswordHasher passwordHasher, IConfiguration configuracion) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Registro(CredencialesRequest request, CancellationToken cancellationToken)
    {
        var nombre = request.Nombre.Trim();
        if (await usuarios.ExisteNombreAsync(nombre, cancellationToken))
            return Conflict(new { mensaje = "El nombre de usuario ya está registrado." });

        var usuario = new Usuario { Nombre = nombre, Password = passwordHasher.Hashear(request.Password) };
        await usuarios.AgregarAsync(usuario, cancellationToken);
        await usuarios.GuardarCambiosAsync(cancellationToken);
        return CreatedAtAction(nameof(Me), new { }, new { usuario.Id_Usuarios, usuario.Nombre });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(CredencialesRequest request, CancellationToken cancellationToken)
    {
        var usuario = await usuarios.ObtenerPorNombreAsync(request.Nombre.Trim(), cancellationToken);
        if (usuario is null || !passwordHasher.Verificar(request.Password, usuario.Password))
            return Unauthorized(new { mensaje = "Nombre o contraseña incorrectos." });

        return Ok(new { token = CrearToken(usuario), usuario = usuario.Nombre });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new
    {
        id = User.FindFirstValue(ClaimTypes.NameIdentifier),
        usuario = User.Identity?.Name
    });

    private string CrearToken(Usuario usuario)
    {
        var clave = configuracion["JWT_SECRET"] ?? throw new InvalidOperationException("Falta la variable de entorno JWT_SECRET.");
        var issuer = configuracion["JWT_ISSUER"] ?? "AutoBrillo.Api";
        var audience = configuracion["JWT_AUDIENCE"] ?? "AutoBrillo.Web";
        var credenciales = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)), SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id_Usuarios.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nombre)
        };
        var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.UtcNow.AddHours(8), signingCredentials: credenciales);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
