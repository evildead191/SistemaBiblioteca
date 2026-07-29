using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaBiblioteca.Business.DTOs.Ejemplar;

namespace SistemaBiblioteca.Web.ViewModels.Ejemplar;

public class EjemplarEditViewModel
{
    public EjemplarEditDto Ejemplar { get; set; } = new();

    public List<SelectListItem> MaterialesBibliograficos { get; set; } = [];

    public List<SelectListItem> Estados { get; set; } = [];
}