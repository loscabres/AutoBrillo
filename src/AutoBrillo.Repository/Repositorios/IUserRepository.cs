using AutoBrillo.Model.Entidades;

namespace AutoBrillo.Repository.Repositorios;

/// <summary>
/// Define las operaciones que la API puede pedir sobre usuarios.
/// Una interfaz funciona como un contrato: indica qué se puede hacer sin exponer cómo se consulta PostgreSQL.
/// </summary>
public interface IUserRepository
{
    /// <summary>Busca un usuario por su nombre. Devuelve null cuando no existe.</summary>
    Task<Usuario?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default);

    /// <summary>Indica si ya existe un usuario con el nombre recibido.</summary>
    Task<bool> ExisteNombreAsync(string nombre, CancellationToken cancellationToken = default);

    /// <summary>Prepara un usuario nuevo para ser guardado.</summary>
    Task AgregarAsync(Usuario usuario, CancellationToken cancellationToken = default);

    /// <summary>Confirma en la base de datos los cambios preparados.</summary>
    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
