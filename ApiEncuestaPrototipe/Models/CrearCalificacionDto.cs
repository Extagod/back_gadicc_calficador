namespace ApiEncuestaPrototipe.Models;

public class CrearCalificacionDto
{
    public int IdEncargado { get; set; }
    public string Calificacion { get; set; } = string.Empty;
    public string? Comentarios { get; set; }
}
