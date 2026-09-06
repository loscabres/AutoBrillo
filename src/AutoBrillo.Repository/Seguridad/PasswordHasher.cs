namespace AutoBrillo.Repository.Seguridad;

/// <summary>Genera y valida hashes BCrypt para contraseñas.</summary>
public class PasswordHasher
{
    public string Hashear(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verificar(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
