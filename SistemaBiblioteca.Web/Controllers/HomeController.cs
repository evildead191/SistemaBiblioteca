using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Web.ViewModels.Home;

namespace SistemaBiblioteca.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class HomeController : Controller
{
    private readonly IMaterialBibliograficoService
        _materialBibliograficoService;

    private readonly IEjemplarService
        _ejemplarService;

    private readonly IUsuarioBibliotecaService
        _usuarioBibliotecaService;

    private readonly IPrestamoService
        _prestamoService;

    public HomeController(
    IMaterialBibliograficoService materialBibliograficoService,
    IEjemplarService ejemplarService,
    IUsuarioBibliotecaService usuarioBibliotecaService,
    IPrestamoService prestamoService)
    {
        _materialBibliograficoService =
            materialBibliograficoService;

        _ejemplarService =
            ejemplarService;

        _usuarioBibliotecaService =
            usuarioBibliotecaService;

        _prestamoService =
            prestamoService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int totalMateriales =
            await _materialBibliograficoService
                .ContarAsync();

        int totalEjemplares =
            await _ejemplarService
                .ContarAsync();

        int ejemplaresDisponibles =
            await _ejemplarService
                .ContarPorEstadoAsync(
                    EstadoEjemplar.Disponible);

        int ejemplaresPrestados =
            await _ejemplarService
                .ContarPorEstadoAsync(
                    EstadoEjemplar.Prestado);

        int usuariosRegistrados =
            await _usuarioBibliotecaService
                .ContarUsuariosAsync();

        int usuariosActivos =
            await _usuarioBibliotecaService
                .ContarUsuariosActivosAsync();

        int prestamosActivos =
            await _prestamoService
                .ContarPrestamosActivosAsync();

        int prestamosAtrasados =
            await _prestamoService
                .ContarPrestamosAtrasadosAsync();

        DashboardViewModel viewModel = new()
        {
            TotalMateriales =
                totalMateriales,

            TotalEjemplares =
                totalEjemplares,

            EjemplaresDisponibles =
                ejemplaresDisponibles,

            EjemplaresPrestados =
                ejemplaresPrestados,

            UsuariosRegistrados =
                usuariosRegistrados,

            UsuariosActivos =
                usuariosActivos,

            PrestamosActivos =
                prestamosActivos,

            PrestamosAtrasados =
                prestamosAtrasados
        };

        return View(viewModel);
    }

    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}