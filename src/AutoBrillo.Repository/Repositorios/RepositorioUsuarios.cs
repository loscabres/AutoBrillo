using AutoBrillo.Model.Entidades;
using AutoBrillo.Repository.Datos;
using Microsoft.EntityFrameworkCore;

namespace AutoBrillo.Repository.Repositorios;

/// <summary>Acceso a usuarios mediante EF Core.</summary>
public class RepositorioUsuarios(AutoBrilloDbContext contexto) : IUserRepository
{
    public Task<Usuario?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default) =>
        contexto.Usuarios.SingleOrDefaultAsync(x => x.Nombre == nombre, cancellationToken);

    public Task<bool> ExisteNombreAsync(string nombre, CancellationToken cancellationToken = default) =>
        contexto.Usuarios.AnyAsync(x => x.Nombre == nombre, cancellationToken);

    public Task AgregarAsync(Usuario usuario, CancellationToken cancellationToken = default) =>
        contexto.Usuarios.AddAsync(usuario, cancellationToken).AsTask();

    public Task GuardarCambiosAsync(CancellationToken cancellationToken = default) =>
        contexto.SaveChangesAsync(cancellationToken);
}
