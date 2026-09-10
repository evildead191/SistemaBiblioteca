using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaBiblioteca.Business.DTOs.Reporte;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Web.ViewModels.Reporte;

namespace SistemaBiblioteca.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class ReporteController : Controller
{
    private readonly IReporteService _reporteService;
    private readonly IReporteExcelService _excelService;

    public ReporteController(
        IReporteService reporteService,
        IReporteExcelService excelService)
    {
        _reporteService = reporteService;
        _excelService = excelService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<ReporteMaterialDto> materiales =
            await _reporteService.ObtenerMaterialesAsync(
                new ReporteMaterialFiltroDto());

        List<ReporteEjemplarDto> ejemplares =
            await _reporteService.ObtenerEjemplaresAsync(
                new ReporteEjemplarFiltroDto());

        List<ReporteUsuarioDto> usuarios =
            await _reporteService.ObtenerUsuariosAsync(
                new ReporteUsuarioFiltroDto());

        List<ReportePrestamoDto> prestamos =
            await _reporteService.ObtenerPrestamosAsync(
                new ReportePrestamoFiltroDto());

        ReporteIndexViewModel viewModel = new()
        {
            TotalMateriales =
                materiales.Count,

            TotalEjemplares =
                ejemplares.Count,

            TotalUsuarios =
                usuarios.Count,

            TotalPrestamos =
                prestamos.Count,

            PrestamosActivos =
                prestamos.Count(x =>
                    x.Estado ==
                        EstadoPrestamo.Activo),

            PrestamosAtrasados =
                prestamos.Count(x =>
                    x.EstaAtrasado)
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Materiales(
        [FromQuery] ReporteMaterialFiltroDto filtro)
    {
        filtro ??= new ReporteMaterialFiltroDto();

        ReporteMaterialViewModel viewModel = new()
        {
            Filtro = filtro,

            Materiales =
                await _reporteService
                    .ObtenerMaterialesAsync(filtro),

            EstadosRegistro =
                ObtenerEstadosRegistro(
                    filtro.Activo)
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Ejemplares(
        [FromQuery] ReporteEjemplarFiltroDto filtro)
    {
        filtro ??= new ReporteEjemplarFiltroDto();

        ReporteEjemplarViewModel viewModel = new()
        {
            Filtro = filtro,

            Ejemplares =
                await _reporteService
                    .ObtenerEjemplaresAsync(filtro),

            EstadosEjemplar =
                ObtenerEstadosEjemplar(
                    filtro.Estado),

            EstadosRegistro =
                ObtenerEstadosRegistro(
                    filtro.Activo)
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Usuarios(
        [FromQuery] ReporteUsuarioFiltroDto filtro)
    {
        filtro ??= new ReporteUsuarioFiltroDto();

        ReporteUsuarioViewModel viewModel = new()
        {
            Filtro = filtro,

            Usuarios =
                await _reporteService
                    .ObtenerUsuariosAsync(filtro),

            TiposUsuario =
                ObtenerTiposUsuario(
                    filtro.TipoUsuario),

            EstadosRegistro =
                ObtenerEstadosRegistro(
                    filtro.Activo)
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Prestamos(
        [FromQuery] ReportePrestamoFiltroDto filtro)
    {
        filtro ??= new ReportePrestamoFiltroDto();

        if (filtro.FechaInicial.HasValue &&
            filtro.FechaFinal.HasValue &&
            filtro.FechaInicial.Value.Date >
                filtro.FechaFinal.Value.Date)
        {
            ModelState.AddModelError(
                string.Empty,
                "La fecha inicial no puede ser posterior a la fecha final."
            );
        }

        List<ReportePrestamoDto> prestamos =
            ModelState.IsValid
                ? await _reporteService
                    .ObtenerPrestamosAsync(filtro)
                : new List<ReportePrestamoDto>();

        ReportePrestamoViewModel viewModel = new()
        {
            Filtro = filtro,

            Prestamos =
                prestamos,

            EstadosPrestamo =
                ObtenerEstadosPrestamo(
                    filtro.Estado)
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> ExportarMateriales(
    [FromQuery] ReporteMaterialFiltroDto filtro)
    {
        List<ReporteMaterialDto> datos =
            await _reporteService.ObtenerMaterialesAsync(filtro);

        byte[] archivo =
            _excelService.GenerarMateriales(datos);

        return File(
            archivo,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Reporte_Materiales_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportarEjemplares(
    [FromQuery] ReporteEjemplarFiltroDto filtro)
    {
        List<ReporteEjemplarDto> datos =
            await _reporteService.ObtenerEjemplaresAsync(filtro);

        byte[] archivo =
            _excelService.GenerarEjemplares(datos);

        return File(
            archivo,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Reporte_Ejemplares_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportarUsuarios(
    [FromQuery] ReporteUsuarioFiltroDto filtro)
    {
        List<ReporteUsuarioDto> datos =
            await _reporteService.ObtenerUsuariosAsync(filtro);

        byte[] archivo =
            _excelService.GenerarUsuarios(datos);

        return File(
            archivo,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Reporte_Usuarios_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportarPrestamos(
    [FromQuery] ReportePrestamoFiltroDto filtro)
    {
        List<ReportePrestamoDto> datos =
            await _reporteService.ObtenerPrestamosAsync(filtro);

        byte[] archivo =
            _excelService.GenerarPrestamos(datos);

        return File(
            archivo,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Reporte_Prestamos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
    }

    private static List<SelectListItem>
        ObtenerEstadosRegistro(
            bool? estadoSeleccionado = null)
    {
        return new List<SelectListItem>
        {
            new()
            {
                Value = string.Empty,
                Text = "Todos",
                Selected = !estadoSeleccionado.HasValue
            },
            new()
            {
                Value = bool.TrueString,
                Text = "Activos",
                Selected =
                    estadoSeleccionado == true
            },
            new()
            {
                Value = bool.FalseString,
                Text = "Inactivos",
                Selected =
                    estadoSeleccionado == false
            }
        };
    }

    private static List<SelectListItem>
        ObtenerEstadosEjemplar(
            EstadoEjemplar? estadoSeleccionado = null)
    {
        List<SelectListItem> estados =
            new()
            {
                new SelectListItem
                {
                    Value = string.Empty,
                    Text = "Todos",
                    Selected =
                        !estadoSeleccionado.HasValue
                }
            };

        estados.AddRange(
            Enum
                .GetValues<EstadoEjemplar>()
                .Select(estado =>
                    new SelectListItem
                    {
                        Value =
                            ((int)estado).ToString(),

                        Text =
                            ObtenerNombreEstadoEjemplar(
                                estado),

                        Selected =
                            estadoSeleccionado.HasValue &&
                            estado ==
                                estadoSeleccionado.Value
                    })
        );

        return estados;
    }

    private static List<SelectListItem>
        ObtenerTiposUsuario(
            TipoUsuarioBiblioteca?
                tipoSeleccionado = null)
    {
        List<SelectListItem> tipos =
            new()
            {
                new SelectListItem
                {
                    Value = string.Empty,
                    Text = "Todos",
                    Selected =
                        !tipoSeleccionado.HasValue
                }
            };

        tipos.AddRange(
            Enum
                .GetValues<TipoUsuarioBiblioteca>()
                .Select(tipo =>
                    new SelectListItem
                    {
                        Value =
                            ((int)tipo).ToString(),

                        Text =
                            ObtenerNombreTipoUsuario(tipo),

                        Selected =
                            tipoSeleccionado.HasValue &&
                            tipo ==
                                tipoSeleccionado.Value
                    })
        );

        return tipos;
    }

    private static List<SelectListItem>
        ObtenerEstadosPrestamo(
            EstadoPrestamo? estadoSeleccionado = null)
    {
        List<SelectListItem> estados =
            new()
            {
                new SelectListItem
                {
                    Value = string.Empty,
                    Text = "Todos",
                    Selected =
                        !estadoSeleccionado.HasValue
                }
            };

        estados.AddRange(
            Enum
                .GetValues<EstadoPrestamo>()
                .Select(estado =>
                    new SelectListItem
                    {
                        Value =
                            ((int)estado).ToString(),

                        Text =
                            ObtenerNombreEstadoPrestamo(
                                estado),

                        Selected =
                            estadoSeleccionado.HasValue &&
                            estado ==
                                estadoSeleccionado.Value
                    })
        );

        return estados;
    }

    private static string ObtenerNombreEstadoEjemplar(
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

    private static string ObtenerNombreTipoUsuario(
        TipoUsuarioBiblioteca tipo)
    {
        return tipo switch
        {
            TipoUsuarioBiblioteca.Estudiante =>
                "Estudiante",

            TipoUsuarioBiblioteca.Docente =>
                "Docente",

            TipoUsuarioBiblioteca.Administrativo =>
                "Administrativo",

            _ => tipo.ToString()
        };
    }

    private static string ObtenerNombreEstadoPrestamo(
        EstadoPrestamo estado)
    {
        return estado switch
        {
            EstadoPrestamo.Activo =>
                "Activo",

            EstadoPrestamo.Devuelto =>
                "Devuelto",

            _ => estado.ToString()
        };
    }
}