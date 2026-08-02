using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Business.DTOs.Reporte;

public class ReporteEjemplarFiltroDto
{
    public string? Busqueda { get; set; }

    public EstadoEjemplar? Estado { get; set; }

    public bool? Activo { get; set; }
}