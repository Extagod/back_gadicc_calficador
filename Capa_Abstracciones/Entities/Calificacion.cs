namespace Capa_Abstracciones.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Capa_Abstracciones.Enums;

[Table("CALIFICACIONES")]
public class Calificacion
{
    [Key]
    [Column("IDCALIFICACION")]
    public int IdCalificacion { get; set; }

    [Required]
    [Column("CEDULARUCPERSONA")]
    [StringLength(14)]
    public string CedulaRucPersona { get; set; } = string.Empty;

    [Required]
    [Column("VALOR")]
    public ValorCalificacion Valor { get; set; }

    [Column("COMENTARIOS")]
    [MaxLength(500)]
    public string? Comentarios { get; set; }

    [Column("FECHAHORA")]
    public DateTime FechaHora { get; set; } = DateTime.UtcNow;

    [Column("IPCLIENTE")]
    [MaxLength(45)]
    public string? IpCliente { get; set; }

    [Column("DEVICEFINGERPRINT")]
    [MaxLength(16)]
    public string? DeviceFingerprint { get; set; }

    [Column("USERAGENT")]
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    [Column("FECHACLIENTE")]
    public DateTime? FechaCliente { get; set; }

    // Navegación
    [ForeignKey("CedulaRucPersona")]
    public Empleado Empleado { get; set; } = null!;
}
