namespace SistemaBiblioteca.Web.ViewModels.Home;

public class DashboardViewModel
{
    public int TotalMateriales { get; set; }

    public int TotalEjemplares { get; set; }

    public int EjemplaresDisponibles { get; set; }

    public int EjemplaresPrestados { get; set; }

    // Se conectarán cuando se implementen estos módulos.
    public int PrestamosActivos { get; set; }

    public int PrestamosVencidos { get; set; }

    public int PersonasRegistradas { get; set; }

    public int DevolucionesPendientes { get; set; }
}