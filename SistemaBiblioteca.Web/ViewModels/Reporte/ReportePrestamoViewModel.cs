using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaBiblioteca.Business.DTOs.Reporte;
using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Web.ViewModels.Reporte;

public class ReportePrestamoViewModel
{
    public ReportePrestamoFiltroDto Filtro { get; set; }
        = new();

    public List<ReportePrestamoDto> Prestamos { get; set; }
        = new();

    public List<SelectListItem> EstadosPrestamo { get; set; }
        = new();

    public int TotalResultados =>
        Prestamos.Count;

    public int PrestamosActivos =>
        Prestamos.Count(x =>
            x.Estado == EstadoPrestamo.Activo);

    public int PrestamosDevueltos =>
        Prestamos.Count(x =>
            x.Estado == EstadoPrestamo.Devuelto);

    public int PrestamosAtrasados =>
        Prestamos.Count(x =>
            x.EstaAtrasado);

    public int TotalDiasAtraso =>
        Prestamos.Sum(x =>
            x.DiasAtraso);
}