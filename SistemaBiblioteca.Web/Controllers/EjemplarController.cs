using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaBiblioteca.Business.DTOs.Ejemplar;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Web.ViewModels.Ejemplar;

namespace SistemaBiblioteca.Web.Controllers;

[Authorize(Roles = "Administrador")]
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
        (string? ultimoCodigo, string siguienteCodigo) =
            await _ejemplarService
                .ObtenerInformacionCodigosAsync();

        EjemplarCreateViewModel viewModel = new()
        {
            Ejemplar = new EjemplarCreateDto
            {
                CodigoBarras = "EJ",

                Estado =
                    EstadoEjemplar.Disponible,

                Cantidad = 1,

                GenerarCodigosAutomaticamente =
                    true
            },

            MaterialesBibliograficos =
                await ObtenerMaterialesBibliograficosAsync(),

            Estados =
                ObtenerEstadosCreacion(),

            UltimoCodigoRegistrado =
                ultimoCodigo,

            SiguienteCodigoSugerido =
                siguienteCodigo
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
            await PrepararCrearViewModelAsync(
                viewModel);

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

            await PrepararCrearViewModelAsync(
                viewModel);

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
    public async Task<IActionResult> EditarModal(
        int id)
    {
        EjemplarEditDto? ejemplar =
            await _ejemplarService
                .ObtenerParaEditarAsync(id);

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
                ObtenerEstadosEdicion(
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
            await PrepararEditarViewModelAsync(
                viewModel);

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

            await PrepararEditarViewModelAsync(
                viewModel);

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
    public async Task<IActionResult> CambiarEstado(
        int id)
    {
        (bool exitoso, string mensaje) =
            await _ejemplarService
                .CambiarEstadoAsync(id);

        if (exitoso)
        {
            TempData["MensajeExito"] =
                mensaje;
        }
        else
        {
            TempData["MensajeError"] =
                mensaje;
        }

        return RedirectToAction(
            nameof(Index));
    }

    private async Task PrepararCrearViewModelAsync(
        EjemplarCreateViewModel viewModel)
    {
        viewModel.MaterialesBibliograficos =
            await ObtenerMaterialesBibliograficosAsync(
                viewModel.Ejemplar
                    .IdMaterialBibliografico);

        viewModel.Estados =
            ObtenerEstadosCreacion(
                viewModel.Ejemplar.Estado);

        (string? ultimoCodigo, string siguienteCodigo) =
            await _ejemplarService
                .ObtenerInformacionCodigosAsync();

        viewModel.UltimoCodigoRegistrado =
            ultimoCodigo;

        viewModel.SiguienteCodigoSugerido =
            siguienteCodigo;
    }

    private async Task PrepararEditarViewModelAsync(
        EjemplarEditViewModel viewModel)
    {
        viewModel.MaterialesBibliograficos =
            await ObtenerMaterialesBibliograficosAsync(
                viewModel.Ejemplar
                    .IdMaterialBibliografico);

        viewModel.Estados =
            ObtenerEstadosEdicion(
                viewModel.Ejemplar.Estado);
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
                    x.IdMaterialBibliografico
                        .ToString(),

                Text =
                    $"{x.NumeroFicha} - {x.Titulo}",

                Selected =
                    x.IdMaterialBibliografico ==
                        idSeleccionado
            })
            .ToList();
    }

    private static List<SelectListItem>
        ObtenerEstadosCreacion(
            EstadoEjemplar? estadoSeleccionado = null)
    {
        EstadoEjemplar[] estadosPermitidos =
        [
            EstadoEjemplar.Disponible,
            EstadoEjemplar.Mantenimiento,
            EstadoEjemplar.FueraDeServicio
        ];

        return estadosPermitidos
            .Select(estado =>
                new SelectListItem
                {
                    Value =
                        ((int)estado).ToString(),

                    Text =
                        ObtenerNombreEstado(
                            estado),

                    Selected =
                        estadoSeleccionado.HasValue &&
                        estado ==
                            estadoSeleccionado.Value
                })
            .ToList();
    }

    private static List<SelectListItem>
        ObtenerEstadosEdicion(
            EstadoEjemplar estadoSeleccionado)
    {
        /*
         * Prestado no se ofrece como una opción
         * seleccionable manualmente.
         *
         * Si el ejemplar ya está prestado,
         * el modal mostrará el estado como
         * solo lectura.
         */

        if (estadoSeleccionado ==
            EstadoEjemplar.Prestado)
        {
            return [];
        }

        return ObtenerEstadosCreacion(
            estadoSeleccionado);
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