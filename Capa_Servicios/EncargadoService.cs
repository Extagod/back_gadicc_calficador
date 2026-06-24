namespace Capa_Servicios;

using System.Net;
using System.Net.Sockets;
using Capa_Abstracciones.Common;
using Capa_Abstracciones.DTOs;
using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Interfaces;
using Microsoft.Extensions.Configuration;

public class EncargadoService : IEncargadoService
{
    private readonly IEncargadoRepository _encargadoRepository;
    private readonly IQRService _qrService;
    private readonly string _frontendBaseUrl;
    private readonly int _frontendPort;

    public EncargadoService(
        IEncargadoRepository encargadoRepository,
        IQRService qrService,
        IConfiguration configuration)
    {
        _encargadoRepository = encargadoRepository;
        _qrService = qrService;
        _frontendPort = 5173;

        // Si hay URL configurada explícitamente, usarla; si no, auto-detectar IP
        var configUrl = configuration["AppSettings:FrontendUrl"];
        if (!string.IsNullOrWhiteSpace(configUrl) && configUrl != "auto")
        {
            _frontendBaseUrl = configUrl;
        }
        else
        {
            var ip = ObtenerIpLocal();
            _frontendBaseUrl = $"http://{ip}:{_frontendPort}";
        }
    }

    private static string ObtenerIpLocal()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ip = host.AddressList
                .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork
                                  && !IPAddress.IsLoopback(a));
            return ip?.ToString() ?? "localhost";
        }
        catch
        {
            return "localhost";
        }
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
        var urlEncuesta = $"{_frontendBaseUrl}/encuesta/{encargado.TokenQR}";
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
        var urlEncuesta = $"{_frontendBaseUrl}/encuesta/{encargado.TokenQR}";
        encargado.CodigoQR = _qrService.GenerarQRBase64(urlEncuesta);

        await _encargadoRepository.ActualizarAsync(encargado);
        return ServiceResult<string>.Success(encargado.CodigoQR);
    }

    public async Task<ServiceResult<bool>> EliminarEncargadoAsync(int id)
    {
        var encargado = await _encargadoRepository.ObtenerPorIdAsync(id);
        if (encargado is null)
            return ServiceResult<bool>.NotFound($"Encargado con Id {id} no encontrado.");

        await _encargadoRepository.EliminarAsync(encargado);
        return ServiceResult<bool>.Success(true);
    }
}
