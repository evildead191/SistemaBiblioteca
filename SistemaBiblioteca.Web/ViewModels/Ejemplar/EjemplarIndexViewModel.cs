using SistemaBiblioteca.Business.DTOs.Ejemplar;

namespace SistemaBiblioteca.Web.ViewModels.Ejemplar;

public class EjemplarIndexViewModel
{
    public string? Busqueda { get; set; }

    public List<EjemplarDto> Ejemplares { get; set; } = [];

    public int PaginaActual { get; set; } = 1;

    public int TamanoPagina { get; set; } = 25;

    public int TotalRegistros { get; set; }

    public int TotalPaginas =>
        TotalRegistros == 0
            ? 0
            : (int)Math.Ceiling(
                TotalRegistros / (double)TamanoPagina);
}