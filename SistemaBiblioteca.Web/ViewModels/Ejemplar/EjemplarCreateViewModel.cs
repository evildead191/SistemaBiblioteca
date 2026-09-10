using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaBiblioteca.Business.DTOs.Ejemplar;

namespace SistemaBiblioteca.Web.ViewModels.Ejemplar;

public class EjemplarCreateViewModel
{
    public EjemplarCreateDto Ejemplar { get; set; }
        = new();

    public List<SelectListItem> MaterialesBibliograficos { get; set; }
        = [];

    public List<SelectListItem> Estados { get; set; }
        = [];

    public string? UltimoCodigoRegistrado { get; set; }

    public string? SiguienteCodigoSugerido { get; set; }
}