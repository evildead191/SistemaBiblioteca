using SistemaBiblioteca.Business.DTOs.Prestamo;

namespace SistemaBiblioteca.Web.ViewModels.Prestamo;

public class PrestamoDevolucionViewModel
{
    public PrestamoDevolucionDto Prestamo { get; set; }
        = new();

    public string NombreUsuario { get; set; }
        = string.Empty;

    public string IdentificacionUsuario { get; set; }
        = string.Empty;

    public string TituloMaterial { get; set; }
        = string.Empty;

    public string CodigoEjemplar { get; set; }
        = string.Empty;

    public DateTime FechaPrestamo { get; set; }

    public DateTime FechaLimiteDevolucion { get; set; }

    public bool EstaAtrasado { get; set; }

    public int DiasAtraso { get; set; }
}