using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.DataAccess.Context;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.DataAccess.Repositories;

public class ReporteRepository : IReporteRepository
{
    private readonly SistemaBibliotecaDbContext _context;

    public ReporteRepository(
        SistemaBibliotecaDbContext context)
    {
        _context = context;
    }

    public async Task<List<MaterialBibliografico>>
        ObtenerMaterialesAsync(
            string? busqueda = null,
            bool? activo = null)
    {
        IQueryable<MaterialBibliografico> query =
            _context.MaterialesBibliograficos
                .AsNoTracking()
                .Include(x => x.Ejemplares);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string criterio = busqueda.Trim();

            query = query.Where(x =>
                x.NumeroFicha.Contains(criterio) ||
                x.Clasificacion.Contains(criterio) ||
                x.Autor.Contains(criterio) ||
                x.Titulo.Contains(criterio));
        }

        if (activo.HasValue)
        {
            query = query.Where(x =>
                x.Activo == activo.Value);
        }

        return await query
            .OrderBy(x => x.Titulo)
            .ThenBy(x => x.Autor)
            .ThenBy(x => x.NumeroFicha)
            .ToListAsync();
    }

    public async Task<List<Ejemplar>>
        ObtenerEjemplaresAsync(
            string? busqueda = null,
            EstadoEjemplar? estado = null,
            bool? activo = null)
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
                x.MaterialBibliografico.NumeroFicha
                    .Contains(criterio) ||
                x.MaterialBibliografico.Titulo
                    .Contains(criterio) ||
                x.MaterialBibliografico.Autor
                    .Contains(criterio));
        }

        if (estado.HasValue)
        {
            query = query.Where(x =>
                x.Estado == estado.Value);
        }

        if (activo.HasValue)
        {
            query = query.Where(x =>
                x.Activo == activo.Value);
        }

        return await query
            .OrderBy(x => x.MaterialBibliografico.Titulo)
            .ThenBy(x => x.CodigoBarras)
            .ToListAsync();
    }

    public async Task<List<UsuarioBiblioteca>>
        ObtenerUsuariosAsync(
            string? busqueda = null,
            TipoUsuarioBiblioteca? tipoUsuario = null,
            bool? activo = null)
    {
        IQueryable<UsuarioBiblioteca> query =
            _context.UsuariosBiblioteca
                .AsNoTracking()
                .Include(x => x.Prestamos);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string criterio = busqueda.Trim();

            query = query.Where(x =>
                x.Identificacion.Contains(criterio) ||
                x.NombreCompleto.Contains(criterio) ||
                (x.SeccionODepartamento != null &&
                 x.SeccionODepartamento.Contains(criterio)) ||
                (x.Correo != null &&
                 x.Correo.Contains(criterio)) ||
                (x.Telefono != null &&
                 x.Telefono.Contains(criterio)));
        }

        if (tipoUsuario.HasValue)
        {
            query = query.Where(x =>
                x.TipoUsuario == tipoUsuario.Value);
        }

        if (activo.HasValue)
        {
            query = query.Where(x =>
                x.Activo == activo.Value);
        }

        return await query
            .OrderBy(x => x.NombreCompleto)
            .ThenBy(x => x.Identificacion)
            .ToListAsync();
    }

    public async Task<List<Prestamo>>
        ObtenerPrestamosAsync(
            string? busqueda = null,
            EstadoPrestamo? estado = null,
            DateTime? fechaInicial = null,
            DateTime? fechaFinal = null,
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
                x.Ejemplar.MaterialBibliografico.NumeroFicha
                    .Contains(criterio) ||
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

        if (fechaInicial.HasValue)
        {
            DateTime inicio =
                fechaInicial.Value.Date;

            query = query.Where(x =>
                x.FechaPrestamo >= inicio);
        }

        if (fechaFinal.HasValue)
        {
            DateTime limiteExclusivo =
                fechaFinal.Value.Date.AddDays(1);

            query = query.Where(x =>
                x.FechaPrestamo < limiteExclusivo);
        }

        if (soloAtrasados)
        {
            DateTime hoy = DateTime.Today;

            query = query.Where(x =>
                x.Estado == EstadoPrestamo.Activo &&
                x.FechaLimiteDevolucion < hoy);
        }

        return await query
            .OrderByDescending(x =>
                x.Estado == EstadoPrestamo.Activo)
            .ThenBy(x => x.FechaLimiteDevolucion)
            .ThenByDescending(x => x.FechaPrestamo)
            .ToListAsync();
    }
}