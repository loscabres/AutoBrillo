using AutoBrillo.Model.Entidades;
using Microsoft.EntityFrameworkCore;

namespace AutoBrillo.Repository.Datos;

/// <summary>
/// Puente entre las clases de C# y las tablas de PostgreSQL.
/// EF Core usa esta clase para saber qué tabla y qué columnas debe consultar.
/// No depende de Supabase; por eso facilita una futura migración a otra base compatible.
/// </summary>
public class AutoBrilloDbContext(DbContextOptions<AutoBrilloDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Representa la tabla usuarios dentro del código.
    /// Ejemplo: contexto.Usuarios permite hacer consultas con LINQ.
    /// </summary>
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    /// <summary>Representa la tabla roles dentro del código.</summary>
    public DbSet<Rol> Roles => Set<Rol>();

    /// <summary>
    /// Configura cómo se relaciona Usuario con la tabla real de la base de datos.
    /// Se ejecuta cuando EF Core construye su modelo interno.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var usuario = modelBuilder.Entity<Usuario>();

        // La tabla real se llama exactamente usuarios y está en minúsculas.
        usuario.ToTable("usuarios");

        // Id_Usuarios es la clave primaria de cada registro.
        usuario.HasKey(x => x.Id_Usuarios);
        usuario.Property(x => x.Id_Usuarios).HasColumnName("Id_Usuarios");

        // Nombre es obligatorio, admite hasta 100 caracteres y debe ser único.
        usuario.Property(x => x.Nombre).HasColumnName("Nombre").HasMaxLength(100).IsRequired();
        usuario.HasIndex(x => x.Nombre).IsUnique();

        // Password es obligatorio y guarda el hash BCrypt, no el texto original.
        usuario.Property(x => x.Password).HasColumnName("Password").IsRequired();

        // Un usuario puede tener varios roles y cada rol puede pertenecer a varios usuarios.
        // La tabla intermedia usuarios_roles guarda cada asignación sin duplicados.
        usuario.HasMany(x => x.Roles)
            .WithMany(x => x.Usuarios)
            .UsingEntity<Dictionary<string, object>>(
                "usuarios_roles",
                relacion => relacion.HasOne<Rol>().WithMany().HasForeignKey("Id_Roles").OnDelete(DeleteBehavior.Restrict),
                relacion => relacion.HasOne<Usuario>().WithMany().HasForeignKey("Id_Usuarios").OnDelete(DeleteBehavior.Cascade),
                relacion =>
                {
                    relacion.ToTable("usuarios_roles");
                    relacion.HasKey("Id_Usuarios", "Id_Roles");
                    relacion.IndexerProperty<int>("Id_Usuarios").HasColumnName("Id_Usuarios");
                    relacion.IndexerProperty<int>("Id_Roles").HasColumnName("Id_Roles");
                });

        var rol = modelBuilder.Entity<Rol>();
        rol.ToTable("roles");
        rol.HasKey(x => x.Id_Roles);
        rol.Property(x => x.Id_Roles).HasColumnName("Id_Roles");
        rol.Property(x => x.Descripcion).HasColumnName("Descripcion").HasMaxLength(100).IsRequired();
        rol.HasIndex(x => x.Descripcion).IsUnique();
    }
}
