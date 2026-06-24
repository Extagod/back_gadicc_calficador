using Microsoft.AspNetCore.Mvc;
using Capa_Abstracciones.Common;
using Capa_Abstracciones.DTOs;
using Capa_Abstracciones.Interfaces;

namespace ApiEncuestaPrototipe.Controllers;

[ApiController]
[Route("api/encargados")]
public class EncargadoController : ControllerBase
{
    private readonly IEncargadoService _encargadoService;
    private readonly IConfiguration _config;

    public EncargadoController(IEncargadoService encargadoService, IConfiguration config)
    {
        _encargadoService = encargadoService;
        _config = config;
    }

    [HttpPost]
    public async Task<IActionResult> CrearEncargado([FromBody] CrearEncargadoDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var resultado = await _encargadoService.CrearEncargadoAsync(dto);
        if (!resultado.IsSuccess)
            return MapError(resultado);

        var emp = resultado.Value!;
        var token = emp.EmpleadoQR?.TokenQR ?? "";
        var host = Request.Host.Host;
        var networkUrl = host == "localhost" || host == "127.0.0.1"
            ? $"http://localhost:5173/encuesta/{token}"
            : $"http://{host}:5173/encuesta/{token}";

        return CreatedAtAction(nameof(ObtenerQR), new { cedula = emp.CedulaRucPersona }, new
        {
            Cedula = emp.CedulaRucPersona,
            Nombre = emp.Persona?.PrimerNombre,
            Apellido = emp.Persona?.PrimerApellido,
            TokenQR = token,
            urlEncuestaLocal = $"http://localhost:5173/encuesta/{token}",
            urlEncuestaRed = networkUrl,
            QRBase64 = emp.EmpleadoQR?.CodigoQR
        });
    }

    [HttpGet("token/{token}")]
    public async Task<IActionResult> ObtenerPorToken(string token)
    {
        var resultado = await _encargadoService.ObtenerPorTokenQRAsync(token);
        if (!resultado.IsSuccess)
            return MapError(resultado);

        var emp = resultado.Value!;
        return Ok(new
        {
            cedula = emp.CedulaRucPersona,
            nombre = emp.Persona?.PrimerNombre ?? "",
            apellido = emp.Persona?.PrimerApellido ?? "",
            cargo = emp.Cargo ?? ""
        });
    }

    [HttpGet("{cedula}/qr")]
    public async Task<IActionResult> ObtenerQR(string cedula)
    {
        var resultado = await _encargadoService.ObtenerPorCedulaAsync(cedula);
        if (!resultado.IsSuccess)
            return MapError(resultado);

        var emp = resultado.Value!;
        var codigoQR = emp.EmpleadoQR?.CodigoQR;
        if (string.IsNullOrEmpty(codigoQR))
            return NotFound(new { mensaje = "Este empleado no tiene QR generado." });

        var bytes = Convert.FromBase64String(codigoQR);
        return File(bytes, "image/png");
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var resultado = await _encargadoService.ObtenerTodosAsync();
        return Ok(resultado.Value);
    }

    [HttpGet("{cedula}")]
    public async Task<IActionResult> ObtenerPorCedula(string cedula)
    {
        var resultado = await _encargadoService.ObtenerPorCedulaAsync(cedula);
        if (!resultado.IsSuccess)
            return MapError(resultado);

        var emp = resultado.Value!;
        return Ok(new
        {
            cedula = emp.CedulaRucPersona,
            nombre = emp.Persona?.PrimerNombre ?? "",
            apellido = emp.Persona?.PrimerApellido ?? "",
            cargo = emp.Cargo ?? "",
            direccion = emp.Persona?.Direccion ?? "",
            tokenQR = emp.EmpleadoQR?.TokenQR ?? "",
            tieneQR = !string.IsNullOrEmpty(emp.EmpleadoQR?.CodigoQR)
        });
    }

    [HttpPut("{cedula}")]
    public async Task<IActionResult> ActualizarEncargado(string cedula, [FromBody] CrearEncargadoDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var resultado = await _encargadoService.ActualizarEncargadoAsync(cedula, dto);
        if (!resultado.IsSuccess)
            return MapError(resultado);

        return Ok(resultado.Value);
    }

    [HttpPost("{cedula}/regenerar-qr")]
    public async Task<IActionResult> RegenerarQR(string cedula)
    {
        var resultado = await _encargadoService.RegenerarQRAsync(cedula);
        if (!resultado.IsSuccess)
            return MapError(resultado);

        // Obtener el empleado actualizado para devolver el token y la URL
        var encResult = await _encargadoService.ObtenerPorCedulaAsync(cedula);
        var token = encResult.Value?.EmpleadoQR?.TokenQR ?? "";
        var host = Request.Host.Host;
        var networkUrl = host == "localhost" || host == "127.0.0.1"
            ? $"http://localhost:5173/encuesta/{token}"
            : $"http://{host}:5173/encuesta/{token}";

        return Ok(new
        {
            qrBase64 = resultado.Value,
            tokenQR = token,
            urlEncuestaLocal = $"http://localhost:5173/encuesta/{token}",
            urlEncuestaRed = networkUrl
        });
    }

    [HttpDelete("{cedula}")]
    public async Task<IActionResult> EliminarEncargado(string cedula)
    {
        var resultado = await _encargadoService.EliminarEncargadoAsync(cedula);
        if (!resultado.IsSuccess)
            return MapError(resultado);

        return Ok(new { mensaje = "Empleado eliminado correctamente." });
    }

    private IActionResult MapError<T>(ServiceResult<T> result)
    {
        return result.ErrorType switch
        {
            ServiceErrorType.NotFound => NotFound(new { mensaje = result.ErrorMessage }),
            ServiceErrorType.Validation => BadRequest(new { mensaje = result.ErrorMessage }),
            _ => StatusCode(500, new { mensaje = result.ErrorMessage })
        };
    }
}
