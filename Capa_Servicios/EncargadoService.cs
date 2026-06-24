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

    public EncargadoService(
        IEncargadoRepository encargadoRepository,
        IQRService qrService,
        IConfiguration configuration)
    {
        _encargadoRepository = encargadoRepository;
        _qrService = qrService;

        var configUrl = configuration["AppSettings:FrontendUrl"];
        if (!string.IsNullOrWhiteSpace(configUrl) && configUrl != "auto")
        {
            _frontendBaseUrl = configUrl;
        }
        else
        {
            var ip = ObtenerIpLocal();
            _frontendBaseUrl = $"http://{ip}:5173";
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
        catch { return "localhost"; }
    }

    public async Task<ServiceResult<Empleado>> CrearEncargadoAsync(CrearEncargadoDto dto)
    {
        // Crear persona si no existe
        var persona = new Persona
        {
            CedulaRucPersona = dto.CedulaRucPersona.Trim(),
            PrimerNombre = dto.Nombre.Trim(),
            PrimerApellido = dto.Apellido.Trim(),
            Direccion = dto.Direccion?.Trim(),
            Activo = 1
        };

        var empleado = new Empleado
        {
            CedulaRucPersona = dto.CedulaRucPersona.Trim(),
            Cargo = dto.Cargo?.Trim(),
            EmpleadoActivo = 1,
            Persona = persona
        };

        var resultado = await _encargadoRepository.AgregarAsync(empleado);

        // Generar QR automáticamente
        var tokenQR = Guid.NewGuid().ToString("N");
        var urlEncuesta = $"{_frontendBaseUrl}/encuesta/{tokenQR}";
        var codigoQR = _qrService.GenerarQRBase64(urlEncuesta);

        var qr = new EmpleadoQR
        {
            CedulaRucPersona = empleado.CedulaRucPersona,
            TokenQR = tokenQR,
            CodigoQR = codigoQR,
            FechaGeneracion = DateTime.UtcNow,
            Activo = 1
        };
        await _encargadoRepository.GuardarQRAsync(qr);
        empleado.EmpleadoQR = qr;

        return ServiceResult<Empleado>.Success(resultado);
    }

    public async Task<ServiceResult<Empleado>> ObtenerPorCedulaAsync(string cedula)
    {
        var empleado = await _encargadoRepository.ObtenerPorCedulaAsync(cedula);
        if (empleado is null)
            return ServiceResult<Empleado>.NotFound($"Empleado con cédula {cedula} no encontrado.");
        return ServiceResult<Empleado>.Success(empleado);
    }

    public async Task<ServiceResult<Empleado>> ObtenerPorTokenQRAsync(string tokenQR)
    {
        var empleado = await _encargadoRepository.ObtenerPorTokenQRAsync(tokenQR);
        if (empleado is null)
            return ServiceResult<Empleado>.NotFound("El código QR no es válido o ya no está disponible.");
        return ServiceResult<Empleado>.Success(empleado);
    }

    public async Task<ServiceResult<IEnumerable<Empleado>>> ObtenerTodosAsync()
    {
        var empleados = await _encargadoRepository.ObtenerTodosAsync();
        return ServiceResult<IEnumerable<Empleado>>.Success(empleados);
    }

    public async Task<ServiceResult<Empleado>> ActualizarEncargadoAsync(string cedula, CrearEncargadoDto dto)
    {
        var empleado = await _encargadoRepository.ObtenerPorCedulaAsync(cedula);
        if (empleado is null)
            return ServiceResult<Empleado>.NotFound($"Empleado con cédula {cedula} no encontrado.");

        if (empleado.Persona is not null)
        {
            empleado.Persona.PrimerNombre = dto.Nombre.Trim();
            empleado.Persona.PrimerApellido = dto.Apellido.Trim();
            empleado.Persona.Direccion = dto.Direccion?.Trim();
        }
        empleado.Cargo = dto.Cargo?.Trim();

        await _encargadoRepository.ActualizarAsync(empleado);
        return ServiceResult<Empleado>.Success(empleado);
    }

    public async Task<ServiceResult<string>> RegenerarQRAsync(string cedula)
    {
        var empleado = await _encargadoRepository.ObtenerPorCedulaAsync(cedula);
        if (empleado is null)
            return ServiceResult<string>.NotFound($"Empleado con cédula {cedula} no encontrado.");

        var tokenQR = Guid.NewGuid().ToString("N");
        var urlEncuesta = $"{_frontendBaseUrl}/encuesta/{tokenQR}";
        var codigoQR = _qrService.GenerarQRBase64(urlEncuesta);

        var qr = new EmpleadoQR
        {
            CedulaRucPersona = cedula,
            TokenQR = tokenQR,
            CodigoQR = codigoQR,
            FechaGeneracion = DateTime.UtcNow,
            Activo = 1
        };
        await _encargadoRepository.GuardarQRAsync(qr);

        return ServiceResult<string>.Success(codigoQR);
    }

    public async Task<ServiceResult<bool>> EliminarEncargadoAsync(string cedula)
    {
        var empleado = await _encargadoRepository.ObtenerPorCedulaAsync(cedula);
        if (empleado is null)
            return ServiceResult<bool>.NotFound($"Empleado con cédula {cedula} no encontrado.");

        await _encargadoRepository.EliminarAsync(empleado);
        return ServiceResult<bool>.Success(true);
    }
}
