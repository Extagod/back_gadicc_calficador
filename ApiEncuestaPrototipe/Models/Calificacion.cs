using System.ComponentModel.DataAnnotations;

namespace ApiEncuestaPrototipe.Models;

public class Calificacion
{
    [Key]
    public int IdCalificacion { get; set; }
    public int IdEncargado { get; set; }
    public string Valor { get; set; } = string.Empty;   // Excelente | Buena | Regular | Mala
    public string? Comentarios { get; set; }
    public DateTime FechaHora { get; set; } = DateTime.UtcNow;

    // Navegación
    public Encargado Encargado { get; set; } = null!;
}
