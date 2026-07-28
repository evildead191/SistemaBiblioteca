using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Entities.Models;

public class Ejemplar
{
    public int IdEjemplar { get; set; }

    public string CodigoBarras { get; set; } = string.Empty;

    public string? NumeroInscripcion { get; set; }

    public int IdMaterialBibliografico { get; set; }

    public EstadoEjemplar Estado { get; set; }
        = EstadoEjemplar.Disponible;

    public string? Biblioteca { get; set; }

    public bool Activo { get; set; } = true;

    public MaterialBibliografico MaterialBibliografico { get; set; }
        = null!;

    public ICollection<Prestamo> Prestamos { get; set; }
        = new List<Prestamo>();
}