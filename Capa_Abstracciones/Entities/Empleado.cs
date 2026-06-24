namespace Capa_Abstracciones.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("EMPLEADO")]
public class Empleado
{
    [Key]
    [Column("CEDULARUCPERSONA")]
    [StringLength(14)]
    public string CedulaRucPersona { get; set; } = string.Empty;

    [Column("NOTASEMPLEADO")]
    [StringLength(250)]
    public string? Notas { get; set; }

    [Column("IDDEPARTAMENTO")]
    public int? IdDepartamento { get; set; }

    [Column("CARGOEMPLEADO")]
    [StringLength(100)]
    public string? Cargo { get; set; }

    [Column("IDTIPOFUNCIONARIO")]
    public int? IdTipoFuncionario { get; set; }

    [Column("EMPLEADOACTIVO")]
    public int? EmpleadoActivo { get; set; }

    [Column("CODIGOSECTORIAL")]
    [StringLength(50)]
    public string? CodigoSectorial { get; set; }

    [Column("TITULOPROFESIONAL")]
    [StringLength(150)]
    public string? TituloProfesional { get; set; }

    // Navegación
    [ForeignKey("CedulaRucPersona")]
    public Persona Persona { get; set; } = null!;

    public EmpleadoQR? EmpleadoQR { get; set; }
    public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();

    // Propiedades de conveniencia (via Persona)
    [NotMapped]
    public string NombreCompleto => Persona?.NombreCompleto ?? CedulaRucPersona;

    [NotMapped]
    public string? PrimerNombre => Persona?.PrimerNombre;

    [NotMapped]
    public string? PrimerApellido => Persona?.PrimerApellido;
}
