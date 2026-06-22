namespace Capa_Abstracciones.Entities;

using System.ComponentModel.DataAnnotations;

public class Encargado
{
    [Key]
    public int IdEncargado { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Apellido { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Cargo { get; set; }

    [MaxLength(200)]
    public string? Direccion { get; set; }

    public string? CodigoQR { get; set; }
    public string? TokenQR { get; set; }

    public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
}
