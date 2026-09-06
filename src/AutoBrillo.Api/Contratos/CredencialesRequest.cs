using System.ComponentModel.DataAnnotations;

namespace AutoBrillo.Api.Contratos;

public class CredencialesRequest
{
    [Required, StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
