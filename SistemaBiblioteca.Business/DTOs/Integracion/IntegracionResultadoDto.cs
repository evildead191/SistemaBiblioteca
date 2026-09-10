namespace SistemaBiblioteca.Business.DTOs.Integracion;

public class IntegracionResultadoDto
{
    public bool Exitoso { get; set; }

    public int FilasProcesadas { get; set; }

    public int FilasImportadas { get; set; }

    public int FilasConError { get; set; }

    public int FilasOmitidas { get; set; }

    public int MaterialesCreados { get; set; }

    public int EjemplaresCreados { get; set; }

    public string Mensaje { get; set; } = string.Empty;
}