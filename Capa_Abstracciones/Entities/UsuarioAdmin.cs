namespace Capa_Abstracciones.Entities;

using System.ComponentModel.DataAnnotations;

public class UsuarioAdmin
{
    [Key]
    public int IdUsuario { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
