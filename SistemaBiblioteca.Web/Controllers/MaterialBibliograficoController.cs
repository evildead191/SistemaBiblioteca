using Microsoft.AspNetCore.Mvc;
using SistemaBiblioteca.Business.DTOs.MaterialBibliografico;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.Web.ViewModels.MaterialBibliografico;

namespace SistemaBiblioteca.Web.Controllers;

public class MaterialBibliograficoController : Controller
{
    private readonly IMaterialBibliograficoService _service;

    public MaterialBibliograficoController(
        IMaterialBibliograficoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? busqueda)
    {
        MaterialBibliograficoIndexViewModel viewModel = new()
        {
            Busqueda = busqueda,
            Materiales = await _service.ObtenerTodosAsync(busqueda)
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult CrearModal()
    {
        return PartialView(
            "_CrearModal",
            new MaterialBibliograficoCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearModal(
    MaterialBibliograficoCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_CrearModal", dto);
        }

        (bool exitoso, string mensaje) =
            await _service.CrearAsync(dto);

        if (!exitoso)
        {
            ModelState.AddModelError(
                nameof(dto.NumeroFicha),
                mensaje);

            return PartialView("_CrearModal", dto);
        }

        return Json(new
        {
            success = true,
            message = mensaje
        });
    }

    [HttpGet]
    public async Task<IActionResult> EditarModal(int id)
    {
        MaterialBibliograficoEditDto? dto =
            await _service.ObtenerParaEditarAsync(id);

        if (dto is null)
        {
            return NotFound();
        }

        return PartialView("_EditarModal", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarModal(
    MaterialBibliograficoEditDto dto)
    {
        if (!ModelState.IsValid)
        {
            return PartialView("_EditarModal", dto);
        }

        (bool exitoso, string mensaje) =
            await _service.EditarAsync(dto);

        if (!exitoso)
        {
            return Json(new
            {
                success = false,
                message = mensaje
            });
        }

        return Json(new
        {
            success = true,
            message = mensaje
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(int id)
    {
        (bool exitoso, string mensaje) =
            await _service.CambiarEstadoAsync(id);

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
}