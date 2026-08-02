using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaBiblioteca.Business.DTOs.Reporte;
using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Web.ViewModels.Reporte;

public class ReporteEjemplarViewModel
{
    public ReporteEjemplarFiltroDto Filtro { get; set; }
        = new();

    public List<ReporteEjemplarDto> Ejemplares { get; set; }
        = new();

    public List<SelectListItem> EstadosEjemplar { get; set; }
        = new();

    public List<SelectListItem> EstadosRegistro { get; set; }
        = new();

    public int TotalResultados =>
        Ejemplares.Count;

    public int TotalDisponibles =>
        Ejemplares.Count(x =>
            x.Estado == EstadoEjemplar.Disponible);

    public int TotalPrestados =>
        Ejemplares.Count(x =>
            x.Estado == EstadoEjemplar.Prestado);

    public int TotalMantenimiento =>
        Ejemplares.Count(x =>
            x.Estado == EstadoEjemplar.Mantenimiento);

    public int TotalFueraServicio =>
        Ejemplares.Count(x =>
            x.Estado == EstadoEjemplar.FueraDeServicio);
}