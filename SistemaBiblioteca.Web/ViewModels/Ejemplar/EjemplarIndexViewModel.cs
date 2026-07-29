using SistemaBiblioteca.Business.DTOs.Ejemplar;

namespace SistemaBiblioteca.Web.ViewModels.Ejemplar;

public class EjemplarIndexViewModel
{
    public string? Busqueda { get; set; }

    public List<EjemplarDto> Ejemplares { get; set; } = [];
}