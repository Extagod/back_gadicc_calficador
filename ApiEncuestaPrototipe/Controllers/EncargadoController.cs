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

        var enc = resultado.Value!;
        var frontendUrl = _config["AppSettings:FrontendUrl"] ?? "http://localhost:5173";
        return CreatedAtAction(nameof(ObtenerQR), new { id = enc.IdEncargado }, new
        {
            enc.IdEncargado,
            enc.Nombre,
            enc.Apellido,
            enc.TokenQR,
            urlEncuestaLocal = $"http://localhost:5173/encuesta/{enc.TokenQR}",
            urlEncuestaRed = $"{frontendUrl}/encuesta/{enc.TokenQR}",
            QRBase64 = enc.CodigoQR
        });
    }

    [HttpGet("token/{token}")]
    public async Task<IActionResult> ObtenerPorToken(string token)
    {
        var resultado = await _encargadoService.ObtenerPorTokenQRAsync(token);
        if (!resultado.IsSuccess)
            return MapError(resultado);

        var enc = resultado.Value!;
        return Ok(new
        {
            idEncargado = enc.IdEncargado,
            nombre = enc.Nombre,
            apellido = enc.Apellido,
            cargo = enc.Cargo ?? ""
        });
    }

    [HttpGet("{id}/qr")]
    public async Task<IActionResult> ObtenerQR(int id)
    {
        var resultado = await _encargadoService.ObtenerPorIdAsync(id);
        if (!resultado.IsSuccess)
            return MapError(resultado);

        var enc = resultado.Value!;
        if (string.IsNullOrEmpty(enc.CodigoQR))
            return NotFound(new { mensaje = "Este encargado no tiene QR generado." });

        var bytes = Convert.FromBase64String(enc.CodigoQR);
        return File(bytes, "image/png");
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var resultado = await _encargadoService.ObtenerTodosAsync();
        return Ok(resultado.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarEncargado(int id, [FromBody] CrearEncargadoDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var resultado = await _encargadoService.ActualizarEncargadoAsync(id, dto);
        if (!resultado.IsSuccess)
            return MapError(resultado);

        return Ok(resultado.Value);
    }

    [HttpPost("{id}/regenerar-qr")]
    public async Task<IActionResult> RegenerarQR(int id)
    {
        var resultado = await _encargadoService.RegenerarQRAsync(id);
        if (!resultado.IsSuccess)
            return MapError(resultado);

        // Obtener el encargado actualizado para devolver el token y la URL
        var encResult = await _encargadoService.ObtenerPorIdAsync(id);
        var frontendUrl = _config["AppSettings:FrontendUrl"] ?? "http://localhost:5173";
        var token = encResult.Value?.TokenQR ?? "";

        return Ok(new
        {
            qrBase64 = resultado.Value,
            tokenQR = token,
            urlEncuestaLocal = $"http://localhost:5173/encuesta/{token}",
            urlEncuestaRed = $"{frontendUrl}/encuesta/{token}"
        });
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
