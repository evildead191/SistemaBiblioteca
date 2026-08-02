using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaBiblioteca.Business.DTOs.Reporte;

namespace SistemaBiblioteca.Web.ViewModels.Reporte;

public class ReporteMaterialViewModel
{
    public ReporteMaterialFiltroDto Filtro { get; set; }
        = new();

    public List<ReporteMaterialDto> Materiales { get; set; }
        = new();

    public List<SelectListItem> EstadosRegistro { get; set; }
        = new();

    public int TotalResultados =>
        Materiales.Count;

    public int TotalEjemplares =>
        Materiales.Sum(x => x.TotalEjemplares);

    public int EjemplaresDisponibles =>
        Materiales.Sum(x => x.EjemplaresDisponibles);

    public int EjemplaresPrestados =>
        Materiales.Sum(x => x.EjemplaresPrestados);
}