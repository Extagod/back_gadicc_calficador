namespace Capa_Abstracciones.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("PERSONA")]
public class Persona
{
    [Key]
    [Column("CEDULARUCPERSONA")]
    [StringLength(14)]
    public string CedulaRucPersona { get; set; } = string.Empty;

    [Column("PRIMERNOMBREPERSONA")]
    [StringLength(100)]
    public string? PrimerNombre { get; set; }

    [Column("SEGUNDONOMBREPERSONA")]
    [StringLength(100)]
    public string? SegundoNombre { get; set; }

    [Column("PRIMERAPELLIDOPERSONA")]
    [StringLength(100)]
    public string? PrimerApellido { get; set; }

    [Column("SEGUNDOAPELLIDOPERSONA")]
    [StringLength(100)]
    public string? SegundoApellido { get; set; }

    [Column("DIRECCIONPERSONA")]
    [StringLength(300)]
    public string? Direccion { get; set; }

    [Column("MOVIL1PERSONA")]
    [StringLength(20)]
    public string? Movil1 { get; set; }

    [Column("EMAIL1PERSONA")]
    [StringLength(100)]
    public string? Email1 { get; set; }

    [Column("ACTIVO")]
    public int? Activo { get; set; }

    // Nombre completo (propiedad calculada)
    [NotMapped]
    public string NombreCompleto => $"{PrimerNombre} {PrimerApellido}".Trim();

    // Navegación
    public Empleado? Empleado { get; set; }
}
