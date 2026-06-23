using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Capa_Abstracciones.Common;
using Capa_Abstracciones.DTOs;
using Capa_Abstracciones.Interfaces;

namespace ApiEncuestaPrototipe.Controllers;

[ApiController]
[Route("api/calificaciones")]
public class CalificacionController : ControllerBase
{
    private readonly ICalificacionService _calificacionService;

    public CalificacionController(ICalificacionService calificacionService)
    {
        _calificacionService = calificacionService;
    }

    [HttpPost]
    [EnableRateLimiting("calificaciones")]
    public async Task<IActionResult> CrearCalificacion([FromBody] CrearCalificacionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Obtener IP real (considerar proxy)
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            ip = HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0].Trim();
        }

        // Fingerprint del dispositivo
        var fingerprint = HttpContext.Request.Headers["X-Client-Fingerprint"].FirstOrDefault();

        // UserAgent completo
        var userAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();

        var resultado = await _calificacionService.CrearCalificacionAsync(dto, ip, fingerprint, userAgent);
        if (!resultado.IsSuccess)
        {
            return resultado.ErrorType switch
            {
                ServiceErrorType.NotFound => NotFound(new { mensaje = resultado.ErrorMessage }),
                ServiceErrorType.Validation => BadRequest(new { mensaje = resultado.ErrorMessage }),
                _ => StatusCode(500, new { mensaje = resultado.ErrorMessage })
            };
        }

        return Ok(new { mensaje = "Calificación guardada correctamente." });
    }

    [HttpGet("encargado/{idEncargado}")]
    public async Task<IActionResult> ObtenerPorEncargado(int idEncargado)
    {
        var resultado = await _calificacionService.ObtenerPorEncargadoAsync(idEncargado);
        if (!resultado.IsSuccess)
            return NotFound(new { mensaje = resultado.ErrorMessage });

        return Ok(resultado.Value);
    }
}
