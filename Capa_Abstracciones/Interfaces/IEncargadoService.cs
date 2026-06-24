namespace Capa_Abstracciones.Interfaces;

using Capa_Abstracciones.Common;
using Capa_Abstracciones.DTOs;
using Capa_Abstracciones.Entities;

public interface IEncargadoService
{
    Task<ServiceResult<Empleado>> CrearEncargadoAsync(CrearEncargadoDto dto);
    Task<ServiceResult<Empleado>> ObtenerPorCedulaAsync(string cedula);
    Task<ServiceResult<Empleado>> ObtenerPorTokenQRAsync(string tokenQR);
    Task<ServiceResult<IEnumerable<Empleado>>> ObtenerTodosAsync();
    Task<ServiceResult<Empleado>> ActualizarEncargadoAsync(string cedula, CrearEncargadoDto dto);
    Task<ServiceResult<string>> RegenerarQRAsync(string cedula);
    Task<ServiceResult<bool>> EliminarEncargadoAsync(string cedula);
}
