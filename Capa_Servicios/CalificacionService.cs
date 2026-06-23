namespace Capa_Servicios;

using Capa_Abstracciones.Common;
using Capa_Abstracciones.DTOs;
using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Enums;
using Capa_Abstracciones.Interfaces;

public class CalificacionService : ICalificacionService
{
    private readonly ICalificacionRepository _calificacionRepository;
    private readonly IEncargadoRepository _encargadoRepository;

    public CalificacionService(
        ICalificacionRepository calificacionRepository,
        IEncargadoRepository encargadoRepository)
    {
        _calificacionRepository = calificacionRepository;
        _encargadoRepository = encargadoRepository;
    }

    public async Task<ServiceResult<Calificacion>> CrearCalificacionAsync(CrearCalificacionDto dto, string? ip = null, string? fingerprint = null, string? userAgent = null)
    {
        // Validate non-empty calificacion
        if (string.IsNullOrWhiteSpace(dto.Calificacion))
        {
            return ServiceResult<Calificacion>.ValidationError(
                "El campo Calificacion es obligatorio.");
        }

        // Parse enum case-insensitive
        if (!Enum.TryParse<ValorCalificacion>(dto.Calificacion, ignoreCase: true, out var valor))
        {
            var valoresValidos = string.Join(", ", Enum.GetNames<ValorCalificacion>());
            return ServiceResult<Calificacion>.ValidationError(
                $"Valor '{dto.Calificacion}' no es válido. Valores aceptados: {valoresValidos}");
        }

        // Verify encargado exists
        var encargado = await _encargadoRepository.ObtenerPorIdAsync(dto.IdEncargado);
        if (encargado is null)
        {
            return ServiceResult<Calificacion>.NotFound(
                $"Encargado con Id {dto.IdEncargado} no encontrado.");
        }

        var calificacion = new Calificacion
        {
            IdEncargado = dto.IdEncargado,
            Valor = valor,
            Comentarios = dto.Comentarios?.Trim(),
            FechaHora = DateTime.UtcNow,
            IpCliente = ip,
            DeviceFingerprint = fingerprint,
            UserAgent = userAgent,
            FechaCliente = dto.FechaCliente
        };

        var resultado = await _calificacionRepository.AgregarAsync(calificacion);
        return ServiceResult<Calificacion>.Success(resultado);
    }

    public async Task<ServiceResult<IEnumerable<Calificacion>>> ObtenerPorEncargadoAsync(int idEncargado)
    {
        var encargado = await _encargadoRepository.ObtenerPorIdAsync(idEncargado);
        if (encargado is null)
        {
            return ServiceResult<IEnumerable<Calificacion>>.NotFound(
                $"Encargado con Id {idEncargado} no encontrado.");
        }

        var calificaciones = await _calificacionRepository.ObtenerPorEncargadoIdAsync(idEncargado);
        return ServiceResult<IEnumerable<Calificacion>>.Success(calificaciones);
    }

    public async Task<ServiceResult<IEnumerable<Calificacion>>> ObtenerPorEncargadoYRangoAsync(
        int idEncargado, DateTime fechaInicio, DateTime fechaFin)
    {
        if (fechaInicio > fechaFin)
        {
            return ServiceResult<IEnumerable<Calificacion>>.ValidationError(
                "La fecha de inicio no puede ser posterior a la fecha de fin.");
        }

        var encargado = await _encargadoRepository.ObtenerPorIdAsync(idEncargado);
        if (encargado is null)
        {
            return ServiceResult<IEnumerable<Calificacion>>.NotFound(
                $"Encargado con Id {idEncargado} no encontrado.");
        }

        var calificaciones = await _calificacionRepository
            .ObtenerPorEncargadoYRangoFechasAsync(idEncargado, fechaInicio, fechaFin);
        return ServiceResult<IEnumerable<Calificacion>>.Success(calificaciones);
    }

    public async Task<ServiceResult<IEnumerable<Calificacion>>> ObtenerTodasAsync()
    {
        var calificaciones = await _calificacionRepository.ObtenerTodasConEncargadoAsync();
        return ServiceResult<IEnumerable<Calificacion>>.Success(calificaciones);
    }
}
