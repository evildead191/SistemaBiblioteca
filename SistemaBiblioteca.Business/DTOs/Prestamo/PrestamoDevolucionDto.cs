using System.ComponentModel.DataAnnotations;

namespace SistemaBiblioteca.Business.DTOs.Prestamo;

public class PrestamoDevolucionDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int IdPrestamo { get; set; }

    [Required(ErrorMessage = "La fecha de devolución es obligatoria.")]
    [DataType(DataType.Date)]
    public DateTime FechaDevolucionReal { get; set; }
        = DateTime.Today;

    [StringLength(
        500,
        ErrorMessage = "Las observaciones no pueden superar los 500 caracteres."
    )]
    public string? Observaciones { get; set; }
}