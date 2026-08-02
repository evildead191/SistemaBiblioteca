using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaBiblioteca.Business.DTOs.Prestamo;
using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Web.ViewModels.Prestamo;

public class PrestamoIndexViewModel
{
    public List<PrestamoDto> Prestamos { get; set; }
        = new();

    public string? Busqueda { get; set; }

    public EstadoPrestamo? Estado { get; set; }

    public bool SoloAtrasados { get; set; }

    public List<SelectListItem> Estados { get; set; }
        = new();
}