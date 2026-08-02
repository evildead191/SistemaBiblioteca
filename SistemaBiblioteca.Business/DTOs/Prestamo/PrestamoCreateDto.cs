using System.ComponentModel.DataAnnotations;

namespace SistemaBiblioteca.Business.DTOs.Prestamo;

public class PrestamoCreateDto
{
    [Required(ErrorMessage = "Debe seleccionar un usuario.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un usuario válido.")]
    public int IdUsuarioBiblioteca { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un ejemplar.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un ejemplar válido.")]
    public int IdEjemplar { get; set; }

    [Required(ErrorMessage = "La fecha del préstamo es obligatoria.")]
    [DataType(DataType.Date)]
    public DateTime FechaPrestamo { get; set; }
        = DateTime.Today;

    [Required(ErrorMessage = "La fecha límite de devolución es obligatoria.")]
    [DataType(DataType.Date)]
    public DateTime FechaLimiteDevolucion { get; set; }
        = DateTime.Today.AddDays(15);

    [StringLength(
        500,
        ErrorMessage = "Las observaciones no pueden superar los 500 caracteres."
    )]
    public string? Observaciones { get; set; }
}