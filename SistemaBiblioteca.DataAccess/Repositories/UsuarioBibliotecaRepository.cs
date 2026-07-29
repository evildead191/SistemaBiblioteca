using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.DataAccess.Context;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.DataAccess.Repositories;

public class UsuarioBibliotecaRepository
    : IUsuarioBibliotecaRepository
{
    private readonly SistemaBibliotecaDbContext _context;

    public UsuarioBibliotecaRepository(
        SistemaBibliotecaDbContext context)
    {
        _context = context;
    }

    public async Task<List<UsuarioBiblioteca>> ObtenerTodosAsync(
        string? busqueda = null,
        TipoUsuarioBiblioteca? tipoUsuario = null)
    {
        IQueryable<UsuarioBiblioteca> consulta =
            _context.UsuariosBiblioteca
                .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string textoBusqueda = busqueda.Trim();

            consulta = consulta.Where(x =>
                x.Identificacion.Contains(textoBusqueda) ||
                x.NombreCompleto.Contains(textoBusqueda) ||
                (x.SeccionODepartamento != null &&
                 x.SeccionODepartamento.Contains(textoBusqueda)) ||
                (x.Correo != null &&
                 x.Correo.Contains(textoBusqueda)) ||
                (x.Telefono != null &&
                 x.Telefono.Contains(textoBusqueda)));
        }

        if (tipoUsuario.HasValue)
        {
            consulta = consulta.Where(x =>
                x.TipoUsuario == tipoUsuario.Value);
        }

        return await consulta
            .OrderBy(x => x.NombreCompleto)
            .ThenBy(x => x.Identificacion)
            .ToListAsync();
    }

    public async Task<UsuarioBiblioteca?> ObtenerPorIdAsync(
        int idUsuarioBiblioteca)
    {
        return await _context.UsuariosBiblioteca
            .FirstOrDefaultAsync(x =>
                x.IdUsuarioBiblioteca == idUsuarioBiblioteca);
    }

    public async Task<bool> ExisteIdentificacionAsync(
        string identificacion,
        int? idExcluir = null)
    {
        string identificacionNormalizada =
            identificacion.Trim();

        IQueryable<UsuarioBiblioteca> consulta =
            _context.UsuariosBiblioteca;

        if (idExcluir.HasValue)
        {
            consulta = consulta.Where(x =>
                x.IdUsuarioBiblioteca != idExcluir.Value);
        }

        return await consulta.AnyAsync(x =>
            x.Identificacion == identificacionNormalizada);
    }

    public async Task AgregarAsync(
        UsuarioBiblioteca usuarioBiblioteca)
    {
        await _context.UsuariosBiblioteca
            .AddAsync(usuarioBiblioteca);
    }

    public void Actualizar(
        UsuarioBiblioteca usuarioBiblioteca)
    {
        _context.UsuariosBiblioteca
            .Update(usuarioBiblioteca);
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<int> ContarAsync()
    {
        return await _context.UsuariosBiblioteca
            .CountAsync();
    }

    public async Task<int> ContarActivosAsync()
    {
        return await _context.UsuariosBiblioteca
            .CountAsync(x => x.Activo);
    }
}