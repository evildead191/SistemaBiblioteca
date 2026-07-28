namespace SistemaBiblioteca.Business.DTOs.MaterialBibliografico;

public class MaterialBibliograficoDto
{
    public int IdMaterialBibliografico { get; set; }

    public string NumeroFicha { get; set; } = string.Empty;

    public string Clasificacion { get; set; } = string.Empty;

    public string Autor { get; set; } = string.Empty;

    public string Titulo { get; set; } = string.Empty;

    public int? AnioPublicacion { get; set; }

    public bool Activo { get; set; }

    public int CantidadEjemplares { get; set; }
}