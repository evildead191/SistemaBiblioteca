using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.Web.ViewModels.Integracion;

public class IntegracionIndexViewModel
{
    public List<IntegracionHistorial> Historial { get; set; } = new();
}