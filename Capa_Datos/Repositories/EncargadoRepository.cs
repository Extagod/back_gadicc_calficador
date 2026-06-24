namespace Capa_Datos.Repositories;

using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Interfaces;
using Microsoft.EntityFrameworkCore;

public class EncargadoRepository : IEncargadoRepository
{
    private readonly AppDbContext _context;

    public EncargadoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Empleado?> ObtenerPorCedulaAsync(string cedula)
        => await _context.Empleados
            .Include(e => e.Persona)
            .Include(e => e.EmpleadoQR)
            .FirstOrDefaultAsync(e => e.CedulaRucPersona == cedula);

    public async Task<Empleado?> ObtenerPorTokenQRAsync(string tokenQR)
    {
        var qr = await _context.EmpleadosQR
            .Include(eq => eq.Empleado)
                .ThenInclude(e => e.Persona)
            .FirstOrDefaultAsync(eq => eq.TokenQR == tokenQR && eq.Activo == 1);

        return qr?.Empleado;
    }

    public async Task<IEnumerable<Empleado>> ObtenerTodosAsync()
        => await _context.Empleados
            .Include(e => e.Persona)
            .Include(e => e.EmpleadoQR)
            .Where(e => e.EmpleadoActivo == 1)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Empleado> AgregarAsync(Empleado empleado)
    {
        _context.Empleados.Add(empleado);
        await _context.SaveChangesAsync();
        return empleado;
    }

    public async Task ActualizarAsync(Empleado empleado)
    {
        _context.Empleados.Update(empleado);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Empleado empleado)
    {
        _context.Empleados.Remove(empleado);
        await _context.SaveChangesAsync();
    }

    public async Task<EmpleadoQR?> ObtenerQRPorCedulaAsync(string cedula)
        => await _context.EmpleadosQR
            .FirstOrDefaultAsync(eq => eq.CedulaRucPersona == cedula);

    public async Task GuardarQRAsync(EmpleadoQR empleadoQR)
    {
        var existing = await _context.EmpleadosQR
            .FirstOrDefaultAsync(eq => eq.CedulaRucPersona == empleadoQR.CedulaRucPersona);

        if (existing is not null)
        {
            existing.TokenQR = empleadoQR.TokenQR;
            existing.CodigoQR = empleadoQR.CodigoQR;
            existing.FechaGeneracion = empleadoQR.FechaGeneracion;
            existing.Activo = 1;
            _context.EmpleadosQR.Update(existing);
        }
        else
        {
            _context.EmpleadosQR.Add(empleadoQR);
        }

        await _context.SaveChangesAsync();
    }
}
