namespace Capa_Abstracciones.DTOs;

using System.ComponentModel.DataAnnotations;

public class CrearEncargadoDto
{
    [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El Nombre debe tener entre 1 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo Apellido es obligatorio.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "El Apellido debe tener entre 1 y 100 caracteres.")]
    public string Apellido { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "El Cargo no puede exceder 100 caracteres.")]
    public string? Cargo { get; set; }

    [MaxLength(200, ErrorMessage = "La Dirección no puede exceder 200 caracteres.")]
    public string? Direccion { get; set; }
}
