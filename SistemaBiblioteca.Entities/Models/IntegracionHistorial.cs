namespace SistemaBiblioteca.Entities.Models;

public class IntegracionHistorial
{
    public int IdIntegracionHistorial { get; set; }

    public string NombreArchivo { get; set; } = string.Empty;

    public DateTime FechaProceso { get; set; }
        = DateTime.Now;

    public string UsuarioEjecutor { get; set; } = string.Empty;

    public int FilasProcesadas { get; set; }

    public int FilasImportadas { get; set; }

    public int FilasConError { get; set; }

    public string Estado { get; set; } = string.Empty;

    public string? Detalle { get; set; }
}