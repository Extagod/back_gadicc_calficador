namespace Capa_Datos;

using Capa_Abstracciones.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Encargado> Encargados { get; set; }
    public DbSet<Calificacion> Calificaciones { get; set; }
    public DbSet<UsuarioAdmin> UsuariosAdmin { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Encargado>(entity =>
        {
            entity.ToTable("Encargados");
            entity.HasKey(e => e.IdEncargado);
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Apellido).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Cargo).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.TokenQR).HasMaxLength(32);
            entity.HasIndex(e => e.TokenQR).IsUnique();
        });

        modelBuilder.Entity<Calificacion>(entity =>
        {
            entity.ToTable("Calificaciones");
            entity.HasKey(e => e.IdCalificacion);
            entity.Property(e => e.Valor).IsRequired();
            entity.Property(e => e.Comentarios).HasMaxLength(500);
            entity.Property(e => e.IpCliente).HasMaxLength(45);
            entity.Property(e => e.DeviceFingerprint).HasMaxLength(16);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.HasOne(c => c.Encargado)
                  .WithMany(e => e.Calificaciones)
                  .HasForeignKey(c => c.IdEncargado)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UsuarioAdmin>(entity =>
        {
            entity.ToTable("UsuariosAdmin");
            entity.HasKey(e => e.IdUsuario);
            entity.Property(e => e.NombreUsuario).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.NombreUsuario).IsUnique();
        });
    }
}
