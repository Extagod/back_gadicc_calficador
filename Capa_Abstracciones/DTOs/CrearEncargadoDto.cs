namespace Capa_Abstracciones.DTOs;

using System.ComponentModel.DataAnnotations;

public class CrearEncargadoDto
{
    [Required(ErrorMessage = "La cédula es obligatoria.")]
    [StringLength(14, MinimumLength = 10, ErrorMessage = "La cédula debe tener entre 10 y 14 caracteres.")]
    public string CedulaRucPersona { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El Nombre debe tener entre 1 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo Apellido es obligatorio.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El Apellido debe tener entre 1 y 100 caracteres.")]
    public string Apellido { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "El Cargo no puede exceder 100 caracteres.")]
    public string? Cargo { get; set; }

    [MaxLength(300, ErrorMessage = "La Dirección no puede exceder 300 caracteres.")]
    public string? Direccion { get; set; }
}
