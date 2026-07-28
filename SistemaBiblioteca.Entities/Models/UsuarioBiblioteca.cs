using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Entities.Models;

public class UsuarioBiblioteca
{
    public int IdUsuarioBiblioteca { get; set; }

    public string Identificacion { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public TipoUsuarioBiblioteca TipoUsuario { get; set; }

    public string? SeccionODepartamento { get; set; }

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Prestamo> Prestamos { get; set; }
        = new List<Prestamo>();
}