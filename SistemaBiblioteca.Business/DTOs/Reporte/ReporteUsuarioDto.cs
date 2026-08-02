using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Business.DTOs.Reporte;

public class ReporteUsuarioDto
{
    public int IdUsuarioBiblioteca { get; set; }

    public string Identificacion { get; set; }
        = string.Empty;

    public string NombreCompleto { get; set; }
        = string.Empty;

    public TipoUsuarioBiblioteca TipoUsuario { get; set; }

    public string? SeccionODepartamento { get; set; }

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public bool Activo { get; set; }

    public int TotalPrestamos { get; set; }

    public int PrestamosActivos { get; set; }

    public int PrestamosAtrasados { get; set; }
}