using SistemaBiblioteca.Business.DTOs.Reporte;

namespace SistemaBiblioteca.Business.Interfaces;

public interface IReporteExcelService
{
    byte[] GenerarMateriales(
        IReadOnlyCollection<ReporteMaterialDto> materiales);

    byte[] GenerarEjemplares(
        IReadOnlyCollection<ReporteEjemplarDto> ejemplares);

    byte[] GenerarUsuarios(
        IReadOnlyCollection<ReporteUsuarioDto> usuarios);

    byte[] GenerarPrestamos(
        IReadOnlyCollection<ReportePrestamoDto> prestamos);
}