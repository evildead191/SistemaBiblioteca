using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Business.DTOs.Reporte;

public class ReporteUsuarioFiltroDto
{
    public string? Busqueda { get; set; }

    public TipoUsuarioBiblioteca? TipoUsuario { get; set; }

    public bool? Activo { get; set; }
}