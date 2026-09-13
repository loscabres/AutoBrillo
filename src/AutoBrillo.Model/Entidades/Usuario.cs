namespace AutoBrillo.Model.Entidades;

/// <summary>
/// Representa un usuario del sistema.
/// Esta clase es el modelo: solo describe los datos, no consulta la base ni muestra pantallas.
/// </summary>
public class Usuario
{
    /// <summary>
    /// Identificador único generado automáticamente por PostgreSQL.
    /// Corresponde a la columna Id_Usuarios de la tabla usuarios.
    /// </summary>
    public int Id_Usuarios { get; set; }

    /// <summary>
    /// Nombre que la persona escribe para iniciar sesión.
    /// La base de datos no permite dos usuarios con el mismo nombre.
    /// Ejemplo: "admin".
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Hash BCrypt de la contraseña.
    /// Nunca contiene ni devuelve la contraseña original escrita por el usuario.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>Clave foránea que enlaza el usuario con su rol obligatorio.</summary>
    public int Id_Roles { get; set; }

    /// <summary>Datos del rol asignado. EF Core carga esta relación desde la tabla roles.</summary>
    public Rol Rol { get; set; } = null!;
}
