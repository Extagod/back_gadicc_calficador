namespace Capa_Servicios;

using Capa_Abstracciones.Common;
using Capa_Abstracciones.DTOs;
using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Interfaces;
using Microsoft.Extensions.Configuration;

public class EncargadoService : IEncargadoService
{
    private readonly IEncargadoRepository _encargadoRepository;
    private readonly IQRService _qrService;
    private readonly string _frontendUrl;

    public EncargadoService(
        IEncargadoRepository encargadoRepository,
        IQRService qrService,
        IConfiguration configuration)
    {
        _encargadoRepository = encargadoRepository;
        _qrService = qrService;
        _frontendUrl = configuration["AppSettings:FrontendUrl"] ?? "http://localhost:5173";
    }

    public async Task<ServiceResult<Encargado>> CrearEncargadoAsync(CrearEncargadoDto dto)
    {
        var encargado = new Encargado
        {
            Nombre = dto.Nombre.Trim(),
            Apellido = dto.Apellido.Trim(),
            Cargo = dto.Cargo?.Trim(),
            Direccion = dto.Direccion?.Trim()
        };

        encargado.TokenQR = Guid.NewGuid().ToString("N");
        var urlEncuesta = $"{_frontendUrl}/encuesta/{encargado.TokenQR}";
        encargado.CodigoQR = _qrService.GenerarQRBase64(urlEncuesta);

        var resultado = await _encargadoRepository.AgregarAsync(encargado);
        return ServiceResult<Encargado>.Success(resultado);
    }

    public async Task<ServiceResult<Encargado>> ObtenerPorIdAsync(int id)
    {
        var encargado = await _encargadoRepository.ObtenerPorIdAsync(id);
        if (encargado is null)
            return ServiceResult<Encargado>.NotFound($"Encargado con Id {id} no encontrado.");
        return ServiceResult<Encargado>.Success(encargado);
    }

    public async Task<ServiceResult<Encargado>> ObtenerPorTokenQRAsync(string tokenQR)
    {
        var encargado = await _encargadoRepository.ObtenerPorTokenQRAsync(tokenQR);
        if (encargado is null)
            return ServiceResult<Encargado>.NotFound("El código QR no es válido o ya no está disponible.");
        return ServiceResult<Encargado>.Success(encargado);
    }

    public async Task<ServiceResult<IEnumerable<Encargado>>> ObtenerTodosAsync()
    {
        var encargados = await _encargadoRepository.ObtenerTodosAsync();
        return ServiceResult<IEnumerable<Encargado>>.Success(encargados);
    }

    public async Task<ServiceResult<Encargado>> ActualizarEncargadoAsync(int id, CrearEncargadoDto dto)
    {
        var encargado = await _encargadoRepository.ObtenerPorIdAsync(id);
        if (encargado is null)
            return ServiceResult<Encargado>.NotFound($"Encargado con Id {id} no encontrado.");

        encargado.Nombre = dto.Nombre.Trim();
        encargado.Apellido = dto.Apellido.Trim();
        encargado.Cargo = dto.Cargo?.Trim();
        encargado.Direccion = dto.Direccion?.Trim();

        await _encargadoRepository.ActualizarAsync(encargado);
        return ServiceResult<Encargado>.Success(encargado);
    }

    public async Task<ServiceResult<string>> RegenerarQRAsync(int id)
    {
        var encargado = await _encargadoRepository.ObtenerPorIdAsync(id);
        if (encargado is null)
            return ServiceResult<string>.NotFound($"Encargado con Id {id} no encontrado.");

        encargado.TokenQR = Guid.NewGuid().ToString("N");
        var urlEncuesta = $"{_frontendUrl}/encuesta/{encargado.TokenQR}";
        encargado.CodigoQR = _qrService.GenerarQRBase64(urlEncuesta);

        await _encargadoRepository.ActualizarAsync(encargado);
        return ServiceResult<string>.Success(encargado.CodigoQR);
    }
}
