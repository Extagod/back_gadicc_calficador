namespace Capa_Abstracciones.Interfaces;

using Capa_Abstracciones.Entities;

/// <summary>
/// Repositorio para la entidad Empleado (antes Encargado).
/// La PK es CedulaRucPersona (string de 14 chars).
/// </summary>
public interface IEncargadoRepository
{
    Task<Empleado?> ObtenerPorCedulaAsync(string cedula);
    Task<Empleado?> ObtenerPorTokenQRAsync(string tokenQR);
    Task<IEnumerable<Empleado>> ObtenerTodosAsync();
    Task<Empleado> AgregarAsync(Empleado empleado);
    Task ActualizarAsync(Empleado empleado);
    Task EliminarAsync(Empleado empleado);
    Task<EmpleadoQR?> ObtenerQRPorCedulaAsync(string cedula);
    Task GuardarQRAsync(EmpleadoQR empleadoQR);
}
