using SistemaBiblioteca.Business.DTOs.Reporte;

namespace SistemaBiblioteca.Business.Interfaces;

public interface IReporteService
{
    Task<List<ReporteMaterialDto>>
        ObtenerMaterialesAsync(
            ReporteMaterialFiltroDto filtro);

    Task<List<ReporteEjemplarDto>>
        ObtenerEjemplaresAsync(
            ReporteEjemplarFiltroDto filtro);

    Task<List<ReporteUsuarioDto>>
        ObtenerUsuariosAsync(
            ReporteUsuarioFiltroDto filtro);

    Task<List<ReportePrestamoDto>>
        ObtenerPrestamosAsync(
            ReportePrestamoFiltroDto filtro);
}