using System.ComponentModel.DataAnnotations;

namespace AutoBrillo.Api.Contratos;

/// <summary>Datos para crear o editar un usuario desde el ABM.</summary>
public class UsuarioRequest
{
    [Required, StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Contraseña nueva. Es obligatoria al crear y opcional al editar.</summary>
    [MinLength(6)]
    public string? Password { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un rol.")]
    public int Id_Roles { get; set; }
}
