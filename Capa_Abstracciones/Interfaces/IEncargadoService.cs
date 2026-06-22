namespace Capa_Abstracciones.Interfaces;

using Capa_Abstracciones.Common;
using Capa_Abstracciones.DTOs;
using Capa_Abstracciones.Entities;

public interface IEncargadoService
{
    Task<ServiceResult<Encargado>> CrearEncargadoAsync(CrearEncargadoDto dto);
    Task<ServiceResult<Encargado>> ObtenerPorIdAsync(int id);
    Task<ServiceResult<Encargado>> ObtenerPorTokenQRAsync(string tokenQR);
    Task<ServiceResult<IEnumerable<Encargado>>> ObtenerTodosAsync();
    Task<ServiceResult<Encargado>> ActualizarEncargadoAsync(int id, CrearEncargadoDto dto);
    Task<ServiceResult<string>> RegenerarQRAsync(int id);
}
