using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaBiblioteca.Business.DTOs.Prestamo;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Entities.Models;
using SistemaBiblioteca.Web.ViewModels.Prestamo;

namespace SistemaBiblioteca.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class PrestamoController : Controller
{
    private readonly IPrestamoService _prestamoService;

    public PrestamoController(
        IPrestamoService prestamoService)
    {
        _prestamoService = prestamoService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? busqueda = null,
        EstadoPrestamo? estado = null,
        bool soloAtrasados = false)
    {
        PrestamoIndexViewModel viewModel = new()
        {
            Busqueda = busqueda,
            Estado = estado,
            SoloAtrasados = soloAtrasados,

            Prestamos = await _prestamoService
                .ObtenerTodosAsync(
                    busqueda,
                    estado,
                    soloAtrasados),

            Estados = ObtenerEstadosPrestamo()
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> CrearModal()
    {
        PrestamoCreateViewModel viewModel = new()
        {
            Prestamo = new PrestamoCreateDto
            {
                FechaPrestamo = DateTime.Today,
                FechaLimiteDevolucion =
                    DateTime.Today.AddDays(15)
            }
        };

        await CargarListasCrearAsync(viewModel);

        return PartialView(
            "_CrearPrestamoModal",
            viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        PrestamoCreateViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            await CargarListasCrearAsync(viewModel);

            return PartialView(
                "_CrearPrestamoModal",
                viewModel);
        }

        string? error =
            await _prestamoService
                .RegistrarAsync(viewModel.Prestamo);

        if (error is not null)
        {
            ModelState.AddModelError(
                string.Empty,
                error);

            await CargarListasCrearAsync(viewModel);

            return PartialView(
                "_CrearPrestamoModal",
                viewModel);
        }

        return Json(new
        {
            success = true,
            message = "El préstamo fue registrado correctamente."
        });
    }

    [HttpGet]
    public async Task<IActionResult> DetalleModal(
        int id)
    {
        PrestamoDetalleDto? prestamo =
            await _prestamoService
                .ObtenerPorIdAsync(id);

        if (prestamo is null)
        {
            return NotFound(
                "No se encontró el préstamo solicitado.");
        }

        PrestamoDetalleViewModel viewModel = new()
        {
            Prestamo = prestamo
        };

        return PartialView(
            "_DetallePrestamoModal",
            viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> DevolucionModal(
        int id)
    {
        PrestamoDevolucionDto? devolucion =
            await _prestamoService
                .ObtenerParaDevolucionAsync(id);

        PrestamoDetalleDto? detalle =
            await _prestamoService
                .ObtenerPorIdAsync(id);

        if (devolucion is null ||
            detalle is null)
        {
            return NotFound(
                "No se encontró un préstamo activo para registrar la devolución.");
        }

        PrestamoDevolucionViewModel viewModel = new()
        {
            Prestamo = devolucion,

            NombreUsuario =
                detalle.NombreUsuario,

            IdentificacionUsuario =
                detalle.IdentificacionUsuario,

            TituloMaterial =
                detalle.TituloMaterial,

            CodigoEjemplar =
                detalle.CodigoEjemplar,

            FechaPrestamo =
                detalle.FechaPrestamo,

            FechaLimiteDevolucion =
                detalle.FechaLimiteDevolucion,

            EstaAtrasado =
                detalle.EstaAtrasado,

            DiasAtraso =
                detalle.DiasAtraso
        };

        return PartialView(
            "_RegistrarDevolucionModal",
            viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistrarDevolucion(
        PrestamoDevolucionViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            await CompletarDatosDevolucionAsync(
                viewModel);

            return PartialView(
                "_RegistrarDevolucionModal",
                viewModel);
        }

        string? error =
            await _prestamoService
                .RegistrarDevolucionAsync(
                    viewModel.Prestamo);

        if (error is not null)
        {
            ModelState.AddModelError(
                string.Empty,
                error);

            await CompletarDatosDevolucionAsync(
                viewModel);

            return PartialView(
                "_RegistrarDevolucionModal",
                viewModel);
        }

        return Json(new
        {
            success = true,
            message = "La devolución fue registrada correctamente."
        });
    }

    private async Task CargarListasCrearAsync(
        PrestamoCreateViewModel viewModel)
    {
        List<UsuarioBiblioteca> usuarios =
            await _prestamoService
                .ObtenerUsuariosActivosAsync();

        List<Ejemplar> ejemplares =
            await _prestamoService
                .ObtenerEjemplaresDisponiblesAsync();

        viewModel.Usuarios = usuarios
            .Select(usuario =>
                new SelectListItem
                {
                    Value =
                        usuario.IdUsuarioBiblioteca
                            .ToString(),

                    Text =
                        $"{usuario.Identificacion} - " +
                        $"{usuario.NombreCompleto}"
                })
            .ToList();

        viewModel.EjemplaresDisponibles =
            ejemplares
                .Select(ejemplar =>
                    new SelectListItem
                    {
                        Value =
                            ejemplar.IdEjemplar
                                .ToString(),

                        Text =
                            $"{ejemplar.CodigoBarras} - " +
                            $"{ejemplar.MaterialBibliografico.Titulo}"
                    })
                .ToList();

        viewModel.Usuarios.Insert(
            0,
            new SelectListItem
            {
                Value = string.Empty,
                Text = "Seleccione un usuario"
            });

        viewModel.EjemplaresDisponibles.Insert(
            0,
            new SelectListItem
            {
                Value = string.Empty,
                Text = "Seleccione un ejemplar"
            });
    }

    private async Task CompletarDatosDevolucionAsync(
        PrestamoDevolucionViewModel viewModel)
    {
        PrestamoDetalleDto? detalle =
            await _prestamoService
                .ObtenerPorIdAsync(
                    viewModel.Prestamo.IdPrestamo);

        if (detalle is null)
        {
            return;
        }

        viewModel.NombreUsuario =
            detalle.NombreUsuario;

        viewModel.IdentificacionUsuario =
            detalle.IdentificacionUsuario;

        viewModel.TituloMaterial =
            detalle.TituloMaterial;

        viewModel.CodigoEjemplar =
            detalle.CodigoEjemplar;

        viewModel.FechaPrestamo =
            detalle.FechaPrestamo;

        viewModel.FechaLimiteDevolucion =
            detalle.FechaLimiteDevolucion;

        viewModel.EstaAtrasado =
            detalle.EstaAtrasado;

        viewModel.DiasAtraso =
            detalle.DiasAtraso;
    }

    private static List<SelectListItem>
        ObtenerEstadosPrestamo()
    {
        List<SelectListItem> estados =
            Enum.GetValues<EstadoPrestamo>()
                .Select(estado =>
                    new SelectListItem
                    {
                        Value =
                            ((int)estado).ToString(),

                        Text =
                            ObtenerNombreEstado(estado)
                    })
                .ToList();

        estados.Insert(
            0,
            new SelectListItem
            {
                Value = string.Empty,
                Text = "Todos los estados"
            });

        return estados;
    }

    private static string ObtenerNombreEstado(
        EstadoPrestamo estado)
    {
        return estado switch
        {
            EstadoPrestamo.Activo =>
                "Activo",

            EstadoPrestamo.Devuelto =>
                "Devuelto",

            _ =>
                estado.ToString()
        };
    }
}