using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaBiblioteca.Business.DTOs.Reporte;

namespace SistemaBiblioteca.Web.ViewModels.Reporte;

public class ReporteUsuarioViewModel
{
    public ReporteUsuarioFiltroDto Filtro { get; set; }
        = new();

    public List<ReporteUsuarioDto> Usuarios { get; set; }
        = new();

    public List<SelectListItem> TiposUsuario { get; set; }
        = new();

    public List<SelectListItem> EstadosRegistro { get; set; }
        = new();

    public int TotalResultados =>
        Usuarios.Count;

    public int UsuariosActivos =>
        Usuarios.Count(x => x.Activo);

    public int UsuariosInactivos =>
        Usuarios.Count(x => !x.Activo);

    public int TotalPrestamos =>
        Usuarios.Sum(x => x.TotalPrestamos);

    public int PrestamosActivos =>
        Usuarios.Sum(x => x.PrestamosActivos);

    public int PrestamosAtrasados =>
        Usuarios.Sum(x => x.PrestamosAtrasados);
}