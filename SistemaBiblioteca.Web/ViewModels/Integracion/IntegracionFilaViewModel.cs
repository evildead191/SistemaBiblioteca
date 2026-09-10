namespace SistemaBiblioteca.Web.ViewModels.Integracion;

public class IntegracionFilaViewModel
{
    public int NumeroFila { get; set; }

    public string NumeroFicha { get; set; } = string.Empty;

    public string Clasificacion { get; set; } = string.Empty;

    public string Autor { get; set; } = string.Empty;

    public string Titulo { get; set; } = string.Empty;

    public int? AnioPublicacion { get; set; }

    public string CodigoBarras { get; set; } = string.Empty;

    public string? NumeroInscripcion { get; set; }

    public string Estado { get; set; } = "Disponible";

    public string? Biblioteca { get; set; }

    public bool EsValida { get; set; }

    public bool YaExiste { get; set; }

    public string? Error { get; set; }
}