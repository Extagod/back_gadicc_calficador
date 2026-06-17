namespace ApiEncuestaPrototipe.Models;

public class CalificacionDto
{
    public string Calificacion { get; set; } = string.Empty;  // Excelente | Buena | Regular | Mala
    public string? Comentarios { get; set; }
}