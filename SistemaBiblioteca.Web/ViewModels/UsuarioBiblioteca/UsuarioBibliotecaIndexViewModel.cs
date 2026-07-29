using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaBiblioteca.Business.DTOs.UsuarioBiblioteca;
using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Web.ViewModels.UsuarioBiblioteca;

public class UsuarioBibliotecaIndexViewModel
{
    public List<UsuarioBibliotecaDto> Usuarios { get; set; }
        = new();

    public string? Busqueda { get; set; }

    public TipoUsuarioBiblioteca? TipoUsuario { get; set; }

    public List<SelectListItem> TiposUsuario { get; set; }
        = new();
}