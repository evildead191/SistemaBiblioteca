namespace SistemaBiblioteca.Entities.Models;

public class Auditoria
{
    public int IdAuditoria { get; set; }

    public string UsuarioSistema { get; set; } = string.Empty;

    public string Accion { get; set; } = string.Empty;

    public string Modulo { get; set; } = string.Empty;

    public string? Detalle { get; set; }

    public DateTime Fecha { get; set; }
        = DateTime.Now;
}