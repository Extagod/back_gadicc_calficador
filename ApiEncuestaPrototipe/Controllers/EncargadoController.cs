using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiEncuestaPrototipe.Data;
using ApiEncuestaPrototipe.Models;
using ApiEncuestaPrototipe.Services;

namespace ApiEncuestaPrototipe.Controllers
{
    [ApiController]
    [Route("api/encargados")]  // ← plural para coincidir con el frontend
    public class EncargadoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly QRService _qrService;
        private readonly IConfiguration _config;

        public EncargadoController(AppDbContext context, QRService qrService, IConfiguration config)
        {
            _context = context;
            _qrService = qrService;
            _config = config;
        }

        // ─── POST /api/encargados ─────────────────────────────────────────────
        // Crea encargado y genera QR único
        [HttpPost]
        public async Task<IActionResult> CrearEncargado([FromBody] Encargado encargado)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            encargado.TokenQR = Guid.NewGuid().ToString("N");

            var baseUrl = _config["AppSettings:FrontendUrl"] ?? "http://localhost:5173";
            var urlEncuesta = $"{baseUrl}/encuesta/{encargado.TokenQR}";

            encargado.CodigoQR = _qrService.GenerarQRBase64(urlEncuesta);

            _context.Encargados.Add(encargado);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerQR), new { id = encargado.IdEncargado }, new
            {
                encargado.IdEncargado,
                encargado.Nombre,
                encargado.Apellido,
                encargado.TokenQR,
                UrlEncuesta = urlEncuesta,
                QRBase64 = encargado.CodigoQR
            });
        }

        // ─── GET /api/encargados/token/{token} ────────────────────────────────
        // LLAMADA #1 del frontend: al escanear el QR obtiene datos del encargado
        // Respuesta esperada por el frontend:
        // { idEncargado, nombre, apellido, cargo }
        [HttpGet("token/{token}")]
        public async Task<IActionResult> ObtenerPorToken(string token)
        {
            var encargado = await _context.Encargados
                .FirstOrDefaultAsync(e => e.TokenQR == token);

            if (encargado == null)
                return NotFound(new { mensaje = "El código QR no es válido o ya no está disponible." });

            return Ok(new
            {
                idEncargado = encargado.IdEncargado,  // camelCase para el frontend
                nombre = encargado.Nombre,
                apellido = encargado.Apellido,
                cargo = encargado.Cargo ?? ""
            });
        }

        // ─── POST /api/calificaciones ─────────────────────────────────────────
        // LLAMADA #2 del frontend: inserta nueva calificación (nunca sobreescribe)
        [HttpPost("/api/calificaciones")]
        public async Task<IActionResult> CrearCalificacion([FromBody] CrearCalificacionDto dto)
        {
            var encargado = await _context.Encargados.FindAsync(dto.IdEncargado);
            if (encargado == null)
                return NotFound(new { mensaje = "Encargado no encontrado." });

            var calificacion = new Calificacion
            {
                IdEncargado = dto.IdEncargado,
                Valor = dto.Calificacion,
                Comentarios = dto.Comentarios,
                FechaHora = DateTime.UtcNow
            };

            _context.Calificaciones.Add(calificacion);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Calificación guardada correctamente." });
        }


        // ─── GET /api/encargados/{id}/qr ──────────────────────────────────────
        // Devuelve imagen PNG del QR para imprimir/mostrar en panel admin
        [HttpGet("{id}/qr")]
        public async Task<IActionResult> ObtenerQR(int id)
        {
            var encargado = await _context.Encargados.FindAsync(id);
            if (encargado == null) return NotFound();

            if (string.IsNullOrEmpty(encargado.CodigoQR))
                return NotFound(new { mensaje = "Este encargado no tiene QR generado." });

            var bytes = Convert.FromBase64String(encargado.CodigoQR);
            return File(bytes, "image/png");
        }
    }
}