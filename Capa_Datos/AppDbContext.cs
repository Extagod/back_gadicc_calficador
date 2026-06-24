namespace Capa_Datos;

using Capa_Abstracciones.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Persona> Personas { get; set; }
    public DbSet<Empleado> Empleados { get; set; }
    public DbSet<EmpleadoQR> EmpleadosQR { get; set; }
    public DbSet<Calificacion> Calificaciones { get; set; }
    public DbSet<UsuarioAdmin> UsuariosAdmin { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // PERSONA
        modelBuilder.Entity<Persona>(entity =>
        {
            entity.ToTable("PERSONA");
            entity.HasKey(e => e.CedulaRucPersona);
            entity.Property(e => e.CedulaRucPersona).HasColumnName("CEDULARUCPERSONA").HasMaxLength(14);
            entity.Property(e => e.PrimerNombre).HasColumnName("PRIMERNOMBREPERSONA").HasMaxLength(100);
            entity.Property(e => e.SegundoNombre).HasColumnName("SEGUNDONOMBREPERSONA").HasMaxLength(100);
            entity.Property(e => e.PrimerApellido).HasColumnName("PRIMERAPELLIDOPERSONA").HasMaxLength(100);
            entity.Property(e => e.SegundoApellido).HasColumnName("SEGUNDOAPELLIDOPERSONA").HasMaxLength(100);
            entity.Property(e => e.Direccion).HasColumnName("DIRECCIONPERSONA").HasMaxLength(300);
            entity.Property(e => e.Movil1).HasColumnName("MOVIL1PERSONA").HasMaxLength(20);
            entity.Property(e => e.Email1).HasColumnName("EMAIL1PERSONA").HasMaxLength(100);
            entity.Property(e => e.Activo).HasColumnName("ACTIVO");
        });

        // EMPLEADO (hereda de PERSONA por FK)
        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.ToTable("EMPLEADO");
            entity.HasKey(e => e.CedulaRucPersona);
            entity.Property(e => e.CedulaRucPersona).HasColumnName("CEDULARUCPERSONA").HasMaxLength(14);
            entity.Property(e => e.Notas).HasColumnName("NOTASEMPLEADO").HasMaxLength(250);
            entity.Property(e => e.IdDepartamento).HasColumnName("IDDEPARTAMENTO");
            entity.Property(e => e.Cargo).HasColumnName("CARGOEMPLEADO").HasMaxLength(100);
            entity.Property(e => e.IdTipoFuncionario).HasColumnName("IDTIPOFUNCIONARIO");
            entity.Property(e => e.EmpleadoActivo).HasColumnName("EMPLEADOACTIVO");
            entity.Property(e => e.CodigoSectorial).HasColumnName("CODIGOSECTORIAL").HasMaxLength(50);
            entity.Property(e => e.TituloProfesional).HasColumnName("TITULOPROFESIONAL").HasMaxLength(150);

            entity.HasOne(e => e.Persona)
                  .WithOne(p => p.Empleado)
                  .HasForeignKey<Empleado>(e => e.CedulaRucPersona);

            entity.HasOne(e => e.EmpleadoQR)
                  .WithOne(eq => eq.Empleado)
                  .HasForeignKey<EmpleadoQR>(eq => eq.CedulaRucPersona);
        });

        // EMPLEADO_QR
        modelBuilder.Entity<EmpleadoQR>(entity =>
        {
            entity.ToTable("EMPLEADO_QR");
            entity.HasKey(e => e.CedulaRucPersona);
            entity.Property(e => e.CedulaRucPersona).HasColumnName("CEDULARUCPERSONA").HasMaxLength(14);
            entity.Property(e => e.TokenQR).HasColumnName("TOKENQR").HasMaxLength(32).IsRequired();
            entity.Property(e => e.CodigoQR).HasColumnName("CODIGOQR");
            entity.Property(e => e.FechaGeneracion).HasColumnName("FECHAGENERACION");
            entity.Property(e => e.Activo).HasColumnName("ACTIVO");
            entity.HasIndex(e => e.TokenQR).IsUnique();
        });

        // CALIFICACIONES
        modelBuilder.Entity<Calificacion>(entity =>
        {
            entity.ToTable("CALIFICACIONES");
            entity.HasKey(e => e.IdCalificacion);
            entity.Property(e => e.IdCalificacion).HasColumnName("IDCALIFICACION")
                  .UseHiLo("SEQ_CALIFICACIONES");
            entity.Property(e => e.CedulaRucPersona).HasColumnName("CEDULARUCPERSONA").HasMaxLength(14).IsRequired();
            entity.Property(e => e.Valor).HasColumnName("VALOR").IsRequired();
            entity.Property(e => e.Comentarios).HasColumnName("COMENTARIOS").HasMaxLength(500);
            entity.Property(e => e.FechaHora).HasColumnName("FECHAHORA");
            entity.Property(e => e.IpCliente).HasColumnName("IPCLIENTE").HasMaxLength(45);
            entity.Property(e => e.DeviceFingerprint).HasColumnName("DEVICEFINGERPRINT").HasMaxLength(16);
            entity.Property(e => e.UserAgent).HasColumnName("USERAGENT").HasMaxLength(500);
            entity.Property(e => e.FechaCliente).HasColumnName("FECHACLIENTE");

            entity.HasOne(c => c.Empleado)
                  .WithMany(e => e.Calificaciones)
                  .HasForeignKey(c => c.CedulaRucPersona);
        });

        // USUARIOS_ADMIN
        modelBuilder.Entity<UsuarioAdmin>(entity =>
        {
            entity.ToTable("USUARIOS_ADMIN");
            entity.HasKey(e => e.IdUsuario);
            entity.Property(e => e.IdUsuario).HasColumnName("IDUSUARIO")
                  .UseHiLo("SEQ_USUARIOS_ADMIN");
            entity.Property(e => e.NombreUsuario).HasColumnName("NOMBREUSUARIO").HasMaxLength(50).IsRequired();
            entity.Property(e => e.PasswordHash).HasColumnName("PASSWORDHASH").HasMaxLength(200).IsRequired();
            entity.Property(e => e.FechaCreacion).HasColumnName("FECHACREACION");
            entity.HasIndex(e => e.NombreUsuario).IsUnique();
        });
    }
}
