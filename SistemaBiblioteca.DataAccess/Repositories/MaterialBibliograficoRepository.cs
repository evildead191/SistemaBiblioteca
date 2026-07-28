using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.DataAccess.Context;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.DataAccess.Repositories;

public class MaterialBibliograficoRepository
    : IMaterialBibliograficoRepository
{
    private readonly SistemaBibliotecaDbContext _context;

    public MaterialBibliograficoRepository(
        SistemaBibliotecaDbContext context)
    {
        _context = context;
    }

    public async Task<List<MaterialBibliograficoListadoResult>> ObtenerTodosAsync(
    string? busqueda = null)
    {
        IQueryable<MaterialBibliografico> query =
            _context.MaterialesBibliograficos
                .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string criterio = busqueda.Trim();

            query = query.Where(x =>
                x.NumeroFicha.Contains(criterio) ||
                x.Clasificacion.Contains(criterio) ||
                x.Autor.Contains(criterio) ||
                x.Titulo.Contains(criterio));
        }

        return await query
            .OrderBy(x => x.Titulo)
            .Select(x => new MaterialBibliograficoListadoResult
            {
                Material = new MaterialBibliografico
                {
                    IdMaterialBibliografico = x.IdMaterialBibliografico,
                    NumeroFicha = x.NumeroFicha,
                    Clasificacion = x.Clasificacion,
                    Autor = x.Autor,
                    Titulo = x.Titulo,
                    AnioPublicacion = x.AnioPublicacion,
                    Activo = x.Activo
                },

                CantidadEjemplares = x.Ejemplares.Count()
            })
            .ToListAsync();
    }

    public async Task<MaterialBibliografico?> ObtenerPorIdAsync(int id)
    {
        return await _context.MaterialesBibliograficos
            .FirstOrDefaultAsync(x =>
                x.IdMaterialBibliografico == id);
    }

    public async Task<bool> ExisteNumeroFichaAsync(
        string numeroFicha,
        int? idExcluir = null)
    {
        return await _context.MaterialesBibliograficos
            .AnyAsync(x =>
                x.NumeroFicha == numeroFicha &&
                (!idExcluir.HasValue ||
                 x.IdMaterialBibliografico != idExcluir.Value));
    }

    public async Task AgregarAsync(MaterialBibliografico material)
    {
        await _context.MaterialesBibliograficos.AddAsync(material);
    }

    public void Actualizar(MaterialBibliografico material)
    {
        _context.MaterialesBibliograficos.Update(material);
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}