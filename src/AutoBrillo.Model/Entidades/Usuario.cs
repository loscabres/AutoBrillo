namespace AutoBrillo.Model.Entidades;

/// <summary>Representa a un usuario registrado en AutoBrillo.</summary>
public class Usuario
{
    public int Id_Usuarios { get; set; }

    // Equivale a varchar(100) en la base de datos.
    public string Nombre { get; set; } = string.Empty;

    // Solo se guarda el hash BCrypt, nunca la contraseña en texto plano.
    public string Password { get; set; } = string.Empty;
}
