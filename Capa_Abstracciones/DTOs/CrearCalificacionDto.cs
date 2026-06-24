namespace Capa_Abstracciones.DTOs;

using System.ComponentModel.DataAnnotations;

public class CrearCalificacionDto
{
    [Required(ErrorMessage = "La cédula del empleado es obligatoria.")]
    [StringLength(14, MinimumLength = 10, ErrorMessage = "La cédula debe tener entre 10 y 14 caracteres.")]
    public string CedulaEmpleado { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo Calificacion es obligatorio.")]
    [MaxLength(20, ErrorMessage = "El campo Calificacion no puede exceder 20 caracteres.")]
    public string Calificacion { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Los comentarios no pueden exceder 500 caracteres.")]
    public string? Comentarios { get; set; }

    public DateTime? FechaCliente { get; set; }
}
