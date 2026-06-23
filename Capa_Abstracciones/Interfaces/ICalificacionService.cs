namespace Capa_Abstracciones.Interfaces;

using Capa_Abstracciones.Common;
using Capa_Abstracciones.DTOs;
using Capa_Abstracciones.Entities;

public interface ICalificacionService
{
    Task<ServiceResult<Calificacion>> CrearCalificacionAsync(CrearCalificacionDto dto, string? ip = null, string? fingerprint = null, string? userAgent = null);
    Task<ServiceResult<IEnumerable<Calificacion>>> ObtenerPorEncargadoAsync(int idEncargado);
    Task<ServiceResult<IEnumerable<Calificacion>>> ObtenerPorEncargadoYRangoAsync(
        int idEncargado, DateTime fechaInicio, DateTime fechaFin);
    Task<ServiceResult<IEnumerable<Calificacion>>> ObtenerTodasAsync();
}
