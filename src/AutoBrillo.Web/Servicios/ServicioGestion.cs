using System.Net.Http.Json;

namespace AutoBrillo.Web.Servicios;

/// <summary>Comunicación HTTP/JSON de los ABM de Roles y Usuarios con AutoBrillo.Api.</summary>
public class ServicioGestion(HttpClient http)
{
    public Task<List<RolDto>?> ObtenerRolesAsync() => http.GetFromJsonAsync<List<RolDto>>("api/roles");
    public Task<List<UsuarioDto>?> ObtenerUsuariosAsync() => http.GetFromJsonAsync<List<UsuarioDto>>("api/usuarios");

    public async Task<string?> GuardarRolAsync(RolDto rol)
    {
        var respuesta = rol.Id_Roles == 0
            ? await http.PostAsJsonAsync("api/roles", new { descripcion = rol.Descripcion })
            : await http.PutAsJsonAsync($"api/roles/{rol.Id_Roles}", new { descripcion = rol.Descripcion });
        return await LeerErrorAsync(respuesta);
    }

    public async Task<string?> EliminarRolAsync(int id) => await LeerErrorAsync(await http.DeleteAsync($"api/roles/{id}"));

    public async Task<string?> GuardarUsuarioAsync(UsuarioEdicionDto usuario)
    {
        var datos = new { nombre = usuario.Nombre, password = usuario.Password, id_Roles = usuario.Id_Roles };
        var respuesta = usuario.Id_Usuarios == 0
            ? await http.PostAsJsonAsync("api/usuarios", datos)
            : await http.PutAsJsonAsync($"api/usuarios/{usuario.Id_Usuarios}", datos);
        return await LeerErrorAsync(respuesta);
    }

    public async Task<string?> EliminarUsuarioAsync(int id) => await LeerErrorAsync(await http.DeleteAsync($"api/usuarios/{id}"));

    private static async Task<string?> LeerErrorAsync(HttpResponseMessage respuesta)
    {
        if (respuesta.IsSuccessStatusCode) return null;
        try
        {
            var problema = await respuesta.Content.ReadFromJsonAsync<ProblemaApi>();
            if (!string.IsNullOrWhiteSpace(problema?.Mensaje)) return problema.Mensaje;
            if (problema?.Errors is not null)
                return problema.Errors.Values.SelectMany(x => x).FirstOrDefault() ?? "No fue posible completar la operación.";
        }
        catch (System.Text.Json.JsonException) { }
        return "No fue posible completar la operación. Revisá los datos e intentá de nuevo.";
    }

    private sealed class ProblemaApi
    {
        public string? Mensaje { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}

public class RolDto
{
    public int Id_Roles { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}

public record UsuarioDto(int Id_Usuarios, string Nombre, List<int> Id_Roles, List<string> Roles);
public class UsuarioEdicionDto
{
    public int Id_Usuarios { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<int> Id_Roles { get; set; } = [];
}
