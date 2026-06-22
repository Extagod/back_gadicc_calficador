namespace Capa_Datos.Repositories;

using Capa_Abstracciones.Entities;
using Capa_Abstracciones.Interfaces;
using Microsoft.EntityFrameworkCore;

public class UsuarioAdminRepository : IUsuarioAdminRepository
{
    private readonly AppDbContext _context;

    public UsuarioAdminRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UsuarioAdmin?> ObtenerPorNombreUsuarioAsync(string nombreUsuario)
        => await _context.UsuariosAdmin
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);
}
