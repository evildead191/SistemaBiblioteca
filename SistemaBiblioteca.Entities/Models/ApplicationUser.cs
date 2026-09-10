using Microsoft.AspNetCore.Identity;

namespace SistemaBiblioteca.Entities.Models;

public class ApplicationUser : IdentityUser
{
    public string Nombre { get; set; } = string.Empty;

    public string PrimerApellido { get; set; } = string.Empty;

    public string? SegundoApellido { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;
}