namespace AutoBrillo.Web.Pages;

/// <summary>
/// Código de la página Inicio, separado de su vista Inicio.razor.
/// </summary>
public partial class Inicio
{
    private bool cargando = true;

    protected override async Task OnInitializedAsync()
    {
        if (!await Autenticacion.ValidarSesionAsync())
        {
            Navegacion.NavigateTo("login", forceLoad: true);
            return;
        }

        cargando = false;
    }
}
