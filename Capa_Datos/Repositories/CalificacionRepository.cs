namespace Capa_Datos.Repositories;

using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Interfaces;
using Microsoft.EntityFrameworkCore;

public class CalificacionRepository : ICalificacionRepository
{
    private readonly AppDbContext _context;

    public CalificacionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Calificacion> AgregarAsync(Calificacion calificacion)
    {
        _context.Calificaciones.Add(calificacion);
        await _context.SaveChangesAsync();
        return calificacion;
    }

    public async Task<IEnumerable<Calificacion>> ObtenerPorEncargadoIdAsync(int idEncargado)
        => await _context.Calificaciones
            .Where(c => c.IdEncargado == idEncargado)
            .OrderByDescending(c => c.FechaHora)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Calificacion>> ObtenerPorEncargadoYRangoFechasAsync(
        int idEncargado, DateTime fechaInicio, DateTime fechaFin)
        => await _context.Calificaciones
            .Where(c => c.IdEncargado == idEncargado
                     && c.FechaHora >= fechaInicio
                     && c.FechaHora <= fechaFin)
            .OrderByDescending(c => c.FechaHora)
            .AsNoTracking()
            .ToListAsync();
}
