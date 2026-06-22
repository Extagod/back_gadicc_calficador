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

    public async Task<Encargado?> ObtenerPorIdAsync(int id)
        => await _context.Encargados.FindAsync(id);

    public async Task<Encargado?> ObtenerPorTokenQRAsync(string tokenQR)
        => await _context.Encargados
            .FirstOrDefaultAsync(e => e.TokenQR == tokenQR);

    public async Task<IEnumerable<Encargado>> ObtenerTodosAsync()
        => await _context.Encargados.AsNoTracking().ToListAsync();

    public async Task<Encargado> AgregarAsync(Encargado encargado)
    {
        _context.Encargados.Add(encargado);
        await _context.SaveChangesAsync();
        return encargado;
    }

    public async Task ActualizarAsync(Encargado encargado)
    {
        _context.Encargados.Update(encargado);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Encargado encargado)
    {
        _context.Encargados.Remove(encargado);
        await _context.SaveChangesAsync();
    }
}
