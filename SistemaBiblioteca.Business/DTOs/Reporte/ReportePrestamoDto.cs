using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Business.DTOs.Reporte;

public class ReportePrestamoDto
{
    public int IdPrestamo { get; set; }

    public int IdUsuarioBiblioteca { get; set; }

    public string IdentificacionUsuario { get; set; }
        = string.Empty;

    public string NombreUsuario { get; set; }
        = string.Empty;

    public TipoUsuarioBiblioteca TipoUsuario { get; set; }

    public int IdEjemplar { get; set; }

    public string CodigoBarras { get; set; }
        = string.Empty;

    public string? NumeroInscripcion { get; set; }

    public string TituloMaterial { get; set; }
        = string.Empty;

    public string AutorMaterial { get; set; }
        = string.Empty;

    public DateTime FechaPrestamo { get; set; }

    public DateTime FechaLimiteDevolucion { get; set; }

    public DateTime? FechaDevolucionReal { get; set; }

    public EstadoPrestamo Estado { get; set; }

    public string? Observaciones { get; set; }

    public int DiasPrestado { get; set; }

    public bool EstaAtrasado { get; set; }

    public int DiasAtraso { get; set; }
}