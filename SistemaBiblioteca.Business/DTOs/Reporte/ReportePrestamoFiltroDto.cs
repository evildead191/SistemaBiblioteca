using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Business.DTOs.Reporte;

public class ReportePrestamoFiltroDto
{
    public string? Busqueda { get; set; }

    public EstadoPrestamo? Estado { get; set; }

    public DateTime? FechaInicial { get; set; }

    public DateTime? FechaFinal { get; set; }

    public bool SoloAtrasados { get; set; }
}