using ApiEncuestaPrototipe.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiEncuestaPrototipe.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Encargado> Encargados { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ── Encargado ──────────────────────────────────────────────────
            modelBuilder.Entity<Encargado>(entity =>
            {
                entity.ToTable("encargado");
                entity.HasKey(e => e.IdEncargado);
                entity.Property(e => e.IdEncargado).HasColumnName("id_encargado");
                entity.Property(e => e.Nombre).HasColumnName("nombre");
                entity.Property(e => e.Apellido).HasColumnName("apellido");
                entity.Property(e => e.Direccion).HasColumnName("direccion");
                entity.Property(e => e.Edad).HasColumnName("edad");
                entity.Property(e => e.CodigoQR).HasColumnName("codigo_qr");
                entity.Property(e => e.TokenQR).HasColumnName("token_qr");
                entity.Property(e => e.Cargo).HasColumnName("cargo");
            });

            // ── Calificacion ───────────────────────────────────────────────
            modelBuilder.Entity<Calificacion>(entity =>
            {
                entity.ToTable("calificacion");
                entity.HasKey(e => e.IdCalificacion);
                entity.Property(e => e.IdCalificacion).HasColumnName("id_calificacion");
                entity.Property(e => e.IdEncargado).HasColumnName("id_encargado");
                entity.Property(e => e.Valor).HasColumnName("calificacion");
                entity.Property(e => e.Comentarios).HasColumnName("comentarios");
                entity.Property(e => e.FechaHora).HasColumnName("fecha_hora");
            });

            modelBuilder.Entity<Calificacion>()
                .HasOne(c => c.Encargado)
                .WithMany(e => e.Calificaciones)
                .HasForeignKey(c => c.IdEncargado);
        }
    }
}
