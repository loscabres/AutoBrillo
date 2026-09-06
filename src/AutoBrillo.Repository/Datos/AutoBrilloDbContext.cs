using AutoBrillo.Model.Entidades;
using Microsoft.EntityFrameworkCore;

namespace AutoBrillo.Repository.Datos;

/// <summary>Contexto EF Core sin dependencias específicas de Supabase.</summary>
public class AutoBrilloDbContext(DbContextOptions<AutoBrilloDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var usuario = modelBuilder.Entity<Usuario>();
        usuario.ToTable("usuarios");
        usuario.HasKey(x => x.Id_Usuarios);
        usuario.Property(x => x.Id_Usuarios).HasColumnName("Id_Usuarios");
        usuario.Property(x => x.Nombre).HasColumnName("Nombre").HasMaxLength(100).IsRequired();
        usuario.HasIndex(x => x.Nombre).IsUnique();
        usuario.Property(x => x.Password).HasColumnName("Password").IsRequired();
    }
}
