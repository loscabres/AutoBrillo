using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace AutoBrillo.Web.Servicios;

/// <summary>Cliente de autenticación; la web se comunica solo con la API.</summary>
public class ServicioAutenticacion(HttpClient http, IJSRuntime js)
{
    private const string ClaveToken = "autobrillo.token";
    private const string ClaveUsuario = "autobrillo.usuario";

    public async Task<string?> LoginAsync(string nombre, string password)
    {
        var respuesta = await http.PostAsJsonAsync("api/auth/login", new { nombre, password });
        if (!respuesta.IsSuccessStatusCode)
            return "Nombre o contraseña incorrectos.";

        var datos = await respuesta.Content.ReadFromJsonAsync<LoginResponse>();
        if (datos is null) return "No fue posible leer la respuesta de la API.";
        await js.InvokeVoidAsync("localStorage.setItem", ClaveToken, datos.Token);
        await js.InvokeVoidAsync("localStorage.setItem", ClaveUsuario, datos.Usuario);
        return null;
    }

    public async Task<string?> ObtenerUsuarioAsync() =>
        await js.InvokeAsync<string?>("localStorage.getItem", ClaveUsuario);

    public async Task<bool> ValidarSesionAsync()
    {
        var token = await js.InvokeAsync<string?>("localStorage.getItem", ClaveToken);
        if (string.IsNullOrWhiteSpace(token)) return false;
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return (await http.GetAsync("api/auth/me")).IsSuccessStatusCode;
    }

    public async Task CerrarSesionAsync()
    {
        http.DefaultRequestHeaders.Authorization = null;
        await js.InvokeVoidAsync("localStorage.removeItem", ClaveToken);
        await js.InvokeVoidAsync("localStorage.removeItem", ClaveUsuario);
    }

    private sealed record LoginResponse(string Token, string Usuario);
}
