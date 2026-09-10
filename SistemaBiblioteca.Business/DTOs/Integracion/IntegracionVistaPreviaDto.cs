namespace SistemaBiblioteca.Business.DTOs.Integracion;

public class IntegracionVistaPreviaDto
{
    public string NombreArchivo { get; set; } = string.Empty;

    public List<IntegracionFilaDto> Filas { get; set; } = new();

    public int TotalFilas => Filas.Count;

    public int FilasValidas =>
        Filas.Count(fila => fila.EsValida && !fila.YaExiste);

    public int FilasConError =>
        Filas.Count(fila => !fila.EsValida);

    public int FilasExistentes =>
        Filas.Count(fila => fila.YaExiste);
}