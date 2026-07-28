namespace SistemaBiblioteca.Entities.Models;

public class MaterialBibliograficoListadoResult
{
    public MaterialBibliografico Material { get; set; } = new();

    public int CantidadEjemplares { get; set; }
}