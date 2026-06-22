namespace Capa_Abstracciones.Entities;

using System.ComponentModel.DataAnnotations;
using Capa_Abstracciones.Enums;

public class Calificacion
{
    [Key]
    public int IdCalificacion { get; set; }

    [Required]
    public int IdEncargado { get; set; }

    [Required]
    public ValorCalificacion Valor { get; set; }

    [MaxLength(500)]
    public string? Comentarios { get; set; }

    public DateTime FechaHora { get; set; } = DateTime.UtcNow;

    // Navigation
    public Encargado Encargado { get; set; } = null!;
}
