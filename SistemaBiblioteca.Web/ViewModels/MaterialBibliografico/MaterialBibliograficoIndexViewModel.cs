using SistemaBiblioteca.Business.DTOs.MaterialBibliografico;

namespace SistemaBiblioteca.Web.ViewModels.MaterialBibliografico;

public class MaterialBibliograficoIndexViewModel
{
    public string? Busqueda { get; set; }

    public List<MaterialBibliograficoDto> Materiales { get; set; }
        = new();
}