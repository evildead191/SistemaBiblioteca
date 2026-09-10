namespace SistemaBiblioteca.Web.ViewModels.Home;

public class DashboardViewModel
{
    public int TotalMateriales { get; set; }

    public int TotalEjemplares { get; set; }

    public int EjemplaresDisponibles { get; set; }

    public int EjemplaresPrestados { get; set; }

    public int UsuariosRegistrados { get; set; }

    public int UsuariosActivos { get; set; }

    public int PrestamosActivos { get; set; }

    public int PrestamosAtrasados { get; set; }
}