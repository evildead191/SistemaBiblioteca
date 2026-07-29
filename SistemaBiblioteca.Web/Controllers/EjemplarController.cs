using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaBiblioteca.Business.DTOs.Ejemplar;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Web.ViewModels.Ejemplar;

namespace SistemaBiblioteca.Web.Controllers;

public class EjemplarController : Controller
{
    private readonly IEjemplarService _ejemplarService;
    private readonly IMaterialBibliograficoService
        _materialBibliograficoService;

    public EjemplarController(
        IEjemplarService ejemplarService,
        IMaterialBibliograficoService materialBibliograficoService)
    {
        _ejemplarService = ejemplarService;
        _materialBibliograficoService =
            materialBibliograficoService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? busqueda = null)
    {
        EjemplarIndexViewModel viewModel = new()
        {
            Busqueda = busqueda,

            Ejemplares =
                await _ejemplarService.ObtenerTodosAsync(
                    busqueda)
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> CrearModal()
    {
        EjemplarCreateViewModel viewModel = new()
        {
            Ejemplar = new EjemplarCreateDto
            {
                Estado = EstadoEjemplar.Disponible
            },

            MaterialesBibliograficos =
                await ObtenerMaterialesBibliograficosAsync(),

            Estados = ObtenerEstados()
        };

        return PartialView(
            "_CrearEjemplarModal",
            viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        EjemplarCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            viewModel.MaterialesBibliograficos =
                await ObtenerMaterialesBibliograficosAsync(
                    viewModel.Ejemplar.IdMaterialBibliografico);

            viewModel.Estados =
                ObtenerEstados(
                    viewModel.Ejemplar.Estado);

            return PartialView(
                "_CrearEjemplarModal",
                viewModel);
        }

        (bool exitoso, string mensaje) =
            await _ejemplarService.CrearAsync(
                viewModel.Ejemplar);

        if (!exitoso)
        {
            ModelState.AddModelError(
                string.Empty,
                mensaje);

            viewModel.MaterialesBibliograficos =
                await ObtenerMaterialesBibliograficosAsync(
                    viewModel.Ejemplar.IdMaterialBibliografico);

            viewModel.Estados =
                ObtenerEstados(
                    viewModel.Ejemplar.Estado);

            return PartialView(
                "_CrearEjemplarModal",
                viewModel);
        }

        return Json(new
        {
            exitoso = true,
            mensaje
        });
    }

    [HttpGet]
    public async Task<IActionResult> EditarModal(int id)
    {
        EjemplarEditDto? ejemplar =
            await _ejemplarService.ObtenerParaEditarAsync(id);

        if (ejemplar is null)
        {
            return NotFound();
        }

        EjemplarEditViewModel viewModel = new()
        {
            Ejemplar = ejemplar,

            MaterialesBibliograficos =
                await ObtenerMaterialesBibliograficosAsync(
                    ejemplar.IdMaterialBibliografico),

            Estados =
                ObtenerEstados(
                    ejemplar.Estado)
        };

        return PartialView(
            "_EditarEjemplarModal",
            viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        EjemplarEditViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            viewModel.MaterialesBibliograficos =
                await ObtenerMaterialesBibliograficosAsync(
                    viewModel.Ejemplar.IdMaterialBibliografico);

            viewModel.Estados =
                ObtenerEstados(
                    viewModel.Ejemplar.Estado);

            return PartialView(
                "_EditarEjemplarModal",
                viewModel);
        }

        (bool exitoso, string mensaje) =
            await _ejemplarService.EditarAsync(
                viewModel.Ejemplar);

        if (!exitoso)
        {
            ModelState.AddModelError(
                string.Empty,
                mensaje);

            viewModel.MaterialesBibliograficos =
                await ObtenerMaterialesBibliograficosAsync(
                    viewModel.Ejemplar.IdMaterialBibliografico);

            viewModel.Estados =
                ObtenerEstados(
                    viewModel.Ejemplar.Estado);

            return PartialView(
                "_EditarEjemplarModal",
                viewModel);
        }

        return Json(new
        {
            exitoso = true,
            mensaje
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id)
    {
        (bool exitoso, string mensaje) =
            await _ejemplarService.CambiarEstadoAsync(id);

        if (exitoso)
        {
            TempData["MensajeExito"] = mensaje;
        }
        else
        {
            TempData["MensajeError"] = mensaje;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>>
        ObtenerMaterialesBibliograficosAsync(
            int? idSeleccionado = null)
    {
        var materiales =
            await _materialBibliograficoService
                .ObtenerTodosAsync();

        return materiales
            .Where(x =>
                x.Activo ||
                x.IdMaterialBibliografico ==
                    idSeleccionado)
            .OrderBy(x => x.Titulo)
            .Select(x => new SelectListItem
            {
                Value =
                    x.IdMaterialBibliografico.ToString(),

                Text =
                    $"{x.NumeroFicha} - {x.Titulo}",

                Selected =
                    x.IdMaterialBibliografico ==
                        idSeleccionado
            })
            .ToList();
    }

    private static List<SelectListItem> ObtenerEstados(
        EstadoEjemplar? estadoSeleccionado = null)
    {
        return Enum
            .GetValues<EstadoEjemplar>()
            .Select(estado => new SelectListItem
            {
                Value = ((int)estado).ToString(),

                Text = ObtenerNombreEstado(estado),

                Selected =
                    estadoSeleccionado.HasValue &&
                    estado == estadoSeleccionado.Value
            })
            .ToList();
    }

    private static string ObtenerNombreEstado(
        EstadoEjemplar estado)
    {
        return estado switch
        {
            EstadoEjemplar.Disponible =>
                "Disponible",

            EstadoEjemplar.Prestado =>
                "Prestado",

            EstadoEjemplar.Mantenimiento =>
                "Mantenimiento",

            EstadoEjemplar.FueraDeServicio =>
                "Fuera de servicio",

            _ => estado.ToString()
        };
    }
}