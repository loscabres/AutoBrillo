using Microsoft.AspNetCore.Components;

namespace AutoBrillo.Web.Pages;

/// <summary>
/// Código de la página de acceso, separado de su vista Login.razor.
/// La vista contiene el HTML; esta clase contiene el comportamiento del formulario.
/// </summary>
public partial class Login
{
    // Objeto que reúne los dos datos escritos en el formulario.
    private readonly Credenciales credenciales = new();

    // Referencia al input Usuario para colocar el cursor allí al abrir la pantalla.
    private ElementReference campoUsuario;

    // Mensaje visible si la API rechaza el acceso.
    private string? error;

    // Evita que el usuario presione Ingresar varias veces mientras llega la respuesta.
    private bool enviando;

    /// <summary>
    /// Se ejecuta después de que Blazor dibuja la página por primera vez.
    /// Coloca el foco en Usuario para que se pueda escribir sin tocar el campo.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool primerRender)
    {
        if (primerRender)
            await campoUsuario.FocusAsync();
    }

    /// <summary>
    /// Se ejecuta cuando el formulario es válido y se presiona Ingresar.
    /// Pide al servicio de autenticación iniciar sesión y redirige a Inicio si no hubo error.
    /// </summary>
    private async Task Ingresar()
    {
        enviando = true;
        error = await Autenticacion.LoginAsync(credenciales.Nombre, credenciales.Password);
        enviando = false;

        // Null significa que LoginAsync recibió un token correcto de la API.
        if (error is null)
            Navegacion.NavigateTo("inicio", forceLoad: true);
    }

    /// <summary>Modelo sencillo que representa los datos escritos en el formulario.</summary>
    private class Credenciales
    {
        /// <summary>Valor enlazado al campo Usuario.</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Valor enlazado al campo Contraseña; solo se envía a la API al ingresar.</summary>
        public string Password { get; set; } = string.Empty;
    }
}
