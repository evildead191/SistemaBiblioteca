namespace SistemaBiblioteca.Web.ViewModels.Integracion;

public class IntegracionPreviewViewModel
{
    public string NombreArchivo { get; set; } = string.Empty;

    public List<IntegracionFilaViewModel> Filas { get; set; } = new();

    public string? Busqueda { get; set; }

    public string FiltroEstado { get; set; } = "Todos";

    public int PaginaActual { get; set; } = 1;

    public int TamanoPagina { get; set; } = 25;

    public int TotalFilas => Filas.Count;

    public int FilasValidas =>
        Filas.Count(x =>
            x.EsValida &&
            !x.YaExiste);

    public int FilasConError =>
        Filas.Count(x =>
            !x.EsValida);

    public int FilasExistentes =>
        Filas.Count(x =>
            x.YaExiste);

    public int TotalPaginas { get; set; }

    public int TotalFiltrado { get; set; }

    public List<IntegracionFilaViewModel> FilasPagina { get; set; } = new();
}