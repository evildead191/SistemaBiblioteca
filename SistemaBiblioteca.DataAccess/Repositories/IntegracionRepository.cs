using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.DataAccess.Context;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.DataAccess.Repositories;

public class IntegracionRepository : IIntegracionRepository
{
    private readonly SistemaBibliotecaDbContext _context;

    public IntegracionRepository(
        SistemaBibliotecaDbContext context)
    {
        _context = context;
    }

    public async Task AgregarHistorialAsync(
        IntegracionHistorial historial)
    {
        await _context.IntegracionesHistorial
            .AddAsync(historial);
    }

    public async Task<List<IntegracionHistorial>>
        ObtenerHistorialAsync()
    {
        return await _context.IntegracionesHistorial
            .AsNoTracking()
            .OrderByDescending(x => x.FechaProceso)
            .ToListAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}