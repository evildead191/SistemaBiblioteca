using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaBiblioteca.Business.DTOs.UsuarioBiblioteca;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Web.ViewModels.UsuarioBiblioteca;

namespace SistemaBiblioteca.Web.Controllers;

public class UsuarioBibliotecaController : Controller
{
    private readonly IUsuarioBibliotecaService
        _usuarioBibliotecaService;

    public UsuarioBibliotecaController(
        IUsuarioBibliotecaService usuarioBibliotecaService)
    {
        _usuarioBibliotecaService =
            usuarioBibliotecaService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? busqueda = null,
        TipoUsuarioBiblioteca? tipoUsuario = null)
    {
        UsuarioBibliotecaIndexViewModel viewModel = new()
        {
            Usuarios = await _usuarioBibliotecaService
                .ObtenerTodosAsync(busqueda, tipoUsuario),

            Busqueda = busqueda,

            TipoUsuario = tipoUsuario,

            TiposUsuario = ObtenerTiposUsuario(
                incluirTodos: true,
                tipoSeleccionado: tipoUsuario)
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult CrearModal()
    {
        UsuarioBibliotecaCreateViewModel viewModel = new()
        {
            Usuario = new UsuarioBibliotecaCreateDto(),

            TiposUsuario = ObtenerTiposUsuario()
        };

        return PartialView(
            "_CrearUsuarioBibliotecaModal",
            viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        UsuarioBibliotecaCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            viewModel.TiposUsuario =
                ObtenerTiposUsuario(
                    tipoSeleccionado:
                        viewModel.Usuario.TipoUsuario);

            return PartialView(
                "_CrearUsuarioBibliotecaModal",
                viewModel);
        }

        try
        {
            string mensaje =
                await _usuarioBibliotecaService
                    .CrearAsync(viewModel.Usuario);

            return Json(new
            {
                exitoso = true,
                mensaje
            });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            viewModel.TiposUsuario =
                ObtenerTiposUsuario(
                    tipoSeleccionado:
                        viewModel.Usuario.TipoUsuario);

            return PartialView(
                "_CrearUsuarioBibliotecaModal",
                viewModel);
        }
        catch (Exception)
        {
            ModelState.AddModelError(
                string.Empty,
                "No fue posible registrar el usuario de biblioteca.");

            viewModel.TiposUsuario =
                ObtenerTiposUsuario(
                    tipoSeleccionado:
                        viewModel.Usuario.TipoUsuario);

            return PartialView(
                "_CrearUsuarioBibliotecaModal",
                viewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditarModal(
        int id)
    {
        UsuarioBibliotecaEditDto? usuario =
            await _usuarioBibliotecaService
                .ObtenerParaEditarAsync(id);

        if (usuario is null)
        {
            return NotFound();
        }

        UsuarioBibliotecaEditViewModel viewModel = new()
        {
            Usuario = usuario,

            TiposUsuario = ObtenerTiposUsuario(
                tipoSeleccionado:
                    usuario.TipoUsuario)
        };

        return PartialView(
            "_EditarUsuarioBibliotecaModal",
            viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        UsuarioBibliotecaEditViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            viewModel.TiposUsuario =
                ObtenerTiposUsuario(
                    tipoSeleccionado:
                        viewModel.Usuario.TipoUsuario);

            return PartialView(
                "_EditarUsuarioBibliotecaModal",
                viewModel);
        }

        try
        {
            string mensaje =
                await _usuarioBibliotecaService
                    .EditarAsync(viewModel.Usuario);

            return Json(new
            {
                exitoso = true,
                mensaje
            });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            viewModel.TiposUsuario =
                ObtenerTiposUsuario(
                    tipoSeleccionado:
                        viewModel.Usuario.TipoUsuario);

            return PartialView(
                "_EditarUsuarioBibliotecaModal",
                viewModel);
        }
        catch (KeyNotFoundException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            viewModel.TiposUsuario =
                ObtenerTiposUsuario(
                    tipoSeleccionado:
                        viewModel.Usuario.TipoUsuario);

            return PartialView(
                "_EditarUsuarioBibliotecaModal",
                viewModel);
        }
        catch (Exception)
        {
            ModelState.AddModelError(
                string.Empty,
                "No fue posible actualizar el usuario de biblioteca.");

            viewModel.TiposUsuario =
                ObtenerTiposUsuario(
                    tipoSeleccionado:
                        viewModel.Usuario.TipoUsuario);

            return PartialView(
                "_EditarUsuarioBibliotecaModal",
                viewModel);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarEstado(
        int id)
    {
        try
        {
            string mensaje =
                await _usuarioBibliotecaService
                    .CambiarEstadoAsync(id);

            TempData["MensajeExito"] = mensaje;
        }
        catch (KeyNotFoundException ex)
        {
            TempData["MensajeError"] =
                ex.Message;
        }
        catch (Exception)
        {
            TempData["MensajeError"] =
                "No fue posible cambiar el estado del usuario.";
        }

        return RedirectToAction(nameof(Index));
    }

    private static List<SelectListItem>
        ObtenerTiposUsuario(
            bool incluirTodos = false,
            TipoUsuarioBiblioteca? tipoSeleccionado = null)
    {
        List<SelectListItem> opciones = new();

        if (incluirTodos)
        {
            opciones.Add(new SelectListItem
            {
                Value = string.Empty,
                Text = "Todos los tipos",
                Selected = !tipoSeleccionado.HasValue
            });
        }
        else
        {
            opciones.Add(new SelectListItem
            {
                Value = string.Empty,
                Text = "Seleccione un tipo de usuario"
            });
        }

        foreach (
            TipoUsuarioBiblioteca tipoUsuario
            in Enum.GetValues<TipoUsuarioBiblioteca>())
        {
            opciones.Add(new SelectListItem
            {
                Value =
                    ((int)tipoUsuario).ToString(),

                Text =
                    ObtenerNombreTipoUsuario(tipoUsuario),

                Selected =
                    tipoSeleccionado.HasValue &&
                    tipoSeleccionado.Value == tipoUsuario
            });
        }

        return opciones;
    }

    private static string ObtenerNombreTipoUsuario(
        TipoUsuarioBiblioteca tipoUsuario)
    {
        return tipoUsuario switch
        {
            TipoUsuarioBiblioteca.Estudiante =>
                "Estudiante",

            TipoUsuarioBiblioteca.Docente =>
                "Docente",

            TipoUsuarioBiblioteca.Administrativo =>
                "Administrativo",

            TipoUsuarioBiblioteca.Otro =>
                "Otro",

            _ =>
                tipoUsuario.ToString()
        };
    }
}