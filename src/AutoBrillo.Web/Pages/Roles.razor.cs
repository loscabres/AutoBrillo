using AutoBrillo.Web.Servicios;

namespace AutoBrillo.Web.Pages;

/// <summary>Página del ABM de Roles. La base se consulta exclusivamente mediante AutoBrillo.Api.</summary>
public partial class Roles
{
    private bool cargando = true;
    private List<RolDto> roles = [];
    private RolDto edicion = new();
    private RolDto? rolAEliminar;
    private string? mensaje;

    protected override async Task OnInitializedAsync()
    {
        if (!await Autenticacion.ValidarSesionAsync()) { Navegacion.NavigateTo("login", true); return; }
        await CargarAsync();
        cargando = false;
    }

    private async Task CargarAsync() => roles = await Gestion.ObtenerRolesAsync() ?? [];
    private async Task Guardar()
    {
        if (string.IsNullOrWhiteSpace(edicion.Descripcion)) { mensaje = "Escribí la descripción del rol."; return; }
        mensaje = await Gestion.GuardarRolAsync(edicion);
        if (mensaje is null) { Cancelar(); await CargarAsync(); }
    }
    private void Editar(RolDto rol) { edicion = new RolDto { Id_Roles = rol.Id_Roles, Descripcion = rol.Descripcion }; mensaje = null; }
    private void Cancelar() { edicion = new(); mensaje = null; }
    private void PedirEliminar(RolDto rol) { rolAEliminar = rol; mensaje = null; }
    private void CancelarEliminar() => rolAEliminar = null;
    private async Task EliminarConfirmado()
    {
        if (rolAEliminar is null) return;
        mensaje = await Gestion.EliminarRolAsync(rolAEliminar.Id_Roles);
        rolAEliminar = null;
        if (mensaje is null) await CargarAsync();
    }
}
