using AutoBrillo.Api.Contratos;
using AutoBrillo.Model.Entidades;
using AutoBrillo.Repository.Repositorios;
using AutoBrillo.Repository.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoBrillo.Api.Controllers;

/// <summary>ABM de usuarios. Las contraseñas se transforman con BCrypt antes de guardarse.</summary>
[ApiController]
[Authorize]
[Route("api/usuarios")]
public class UsuariosController(IUserRepository usuarios, IRolRepository roles, PasswordHasher passwordHasher) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<UsuarioResponse>>> ObtenerTodos(CancellationToken cancellationToken)
    {
        var lista = await usuarios.ObtenerTodosAsync(cancellationToken);
        return Ok(lista.Select(Convertir).ToList());
    }

    [HttpPost]
    public async Task<IActionResult> Crear(UsuarioRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { mensaje = "La contraseña es obligatoria al crear un usuario." });

        var nombre = request.Nombre.Trim();
        if (await usuarios.ExisteNombreAsync(nombre, cancellationToken))
            return Conflict(new { mensaje = "El nombre de usuario ya está registrado." });
        if (await roles.ObtenerPorIdAsync(request.Id_Roles, cancellationToken) is null)
            return BadRequest(new { mensaje = "El rol seleccionado no existe." });

        var usuario = new Usuario
        {
            Nombre = nombre,
            Password = passwordHasher.Hashear(request.Password),
            Id_Roles = request.Id_Roles
        };
        await usuarios.AgregarAsync(usuario, cancellationToken);
        await usuarios.GuardarCambiosAsync(cancellationToken);
        usuario = await usuarios.ObtenerPorIdAsync(usuario.Id_Usuarios, cancellationToken) ?? usuario;
        return CreatedAtAction(nameof(ObtenerTodos), new { usuario.Id_Usuarios }, Convertir(usuario));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, UsuarioRequest request, CancellationToken cancellationToken)
    {
        var usuario = await usuarios.ObtenerPorIdAsync(id, cancellationToken);
        if (usuario is null) return NotFound(new { mensaje = "Usuario no encontrado." });

        var nombre = request.Nombre.Trim();
        if (await usuarios.ExisteNombreAsync(nombre, cancellationToken) && usuario.Nombre != nombre)
            return Conflict(new { mensaje = "El nombre de usuario ya está registrado." });
        if (await roles.ObtenerPorIdAsync(request.Id_Roles, cancellationToken) is null)
            return BadRequest(new { mensaje = "El rol seleccionado no existe." });

        usuario.Nombre = nombre;
        usuario.Id_Roles = request.Id_Roles;
        if (!string.IsNullOrWhiteSpace(request.Password))
            usuario.Password = passwordHasher.Hashear(request.Password);

        await usuarios.GuardarCambiosAsync(cancellationToken);
        usuario = await usuarios.ObtenerPorIdAsync(id, cancellationToken) ?? usuario;
        return Ok(Convertir(usuario));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken)
    {
        var usuario = await usuarios.ObtenerPorIdAsync(id, cancellationToken);
        if (usuario is null) return NotFound(new { mensaje = "Usuario no encontrado." });

        usuarios.Eliminar(usuario);
        await usuarios.GuardarCambiosAsync(cancellationToken);
        return NoContent();
    }

    private static UsuarioResponse Convertir(Usuario usuario) =>
        new(usuario.Id_Usuarios, usuario.Nombre, usuario.Id_Roles, usuario.Rol?.Descripcion ?? string.Empty);
}
