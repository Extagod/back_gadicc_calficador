namespace Capa_Abstracciones.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("USUARIOS_ADMIN")]
public class UsuarioAdmin
{
    [Key]
    [Column("IDUSUARIO")]
    public int IdUsuario { get; set; }

    [Required]
    [Column("NOMBREUSUARIO")]
    [StringLength(50)]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required]
    [Column("PASSWORDHASH")]
    [StringLength(200)]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("FECHACREACION")]
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
