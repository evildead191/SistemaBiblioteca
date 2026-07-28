using System.ComponentModel.DataAnnotations;

namespace SistemaBiblioteca.Business.DTOs.MaterialBibliografico;

public class MaterialBibliograficoEditDto
{
    public int IdMaterialBibliografico { get; set; }

    [Required(ErrorMessage = "El número de ficha es obligatorio.")]
    [StringLength(50)]
    public string NumeroFicha { get; set; } = string.Empty;

    [Required(ErrorMessage = "La clasificación es obligatoria.")]
    [StringLength(100)]
    public string Clasificacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El autor es obligatorio.")]
    [StringLength(250)]
    public string Autor { get; set; } = string.Empty;

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(500)]
    public string Titulo { get; set; } = string.Empty;

    [Range(1000, 9999, ErrorMessage = "El año de publicación no es válido.")]
    public int? AnioPublicacion { get; set; }

    public bool Activo { get; set; }
}