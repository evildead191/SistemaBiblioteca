using System.ComponentModel.DataAnnotations;
using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Business.DTOs.Ejemplar;

public class EjemplarCreateDto
{
    [Required(ErrorMessage = "Ingrese el código base.")]
    [StringLength(
        100,
        ErrorMessage = "El código base no puede superar los 100 caracteres.")]
    public string CodigoBarras { get; set; }
        = string.Empty;

    [StringLength(
        100,
        ErrorMessage = "El número de inscripción no puede superar los 100 caracteres.")]
    public string? NumeroInscripcion { get; set; }

    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "Seleccione un material bibliográfico.")]
    public int IdMaterialBibliografico { get; set; }

    [Required(ErrorMessage = "Seleccione el estado del ejemplar.")]
    public EstadoEjemplar Estado { get; set; }
        = EstadoEjemplar.Disponible;

    [Range(
        1,
        100,
        ErrorMessage = "La cantidad debe estar entre 1 y 100 ejemplares.")]
    public int Cantidad { get; set; } = 1;

    public bool GenerarCodigosAutomaticamente { get; set; }
        = true;
}