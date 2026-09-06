using AutoBrillo.Model.Entidades;

namespace AutoBrillo.Repository.Repositorios;

public interface IUserRepository
{
    Task<Usuario?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken = default);
    Task<bool> ExisteNombreAsync(string nombre, CancellationToken cancellationToken = default);
    Task AgregarAsync(Usuario usuario, CancellationToken cancellationToken = default);
    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
