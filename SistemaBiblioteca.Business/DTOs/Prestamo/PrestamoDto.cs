using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Business.DTOs.Prestamo;

public class PrestamoDto
{
    public int IdPrestamo { get; set; }

    public int IdUsuarioBiblioteca { get; set; }

    public string IdentificacionUsuario { get; set; }
        = string.Empty;

    public string NombreUsuario { get; set; }
        = string.Empty;

    public string TipoUsuario { get; set; }
        = string.Empty;

    public int IdEjemplar { get; set; }

    public string CodigoEjemplar { get; set; }
        = string.Empty;

    public string TituloMaterial { get; set; }
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