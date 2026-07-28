namespace SistemaBiblioteca.Entities.Models;

public class MaterialBibliografico
{
    public int IdMaterialBibliografico { get; set; }

    public string NumeroFicha { get; set; } = string.Empty;

    public string Clasificacion { get; set; } = string.Empty;

    public string Autor { get; set; } = string.Empty;

    public string Titulo { get; set; } = string.Empty;

    public int? AnioPublicacion { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Ejemplar> Ejemplares { get; set; }
        = new List<Ejemplar>();
}