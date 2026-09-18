window.autobrilloAlerta = {
    confirmarEliminar: async (titulo, nombre) => {
        const resultado = await Swal.fire({
            title: titulo,
            text: `Se eliminará el usuario ${nombre}.`,
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Sí, eliminar",
            cancelButtonText: "Cancelar",
            reverseButtons: true,
            focusCancel: true,
            customClass: {
                popup: "autobrillo-swal",
                confirmButton: "autobrillo-swal-eliminar",
                cancelButton: "autobrillo-swal-cancelar"
            }
        });
        return resultado.isConfirmed;
    },
    exito: (mensaje) => Swal.fire({
        title: "Eliminado",
        text: mensaje,
        icon: "success",
        confirmButtonText: "Entendido",
        customClass: { popup: "autobrillo-swal", confirmButton: "autobrillo-swal-principal" }
    }),
    error: (mensaje) => Swal.fire({
        title: "No se pudo eliminar",
        text: mensaje,
        icon: "error",
        confirmButtonText: "Entendido",
        customClass: { popup: "autobrillo-swal", confirmButton: "autobrillo-swal-principal" }
    })
};
