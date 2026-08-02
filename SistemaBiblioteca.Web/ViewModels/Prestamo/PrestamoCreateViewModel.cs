using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaBiblioteca.Business.DTOs.Prestamo;

namespace SistemaBiblioteca.Web.ViewModels.Prestamo;

public class PrestamoCreateViewModel
{
    public PrestamoCreateDto Prestamo { get; set; }
        = new();

    public List<SelectListItem> Usuarios { get; set; }
        = new();

    public List<SelectListItem> EjemplaresDisponibles { get; set; }
        = new();
}