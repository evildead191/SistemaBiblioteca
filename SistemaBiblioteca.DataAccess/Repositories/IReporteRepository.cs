using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.DataAccess.Repositories;

public interface IReporteRepository
{
    Task<List<MaterialBibliografico>>
        ObtenerMaterialesAsync(
            string? busqueda = null,
            bool? activo = null);

    Task<List<Ejemplar>>
        ObtenerEjemplaresAsync(
            string? busqueda = null,
            EstadoEjemplar? estado = null,
            bool? activo = null);

    Task<List<UsuarioBiblioteca>>
        ObtenerUsuariosAsync(
            string? busqueda = null,
            TipoUsuarioBiblioteca? tipoUsuario = null,
            bool? activo = null);

    Task<List<Prestamo>>
        ObtenerPrestamosAsync(
            string? busqueda = null,
            EstadoPrestamo? estado = null,
            DateTime? fechaInicial = null,
            DateTime? fechaFinal = null,
            bool soloAtrasados = false);
}