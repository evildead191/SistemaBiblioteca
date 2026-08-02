namespace SistemaBiblioteca.Web.ViewModels.Reporte;

public class ReporteIndexViewModel
{
    public int TotalMateriales { get; set; }

    public int TotalEjemplares { get; set; }

    public int TotalUsuarios { get; set; }

    public int TotalPrestamos { get; set; }

    public int PrestamosActivos { get; set; }

    public int PrestamosAtrasados { get; set; }
}