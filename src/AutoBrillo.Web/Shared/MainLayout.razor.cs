namespace AutoBrillo.Web.Shared;

/// <summary>
/// Código del menú superior, separado de su vista MainLayout.razor.
/// </summary>
public partial class MainLayout
{
    private string? usuario;

    protected override async Task OnInitializedAsync()
    {
        usuario = await Autenticacion.ObtenerUsuarioAsync();
    }

    private async Task Salir()
    {
        await Autenticacion.CerrarSesionAsync();
        Navegacion.NavigateTo("login", forceLoad: true);
    }
}
