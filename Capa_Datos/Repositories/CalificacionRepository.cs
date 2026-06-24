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

    public async Task<IEnumerable<Calificacion>> ObtenerPorEmpleadoAsync(string cedula)
        => await _context.Calificaciones
            .Where(c => c.CedulaRucPersona == cedula)
            .OrderByDescending(c => c.FechaHora)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Calificacion>> ObtenerPorEmpleadoYRangoFechasAsync(
        string cedula, DateTime fechaInicio, DateTime fechaFin)
        => await _context.Calificaciones
            .Where(c => c.CedulaRucPersona == cedula
                     && c.FechaHora >= fechaInicio
                     && c.FechaHora <= fechaFin)
            .OrderByDescending(c => c.FechaHora)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Calificacion>> ObtenerTodasConEmpleadoAsync()
        => await _context.Calificaciones
            .Include(c => c.Empleado)
                .ThenInclude(e => e.Persona)
            .OrderByDescending(c => c.FechaHora)
            .AsNoTracking()
            .ToListAsync();
}
