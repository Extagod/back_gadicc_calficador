namespace Capa_Abstracciones.DTOs;

using System.ComponentModel.DataAnnotations;

public class CrearCalificacionDto
{
    [Required(ErrorMessage = "El campo IdEncargado es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "IdEncargado debe ser mayor a 0.")]
    public int IdEncargado { get; set; }

    [Required(ErrorMessage = "El campo Calificacion es obligatorio.")]
    [MaxLength(20, ErrorMessage = "El campo Calificacion no puede exceder 20 caracteres.")]
    public string Calificacion { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Los comentarios no pueden exceder 500 caracteres.")]
    public string? Comentarios { get; set; }
}
