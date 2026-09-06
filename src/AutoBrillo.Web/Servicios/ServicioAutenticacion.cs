using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace AutoBrillo.Web.Servicios;

/// <summary>
/// Centraliza la comunicación de la web con los endpoints de autenticación de la API.
/// Así las páginas Login, Inicio y MainLayout no repiten solicitudes HTTP ni código de localStorage.
/// </summary>
public class ServicioAutenticacion(HttpClient http, IJSRuntime js)
{
    // Nombres con los que el navegador guarda los datos de sesión en localStorage.
    private const string ClaveToken = "autobrillo.token";
    private const string ClaveUsuario = "autobrillo.usuario";

    /// <summary>
    /// Envía Nombre y Password a POST /api/auth/login.
    /// Si la API acepta las credenciales, guarda el token y el nombre para usar la sesión después.
    /// </summary>
    /// <returns>Null cuando el acceso fue correcto; un mensaje cuando falló.</returns>
    public async Task<string?> LoginAsync(string nombre, string password)
    {
        var respuesta = await http.PostAsJsonAsync("api/auth/login", new { nombre, password });
        if (!respuesta.IsSuccessStatusCode)
            return "Nombre o contraseña incorrectos.";

        var datos = await respuesta.Content.ReadFromJsonAsync<LoginResponse>();
        if (datos is null) return "No fue posible leer la respuesta de la API.";

        // localStorage conserva la sesión aunque se actualice la página del navegador.
        await js.InvokeVoidAsync("localStorage.setItem", ClaveToken, datos.Token);
        await js.InvokeVoidAsync("localStorage.setItem", ClaveUsuario, datos.Usuario);
        return null;
    }

    /// <summary>Obtiene el nombre almacenado para mostrarlo en la barra superior.</summary>
    public async Task<string?> ObtenerUsuarioAsync() =>
        await js.InvokeAsync<string?>("localStorage.getItem", ClaveUsuario);

    /// <summary>
    /// Comprueba que existe un token y pregunta a GET /api/auth/me si sigue siendo válido.
    /// Devuelve false cuando no hay sesión, el token venció o la API lo rechaza.
    /// </summary>
    public async Task<bool> ValidarSesionAsync()
    {
        var token = await js.InvokeAsync<string?>("localStorage.getItem", ClaveToken);
        if (string.IsNullOrWhiteSpace(token)) return false;

        // Agrega el token al encabezado que la API usa para identificar al usuario.
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return (await http.GetAsync("api/auth/me")).IsSuccessStatusCode;
    }

    /// <summary>
    /// Elimina los datos guardados en el navegador y deja de enviar el token en solicitudes futuras.
    /// </summary>
    public async Task CerrarSesionAsync()
    {
        http.DefaultRequestHeaders.Authorization = null;
        await js.InvokeVoidAsync("localStorage.removeItem", ClaveToken);
        await js.InvokeVoidAsync("localStorage.removeItem", ClaveUsuario);
    }

    /// <summary>Formato de la respuesta JSON que devuelve el endpoint de login.</summary>
    private sealed record LoginResponse(string Token, string Usuario);
}
