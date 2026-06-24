namespace Capa_Abstracciones.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("EMPLEADO_QR")]
public class EmpleadoQR
{
    [Key]
    [Column("CEDULARUCPERSONA")]
    [StringLength(14)]
    public string CedulaRucPersona { get; set; } = string.Empty;

    [Required]
    [Column("TOKENQR")]
    [StringLength(32)]
    public string TokenQR { get; set; } = string.Empty;

    [Column("CODIGOQR")]
    public string? CodigoQR { get; set; }

    [Column("FECHAGENERACION")]
    public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;

    [Column("ACTIVO")]
    public int Activo { get; set; } = 1;

    // Navegación
    [ForeignKey("CedulaRucPersona")]
    public Empleado Empleado { get; set; } = null!;
}
