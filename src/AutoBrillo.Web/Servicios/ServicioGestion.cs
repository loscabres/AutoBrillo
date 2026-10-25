using System.Net.Http.Json;

namespace AutoBrillo.Web.Servicios;

/// <summary>Comunicación HTTP/JSON de los ABM de Roles y Usuarios con AutoBrillo.Api.</summary>
public class ServicioGestion(HttpClient http)
{
    public Task<List<RolDto>?> ObtenerRolesAsync() => http.GetFromJsonAsync<List<RolDto>>("api/roles");
    public Task<List<UsuarioDto>?> ObtenerUsuariosAsync() => http.GetFromJsonAsync<List<UsuarioDto>>("api/usuarios");
    public Task<List<PersonaDto>?> ObtenerPersonasAsync() => http.GetFromJsonAsync<List<PersonaDto>>("api/personas");
    public Task<List<TipoPersonaDto>?> ObtenerTiposPersonasAsync() => http.GetFromJsonAsync<List<TipoPersonaDto>>("api/tipos-personas");

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

    public async Task<string?> CambiarEstadoUsuarioAsync(int id, bool activo) =>
        await LeerErrorAsync(await http.PatchAsJsonAsync($"api/usuarios/{id}/estado", new { activo }));

    public async Task<string?> GuardarPersonaAsync(PersonaEdicionDto persona)
    {
        var datos = new { nombre = persona.Nombre, documento = persona.Documento, telefono = persona.Telefono, id_Tipos_Personas = persona.Id_Tipos_Personas };
        var respuesta = persona.Id_Personas == 0
            ? await http.PostAsJsonAsync("api/personas", datos)
            : await http.PutAsJsonAsync($"api/personas/{persona.Id_Personas}", datos);
        return await LeerErrorAsync(respuesta);
    }

    public async Task<string?> CambiarEstadoPersonaAsync(int id, bool activo) =>
        await LeerErrorAsync(await http.PatchAsJsonAsync($"api/personas/{id}/estado", new { activo }));

    public async Task<string?> EliminarPersonaAsync(int id) => await LeerErrorAsync(await http.DeleteAsync($"api/personas/{id}"));

    public async Task<string?> GuardarTipoPersonaAsync(TipoPersonaDto tipo)
    {
        var respuesta = tipo.Id_Tipos_Personas == 0
            ? await http.PostAsJsonAsync("api/tipos-personas", new { descripcion = tipo.Descripcion })
            : await http.PutAsJsonAsync($"api/tipos-personas/{tipo.Id_Tipos_Personas}", new { descripcion = tipo.Descripcion });
        return await LeerErrorAsync(respuesta);
    }

    public async Task<string?> EliminarTipoPersonaAsync(int id) => await LeerErrorAsync(await http.DeleteAsync($"api/tipos-personas/{id}"));

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

public record UsuarioDto(int Id_Usuarios, string Nombre, List<int> Id_Roles, List<string> Roles, bool Activo = true);
public class PersonaDto
{
    public int Id_Personas { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public bool Activo { get; set; }
    public List<int> Id_Tipos_Personas { get; set; } = [];
    public List<string> Tipos { get; set; } = [];
}

public class PersonaEdicionDto
{
    public int Id_Personas { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public List<int> Id_Tipos_Personas { get; set; } = [];
}

public class TipoPersonaDto
{
    public int Id_Tipos_Personas { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}

public class UsuarioEdicionDto
{
    public int Id_Usuarios { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<int> Id_Roles { get; set; } = [];
}
