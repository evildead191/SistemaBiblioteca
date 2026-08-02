using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.DataAccess.Context;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.DataAccess.Repositories;

public class PrestamoRepository : IPrestamoRepository
{
    private readonly SistemaBibliotecaDbContext _context;

    public PrestamoRepository(
        SistemaBibliotecaDbContext context)
    {
        _context = context;
    }

    public async Task<List<Prestamo>> ObtenerTodosAsync(
        string? busqueda = null,
        EstadoPrestamo? estado = null,
        bool soloAtrasados = false)
    {
        IQueryable<Prestamo> query =
            _context.Prestamos
                .AsNoTracking()
                .Include(x => x.UsuarioBiblioteca)
                .Include(x => x.Ejemplar)
                    .ThenInclude(x =>
                        x.MaterialBibliografico);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string criterio = busqueda.Trim();

            query = query.Where(x =>
                x.UsuarioBiblioteca.Identificacion
                    .Contains(criterio) ||

                x.UsuarioBiblioteca.NombreCompleto
                    .Contains(criterio) ||

                x.Ejemplar.CodigoBarras
                    .Contains(criterio) ||

                (x.Ejemplar.NumeroInscripcion != null &&
                 x.Ejemplar.NumeroInscripcion
                    .Contains(criterio)) ||

                x.Ejemplar.MaterialBibliografico.Titulo
                    .Contains(criterio) ||

                x.Ejemplar.MaterialBibliografico.Autor
                    .Contains(criterio));
        }

        if (estado.HasValue)
        {
            query = query.Where(x =>
                x.Estado == estado.Value);
        }

        if (soloAtrasados)
        {
            DateTime fechaActual = DateTime.Today;

            query = query.Where(x =>
                x.Estado == EstadoPrestamo.Activo &&
                x.FechaLimiteDevolucion < fechaActual);
        }

        return await query
            .OrderByDescending(x =>
                x.Estado == EstadoPrestamo.Activo)
            .ThenBy(x => x.FechaLimiteDevolucion)
            .ThenByDescending(x => x.FechaPrestamo)
            .ToListAsync();
    }

    public async Task<Prestamo?> ObtenerPorIdAsync(
        int id)
    {
        return await _context.Prestamos
            .Include(x => x.UsuarioBiblioteca)
            .Include(x => x.Ejemplar)
                .ThenInclude(x =>
                    x.MaterialBibliografico)
            .FirstOrDefaultAsync(x =>
                x.IdPrestamo == id);
    }

    public async Task<UsuarioBiblioteca?>
        ObtenerUsuarioPorIdAsync(
            int idUsuarioBiblioteca)
    {
        return await _context.UsuariosBiblioteca
            .FirstOrDefaultAsync(x =>
                x.IdUsuarioBiblioteca ==
                    idUsuarioBiblioteca);
    }

    public async Task<Ejemplar?>
        ObtenerEjemplarPorIdAsync(
            int idEjemplar)
    {
        return await _context.Ejemplares
            .Include(x => x.MaterialBibliografico)
            .FirstOrDefaultAsync(x =>
                x.IdEjemplar == idEjemplar);
    }

    public async Task<List<UsuarioBiblioteca>>
        ObtenerUsuariosActivosAsync()
    {
        return await _context.UsuariosBiblioteca
            .AsNoTracking()
            .Where(x => x.Activo)
            .OrderBy(x => x.NombreCompleto)
            .ToListAsync();
    }

    public async Task<List<Ejemplar>>
        ObtenerEjemplaresDisponiblesAsync()
    {
        return await _context.Ejemplares
            .AsNoTracking()
            .Include(x => x.MaterialBibliografico)
            .Where(x =>
                x.Activo &&
                x.Estado ==
                    EstadoEjemplar.Disponible &&
                x.MaterialBibliografico.Activo)
            .OrderBy(x =>
                x.MaterialBibliografico.Titulo)
            .ThenBy(x => x.CodigoBarras)
            .ToListAsync();
    }

    public async Task<bool>
        ExistePrestamoActivoPorEjemplarAsync(
            int idEjemplar)
    {
        return await _context.Prestamos
            .AnyAsync(x =>
                x.IdEjemplar == idEjemplar &&
                x.Estado ==
                    EstadoPrestamo.Activo);
    }

    public async Task AgregarAsync(
        Prestamo prestamo)
    {
        await _context.Prestamos
            .AddAsync(prestamo);
    }

    public void ActualizarPrestamo(
        Prestamo prestamo)
    {
        _context.Prestamos.Update(prestamo);
    }

    public void ActualizarEjemplar(
        Ejemplar ejemplar)
    {
        _context.Ejemplares.Update(ejemplar);
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<int> ContarActivosAsync()
    {
        return await _context.Prestamos
            .CountAsync(x =>
                x.Estado ==
                    EstadoPrestamo.Activo);
    }

    public async Task<int> ContarAtrasadosAsync()
    {
        DateTime fechaActual = DateTime.Today;

        return await _context.Prestamos
            .CountAsync(x =>
                x.Estado ==
                    EstadoPrestamo.Activo &&
                x.FechaLimiteDevolucion <
                    fechaActual);
    }
}