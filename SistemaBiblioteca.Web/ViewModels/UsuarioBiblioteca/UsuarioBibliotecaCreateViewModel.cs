using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaBiblioteca.Business.DTOs.UsuarioBiblioteca;

namespace SistemaBiblioteca.Web.ViewModels.UsuarioBiblioteca;

public class UsuarioBibliotecaCreateViewModel
{
    public UsuarioBibliotecaCreateDto Usuario { get; set; }
        = new();

    public List<SelectListItem> TiposUsuario { get; set; }
        = new();
}