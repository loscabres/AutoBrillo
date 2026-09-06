using AutoBrillo.Model.Entidades;
using AutoBrillo.Repository.Datos;
using Microsoft.EntityFrameworkCore;

namespace AutoBrillo.Repository.Repositorios;

/// <summary>
/// Implementación concreta del contrato IUserRepository.
/// Centraliza las consultas de usuarios para que la API no escriba SQL ni conozca Entity Framework Core.
/// </summary>
public class RepositorioUsuarios(AutoBrilloDbContext contexto) : IUserRepository
{
    /// <summary>
    /// Consulta un único usuario cuyo Nombre coincida exactamente con el valor indicado.
    /// SingleOrDefaultAsync devuelve null cuando la consulta no encuentra ninguno.
    /// </summary>
    public Task<Usuario?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default) =>
        contexto.Usuarios.SingleOrDefaultAsync(x => x.Nombre == nombre, cancellationToken);

    /// <summary>
    /// Pregunta a la base si existe al menos un usuario con ese nombre.
    /// Se usa antes de registrar para evitar nombres duplicados.
    /// </summary>
    public Task<bool> ExisteNombreAsync(string nombre, CancellationToken cancellationToken = default) =>
        contexto.Usuarios.AnyAsync(x => x.Nombre == nombre, cancellationToken);

    /// <summary>
    /// Agrega el objeto al contexto de EF Core, pero todavía no ejecuta el INSERT.
    /// El INSERT ocurre después en GuardarCambiosAsync.
    /// </summary>
    public Task AgregarAsync(Usuario usuario, CancellationToken cancellationToken = default) =>
        contexto.Usuarios.AddAsync(usuario, cancellationToken).AsTask();

    /// <summary>Envía a PostgreSQL los cambios pendientes, por ejemplo el INSERT de un usuario nuevo.</summary>
    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        contexto.SaveChangesAsync(cancellationToken);
}
