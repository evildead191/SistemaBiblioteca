using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaBiblioteca.Business.DTOs.UsuarioBiblioteca;

namespace SistemaBiblioteca.Web.ViewModels.UsuarioBiblioteca;

public class UsuarioBibliotecaEditViewModel
{
    public UsuarioBibliotecaEditDto Usuario { get; set; }
        = new();

    public List<SelectListItem> TiposUsuario { get; set; }
        = new();
}