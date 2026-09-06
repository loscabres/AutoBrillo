using Microsoft.AspNetCore.Components;

namespace AutoBrillo.Web.Pages;

/// <summary>
/// Código de la página de acceso, separado de su vista Login.razor.
/// </summary>
public partial class Login
{
    private readonly Credenciales credenciales = new();
    private ElementReference campoUsuario;
    private string? error;
    private bool enviando;

    protected override async Task OnAfterRenderAsync(bool primerRender)
    {
        if (primerRender)
            await campoUsuario.FocusAsync();
    }

    private async Task Ingresar()
    {
        enviando = true;
        error = await Autenticacion.LoginAsync(credenciales.Nombre, credenciales.Password);
        enviando = false;

        if (error is null)
            Navegacion.NavigateTo("inicio", forceLoad: true);
    }

    /// <summary>Datos escritos por el usuario en el formulario de acceso.</summary>
    private class Credenciales
    {
        public string Nombre { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
