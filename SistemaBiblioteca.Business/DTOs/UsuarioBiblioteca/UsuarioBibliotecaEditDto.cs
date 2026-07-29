using System.ComponentModel.DataAnnotations;
using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Business.DTOs.UsuarioBiblioteca;

public class UsuarioBibliotecaEditDto
{
    public int IdUsuarioBiblioteca { get; set; }

    [Required(ErrorMessage = "La identificación es obligatoria.")]
    [StringLength(
        100,
        ErrorMessage = "La identificación no puede superar los 100 caracteres.")]
    [Display(Name = "Identificación")]
    public string Identificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [StringLength(
        250,
        ErrorMessage = "El nombre completo no puede superar los 250 caracteres.")]
    [Display(Name = "Nombre completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar un tipo de usuario.")]
    [Display(Name = "Tipo de usuario")]
    public TipoUsuarioBiblioteca TipoUsuario { get; set; }

    [StringLength(
        150,
        ErrorMessage = "La sección o departamento no puede superar los 150 caracteres.")]
    [Display(Name = "Sección o departamento")]
    public string? SeccionODepartamento { get; set; }

    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    [StringLength(
        250,
        ErrorMessage = "El correo no puede superar los 250 caracteres.")]
    [Display(Name = "Correo electrónico")]
    public string? Correo { get; set; }

    [Phone(ErrorMessage = "El número de teléfono no tiene un formato válido.")]
    [StringLength(
        50,
        ErrorMessage = "El teléfono no puede superar los 50 caracteres.")]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    public bool Activo { get; set; }
}