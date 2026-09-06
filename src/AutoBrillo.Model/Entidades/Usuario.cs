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
}
