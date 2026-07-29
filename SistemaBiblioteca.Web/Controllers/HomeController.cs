using Microsoft.AspNetCore.Mvc;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Web.ViewModels.Home;

namespace SistemaBiblioteca.Web.Controllers;

public class HomeController : Controller
{
    private readonly IMaterialBibliograficoService
        _materialBibliograficoService;

    private readonly IEjemplarService
        _ejemplarService;

    public HomeController(
        IMaterialBibliograficoService materialBibliograficoService,
        IEjemplarService ejemplarService)
    {
        _materialBibliograficoService =
            materialBibliograficoService;

        _ejemplarService =
            ejemplarService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int totalMateriales =
            await _materialBibliograficoService.ContarAsync();

        int totalEjemplares =
            await _ejemplarService.ContarAsync();

        int ejemplaresDisponibles =
            await _ejemplarService.ContarPorEstadoAsync(
                EstadoEjemplar.Disponible);

        int ejemplaresPrestados =
            await _ejemplarService.ContarPorEstadoAsync(
                EstadoEjemplar.Prestado);

        DashboardViewModel viewModel = new()
        {
            TotalMateriales = totalMateriales,

            TotalEjemplares = totalEjemplares,

            EjemplaresDisponibles =
                ejemplaresDisponibles,

            EjemplaresPrestados =
                ejemplaresPrestados,

            PrestamosActivos = 0,
            PrestamosVencidos = 0,
            PersonasRegistradas = 0,
            DevolucionesPendientes = 0
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