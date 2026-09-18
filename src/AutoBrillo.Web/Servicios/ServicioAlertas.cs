using Microsoft.JSInterop;

namespace AutoBrillo.Web.Servicios;

/// <summary>Centraliza confirmaciones y resultados visuales de eliminación.</summary>
public class ServicioAlertas(IJSRuntime javascript)
{
    public ValueTask<bool> ConfirmarEliminarUsuarioAsync(string nombre)
        => javascript.InvokeAsync<bool>(
            "autobrilloAlerta.confirmarEliminar",
            "¿Eliminar usuario?",
            nombre);

    public ValueTask MostrarEliminacionExitosaAsync(string nombre)
        => javascript.InvokeVoidAsync(
            "autobrilloAlerta.exito",
            $"El usuario {nombre} fue eliminado correctamente.");

    public ValueTask MostrarErrorEliminarAsync(string detalle)
        => javascript.InvokeVoidAsync("autobrilloAlerta.error", detalle);
}
