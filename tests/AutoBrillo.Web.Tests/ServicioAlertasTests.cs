using AutoBrillo.Web.Servicios;
using Microsoft.JSInterop;
using Xunit;

namespace AutoBrillo.Web.Tests;

public class ServicioAlertasTests
{
    [Fact]
    public async Task ConfirmarEliminarUsuario_muestra_dialogo_central_con_el_nombre_del_usuario()
    {
        var javascript = new JavaScriptFalso(true);
        var servicio = new ServicioAlertas(javascript);

        var confirmado = await servicio.ConfirmarEliminarUsuarioAsync("María");

        Assert.True(confirmado);
        Assert.Equal("autobrilloAlerta.confirmarEliminar", javascript.Identificador);
        Assert.Equal("María", javascript.Argumentos[1]);
    }

    [Fact]
    public async Task MostrarEliminacionExitosa_notifica_en_un_dialogo_central()
    {
        var javascript = new JavaScriptFalso(null);
        var servicio = new ServicioAlertas(javascript);

        await servicio.MostrarEliminacionExitosaAsync("María");

        Assert.Equal("autobrilloAlerta.exito", javascript.Identificador);
        Assert.Contains("María", javascript.Argumentos[0]?.ToString());
    }

    [Fact]
    public async Task MostrarErrorEliminar_notifica_el_error_en_un_dialogo_central()
    {
        var javascript = new JavaScriptFalso(null);
        var servicio = new ServicioAlertas(javascript);

        await servicio.MostrarErrorEliminarAsync("El usuario no existe.");

        Assert.Equal("autobrilloAlerta.error", javascript.Identificador);
        Assert.Equal("El usuario no existe.", javascript.Argumentos[0]);
    }

    private sealed class JavaScriptFalso(bool? respuesta) : IJSRuntime
    {
        public string? Identificador { get; private set; }
        public object?[] Argumentos { get; private set; } = [];

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            Identificador = identifier;
            Argumentos = args ?? [];
            return ValueTask.FromResult((TValue)(object?)respuesta!);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            => InvokeAsync<TValue>(identifier, args);
    }
}
