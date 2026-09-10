using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.DataAccess.Context;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.DataAccess.Repositories;

public class EjemplarRepository : IEjemplarRepository
{
    private readonly SistemaBibliotecaDbContext _context;

    public EjemplarRepository(
        SistemaBibliotecaDbContext context)
    {
        _context = context;
    }

    public async Task<List<Ejemplar>> ObtenerTodosAsync(
    string? busqueda = null,
    int pagina = 1,
    int tamanoPagina = 25)
    {
        IQueryable<Ejemplar> query =
            _context.Ejemplares
                .AsNoTracking()
                .Include(x => x.MaterialBibliografico);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string criterio = busqueda.Trim();

            query = query.Where(x =>
                x.CodigoBarras.Contains(criterio) ||
                (x.NumeroInscripcion != null &&
                 x.NumeroInscripcion.Contains(criterio)) ||
                (x.Biblioteca != null &&
                 x.Biblioteca.Contains(criterio)) ||
                x.MaterialBibliografico.Titulo.Contains(criterio) ||
                x.MaterialBibliografico.NumeroFicha.Contains(criterio));
        }

        pagina = Math.Max(pagina, 1);
        tamanoPagina = Math.Max(tamanoPagina, 1);

        return await query
            .OrderBy(x => x.MaterialBibliografico.Titulo)
            .ThenBy(x => x.CodigoBarras)
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync();
    }

    public async Task<int> ContarFiltradosAsync(
    string? busqueda = null)
    {
        IQueryable<Ejemplar> query =
            _context.Ejemplares
                .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string criterio = busqueda.Trim();

            query = query.Where(x =>
                x.CodigoBarras.Contains(criterio) ||
                (x.NumeroInscripcion != null &&
                 x.NumeroInscripcion.Contains(criterio)) ||
                (x.Biblioteca != null &&
                 x.Biblioteca.Contains(criterio)) ||
                x.MaterialBibliografico.Titulo.Contains(criterio) ||
                x.MaterialBibliografico.NumeroFicha.Contains(criterio));
        }

        return await query.CountAsync();
    }

    public async Task<Ejemplar?> ObtenerPorIdAsync(int id)
    {
        return await _context.Ejemplares
            .FirstOrDefaultAsync(x =>
                x.IdEjemplar == id);
    }

    public async Task<bool> ExisteCodigoBarrasAsync(
        string codigoBarras,
        int? idExcluir = null)
    {
        return await _context.Ejemplares
            .AnyAsync(x =>
                x.CodigoBarras == codigoBarras &&
                (!idExcluir.HasValue ||
                 x.IdEjemplar != idExcluir.Value));
    }

    public async Task<bool> ExisteMaterialBibliograficoAsync(
        int idMaterialBibliografico)
    {
        return await _context.MaterialesBibliograficos
            .AnyAsync(x =>
                x.IdMaterialBibliografico ==
                    idMaterialBibliografico &&
                x.Activo);
    }

    public async Task AgregarAsync(Ejemplar ejemplar)
    {
        await _context.Ejemplares.AddAsync(ejemplar);
    }

    public async Task<int> ContarAsync()
    {
        return await _context.Ejemplares
            .CountAsync();
    }

    public async Task<int> ContarPorEstadoAsync(
        EstadoEjemplar estado)
    {
        return await _context.Ejemplares
            .CountAsync(x =>
                x.Activo &&
                x.Estado == estado);
    }

    public void Actualizar(Ejemplar ejemplar)
    {
        _context.Ejemplares.Update(ejemplar);
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<string>>
    ObtenerCodigosBarrasAsync()
    {
        return await _context.Ejemplares
            .AsNoTracking()
            .Select(x => x.CodigoBarras)
            .OrderBy(x => x)
            .ToListAsync();
    }

    public async Task AgregarRangoAsync(
        IEnumerable<Ejemplar> ejemplares)
    {
        await _context.Ejemplares
            .AddRangeAsync(ejemplares);
    }
}