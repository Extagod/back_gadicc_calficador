namespace Capa_Abstracciones.Interfaces;

using Capa_Abstracciones.Entities;

public interface IUsuarioAdminRepository
{
    Task<UsuarioAdmin?> ObtenerPorNombreUsuarioAsync(string nombreUsuario);
}
