using AutoBrillo.Web.Servicios;

namespace AutoBrillo.Web.Pages;

/// <summary>Página del ABM de Usuarios. Las contraseñas viajan a la API y allí se almacenan como BCrypt.</summary>
public partial class Usuarios
{
    private bool cargando = true;
    private List<UsuarioDto> usuarios = [];
    private List<RolDto> roles = [];
    private UsuarioEdicionDto edicion = new();
    private UsuarioDto? usuarioAEliminar;
    private string? mensaje;

    protected override async Task OnInitializedAsync()
    {
        if (!await Autenticacion.ValidarSesionAsync()) { Navegacion.NavigateTo("login", true); return; }
        await CargarAsync();
        cargando = false;
    }

    private async Task CargarAsync()
    {
        usuarios = await Gestion.ObtenerUsuariosAsync() ?? [];
        roles = await Gestion.ObtenerRolesAsync() ?? [];
    }
    private async Task Guardar()
    {
        if (string.IsNullOrWhiteSpace(edicion.Nombre) || edicion.Id_Roles.Count == 0 || (edicion.Id_Usuarios == 0 && string.IsNullOrWhiteSpace(edicion.Password)))
        { mensaje = "Completá nombre, contraseña y al menos un rol."; return; }
        mensaje = await Gestion.GuardarUsuarioAsync(edicion);
        if (mensaje is null) { Cancelar(); await CargarAsync(); }
    }
    private void Editar(UsuarioDto usuario)
    {
        edicion = new UsuarioEdicionDto { Id_Usuarios = usuario.Id_Usuarios, Nombre = usuario.Nombre, Id_Roles = [.. usuario.Id_Roles] };
        mensaje = null;
    }
    private bool EstaSeleccionado(int idRol) => edicion.Id_Roles.Contains(idRol);
    private void CambiarRol(int idRol)
    {
        if (EstaSeleccionado(idRol)) edicion.Id_Roles.Remove(idRol);
        else edicion.Id_Roles.Add(idRol);
    }
    private void Cancelar() { edicion = new(); mensaje = null; }
    private void PedirEliminar(UsuarioDto usuario) { usuarioAEliminar = usuario; mensaje = null; }
    private void CancelarEliminar() => usuarioAEliminar = null;
    private async Task EliminarConfirmado()
    {
        if (usuarioAEliminar is null) return;
        mensaje = await Gestion.EliminarUsuarioAsync(usuarioAEliminar.Id_Usuarios);
        usuarioAEliminar = null;
        if (mensaje is null) await CargarAsync();
    }
}
