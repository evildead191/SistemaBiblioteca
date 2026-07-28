using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Business.DTOs.Ejemplar;

public class EjemplarDto
{
    public int IdEjemplar { get; set; }

    public string CodigoBarras { get; set; } = string.Empty;

    public string? NumeroInscripcion { get; set; }

    public int IdMaterialBibliografico { get; set; }

    public string MaterialBibliografico { get; set; }
        = string.Empty;

    public EstadoEjemplar Estado { get; set; }

    public string? Biblioteca { get; set; }

    public bool Activo { get; set; }
}