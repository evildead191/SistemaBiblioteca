using System.ComponentModel.DataAnnotations;

namespace SistemaBiblioteca.Web.ViewModels.Integracion;

public class IntegracionEditarFilaViewModel
{
    public int NumeroFila { get; set; }

    [Required(ErrorMessage = "La ficha es obligatoria.")]
    [Display(Name = "Número de ficha")]
    public string NumeroFicha { get; set; } = string.Empty;

    [Required(ErrorMessage = "La clasificación es obligatoria.")]
    [Display(Name = "Clasificación")]
    public string Clasificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El autor es obligatorio.")]
    public string Autor { get; set; } = string.Empty;

    [Required(ErrorMessage = "El título es obligatorio.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Display(Name = "Año de publicación")]
    [Range(
        1000,
        9999,
        ErrorMessage = "Ingrese un año válido.")]
    public int? AnioPublicacion { get; set; }

    [Required(ErrorMessage = "El código de barras es obligatorio.")]
    [Display(Name = "Código de barras")]
    public string CodigoBarras { get; set; } = string.Empty;

    [Display(Name = "Número de inscripción")]
    public string? NumeroInscripcion { get; set; }

    public string Estado { get; set; } = "Disponible";

    public string? Biblioteca { get; set; }
}