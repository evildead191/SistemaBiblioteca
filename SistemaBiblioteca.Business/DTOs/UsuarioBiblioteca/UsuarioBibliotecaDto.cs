using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Business.DTOs.UsuarioBiblioteca;

public class UsuarioBibliotecaDto
{
    public int IdUsuarioBiblioteca { get; set; }

    public string Identificacion { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public TipoUsuarioBiblioteca TipoUsuario { get; set; }

    public string TipoUsuarioTexto =>
        TipoUsuario.ToString();

    public string? SeccionODepartamento { get; set; }

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public bool Activo { get; set; }
}