using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Business.DTOs.Reporte;

public class ReporteEjemplarDto
{
    public int IdEjemplar { get; set; }

    public string CodigoBarras { get; set; }
        = string.Empty;

    public string? NumeroInscripcion { get; set; }

    public int IdMaterialBibliografico { get; set; }

    public string NumeroFicha { get; set; }
        = string.Empty;

    public string TituloMaterial { get; set; }
        = string.Empty;

    public string AutorMaterial { get; set; }
        = string.Empty;

    public EstadoEjemplar Estado { get; set; }

    public string? Biblioteca { get; set; }

    public bool Activo { get; set; }
}