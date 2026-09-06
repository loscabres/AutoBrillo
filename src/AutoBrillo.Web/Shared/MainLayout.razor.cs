namespace AutoBrillo.Web.Shared;

/// <summary>
/// Código del menú superior, separado de su vista MainLayout.razor.
/// Se encarga de mostrar el usuario conectado y de cerrar la sesión.
/// </summary>
public partial class MainLayout
{
    // Nombre que se leerá de localStorage para saludar al usuario en la barra superior.
    private string? usuario;

    /// <summary>Obtiene el nombre de la sesión guardada cuando se crea el diseño de la página.</summary>
    protected override async Task OnInitializedAsync()
    {
        usuario = await Autenticacion.ObtenerUsuarioAsync();
    }

    /// <summary>
    /// Borra token y nombre del navegador mediante ServicioAutenticacion,
    /// luego devuelve a la pantalla Login.
    /// </summary>
    private async Task Salir()
    {
        await Autenticacion.CerrarSesionAsync();
        Navegacion.NavigateTo("login", forceLoad: true);
    }
}
