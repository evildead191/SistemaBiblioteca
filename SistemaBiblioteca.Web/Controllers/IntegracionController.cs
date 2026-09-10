using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaBiblioteca.Business.DTOs.Integracion;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.Web.ViewModels.Integracion;

namespace SistemaBiblioteca.Web.Controllers;

[Authorize(Roles = "Administrador")]
public class IntegracionController : Controller
{
    private readonly IIntegracionService _integracionService;

    private const string SessionPreview =
        "Integracion_Preview";

    private const string SessionNombreArchivo =
        "Integracion_NombreArchivo";

    private const int TamanoPagina = 25;

    public IntegracionController(
        IIntegracionService integracionService)
    {
        _integracionService = integracionService;
    }

    // =========================================================
    // INDEX
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        IntegracionIndexViewModel model = new()
        {
            Historial =
                await _integracionService
                    .ObtenerHistorialAsync()
        };

        return View(model);
    }

    // =========================================================
    // CARGAR ARCHIVO
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cargar(
        IFormFile? archivo)
    {
        if (archivo is null || archivo.Length == 0)
        {
            TempData["MensajeError"] =
                "Seleccione un archivo para importar.";

            return RedirectToAction(nameof(Index));
        }

        string extension =
            Path.GetExtension(archivo.FileName);

        if (!extension.Equals(
                ".xlsx",
                StringComparison.OrdinalIgnoreCase))
        {
            TempData["MensajeError"] =
                "El archivo debe tener formato .xlsx.";

            return RedirectToAction(nameof(Index));
        }

        try
        {
            await using Stream stream =
                archivo.OpenReadStream();

            IntegracionVistaPreviaDto preview =
                await _integracionService
                    .GenerarVistaPreviaAsync(
                        stream,
                        archivo.FileName);

            if (preview.Filas.Count == 0)
            {
                TempData["MensajeError"] =
                    "El archivo no contiene registros para importar.";

                return RedirectToAction(nameof(Index));
            }

            GuardarPreview(preview.Filas);

            HttpContext.Session.SetString(
                SessionNombreArchivo,
                archivo.FileName);

            return RedirectToAction(
                nameof(Preview));
        }
        catch (Exception ex)
        {
            Exception errorReal = ex;

            while (errorReal.InnerException is not null)
            {
                errorReal = errorReal.InnerException;
            }

            TempData["MensajeError"] =
                $"Error" +
                $"{errorReal.Message}";

            return RedirectToAction(
                nameof(Preview));
        }
    }

    // =========================================================
    // VISTA PREVIA / PAGINACIÓN / FILTROS
    // =========================================================

    [HttpGet]
    public IActionResult Preview(
        string? busqueda = null,
        string filtroEstado = "Todos",
        int pagina = 1)
    {
        List<IntegracionFilaDto>? filas =
            ObtenerPreview();

        if (filas is null || filas.Count == 0)
        {
            TempData["MensajeError"] =
                "No existe una importación pendiente.";

            return RedirectToAction(nameof(Index));
        }

        if (pagina < 1)
            pagina = 1;

        IEnumerable<IntegracionFilaDto> consulta =
            filas;

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string termino =
                busqueda.Trim();

            consulta = consulta.Where(x =>
                Contiene(
                    x.NumeroFicha,
                    termino) ||
                Contiene(
                    x.Clasificacion,
                    termino) ||
                Contiene(
                    x.Autor,
                    termino) ||
                Contiene(
                    x.Titulo,
                    termino) ||
                Contiene(
                    x.CodigoBarras,
                    termino) ||
                Contiene(
                    x.NumeroInscripcion,
                    termino));
        }

        consulta = filtroEstado switch
        {
            "Validos" =>
                consulta.Where(x =>
                    x.EsValida &&
                    !x.YaExiste),

            "Errores" =>
                consulta.Where(x =>
                    !x.EsValida),

            "Existentes" =>
                consulta.Where(x =>
                    x.YaExiste),

            _ => consulta
        };

        List<IntegracionFilaDto> filasFiltradas =
            consulta.ToList();

        int totalFiltrado =
            filasFiltradas.Count;

        int totalPaginas =
            Math.Max(
                1,
                (int)Math.Ceiling(
                    totalFiltrado /
                    (double)TamanoPagina));

        if (pagina > totalPaginas)
            pagina = totalPaginas;

        List<IntegracionFilaViewModel> filasPagina =
            filasFiltradas
                .Skip(
                    (pagina - 1) *
                    TamanoPagina)
                .Take(TamanoPagina)
                .Select(MapearFila)
                .ToList();

        IntegracionPreviewViewModel model = new()
        {
            NombreArchivo =
                ObtenerNombreArchivo(),

            Filas =
                filas.Select(MapearFila)
                    .ToList(),

            Busqueda =
                busqueda,

            FiltroEstado =
                filtroEstado,

            PaginaActual =
                pagina,

            TamanoPagina =
                TamanoPagina,

            TotalPaginas =
                totalPaginas,

            TotalFiltrado =
                totalFiltrado,

            FilasPagina =
                filasPagina
        };

        return View(model);
    }

    // =========================================================
    // EDITAR FILA
    // =========================================================

    [HttpGet]
    public IActionResult EditarFilaModal(
        int numeroFila)
    {
        List<IntegracionFilaDto>? filas =
            ObtenerPreview();

        if (filas is null)
            return NotFound();

        IntegracionFilaDto? fila =
            filas.FirstOrDefault(x =>
                x.NumeroFila == numeroFila);

        if (fila is null)
            return NotFound();

        IntegracionEditarFilaViewModel model = new()
        {
            NumeroFila =
                fila.NumeroFila,

            NumeroFicha =
                fila.NumeroFicha,

            Clasificacion =
                fila.Clasificacion,

            Autor =
                fila.Autor,

            Titulo =
                fila.Titulo,

            AnioPublicacion =
                fila.AnioPublicacion,

            CodigoBarras =
                fila.CodigoBarras,

            NumeroInscripcion =
                fila.NumeroInscripcion,

            Estado =
                fila.Estado,

            Biblioteca =
                fila.Biblioteca
        };

        return PartialView(
            "_EditarFilaModal",
            model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EditarFila(
        IntegracionEditarFilaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return PartialView(
                "_EditarFilaModal",
                model);
        }

        List<IntegracionFilaDto>? filas =
            ObtenerPreview();

        if (filas is null)
        {
            return Json(new
            {
                success = false,
                message =
                    "La vista previa ya no se encuentra disponible."
            });
        }

        IntegracionFilaDto? fila =
            filas.FirstOrDefault(x =>
                x.NumeroFila ==
                model.NumeroFila);

        if (fila is null)
        {
            return Json(new
            {
                success = false,
                message =
                    "No se encontró la fila seleccionada."
            });
        }

        string codigoAnterior =
            fila.CodigoBarras;

        fila.NumeroFicha =
            model.NumeroFicha.Trim();

        fila.Clasificacion =
            model.Clasificacion.Trim();

        fila.Autor =
            model.Autor.Trim();

        fila.Titulo =
            model.Titulo.Trim();

        fila.AnioPublicacion =
            model.AnioPublicacion;

        fila.CodigoBarras =
            model.CodigoBarras.Trim();

        fila.NumeroInscripcion =
            LimpiarOpcional(
                model.NumeroInscripcion);

        fila.Estado =
            "Disponible";

        fila.Biblioteca =
            LimpiarOpcional(
                model.Biblioteca);

        RevalidarFilas(
            filas,
            model.NumeroFila,
            codigoAnterior);

        GuardarPreview(filas);

        return Json(new
        {
            success = true,
            message =
                "La fila fue actualizada correctamente."
        });
    }

    // =========================================================
    // CONFIRMAR IMPORTACIÓN
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult>
        Confirmar()
    {
        List<IntegracionFilaDto>? filas =
            ObtenerPreview();

        if (filas is null || filas.Count == 0)
        {
            TempData["MensajeError"] =
                "No existe una importación pendiente.";

            return RedirectToAction(nameof(Index));
        }

        int errores =
            filas.Count(x => !x.EsValida);

        if (errores > 0)
        {
            TempData["MensajeError"] =
                $"Existen {errores} fila(s) con errores. " +
                "Corríjalas antes de confirmar la importación.";

            return RedirectToAction(
                nameof(Preview),
                new
                {
                    filtroEstado = "Errores"
                });
        }

        string nombreArchivo =
            ObtenerNombreArchivo();

        string usuarioEjecutor =
            User.Identity?.Name
            ?? "Administrador";

        try
        {
            IntegracionResultadoDto resultado =
                await _integracionService
                    .ImportarAsync(
                        filas,
                        nombreArchivo,
                        usuarioEjecutor);

            if (resultado.Exitoso)
            {
                LimpiarPreview();

                TempData["MensajeExito"] =
                    resultado.Mensaje;

                return RedirectToAction(
                    nameof(Index));
            }

            TempData["MensajeError"] =
                resultado.Mensaje;

            return RedirectToAction(
                nameof(Preview));
        }
        catch (Exception ex)
        {
            Exception errorReal = ex;

            while (errorReal.InnerException is not null)
            {
                errorReal = errorReal.InnerException;
            }

            TempData["MensajeError"] =
                $"No fue posible completar la importación. " +
                $"{errorReal.Message}";

            return RedirectToAction(
                nameof(Preview));
        }
    }

    // =========================================================
    // CANCELAR
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Cancelar()
    {
        LimpiarPreview();

        TempData["MensajeExito"] =
            "La importación pendiente fue cancelada.";

        return RedirectToAction(
            nameof(Index));
    }

    // =========================================================
    // SESSION
    // =========================================================

    private void GuardarPreview(
        List<IntegracionFilaDto> filas)
    {
        string json =
            JsonSerializer.Serialize(filas);

        HttpContext.Session.SetString(
            SessionPreview,
            json);
    }

    private List<IntegracionFilaDto>?
        ObtenerPreview()
    {
        string? json =
            HttpContext.Session.GetString(
                SessionPreview);

        if (string.IsNullOrWhiteSpace(json))
            return null;

        return JsonSerializer.Deserialize<
            List<IntegracionFilaDto>>(json);
    }

    private string ObtenerNombreArchivo()
    {
        return HttpContext.Session.GetString(
                   SessionNombreArchivo)
               ?? "Archivo.xlsx";
    }

    private void LimpiarPreview()
    {
        HttpContext.Session.Remove(
            SessionPreview);

        HttpContext.Session.Remove(
            SessionNombreArchivo);
    }

    // =========================================================
    // VALIDACIÓN DE FILAS EDITADAS
    // =========================================================

    private static void RevalidarFilas(
        List<IntegracionFilaDto> filas,
        int numeroFilaEditada,
        string codigoAnterior)
    {
        IntegracionFilaDto? fila =
            filas.FirstOrDefault(x =>
                x.NumeroFila ==
                numeroFilaEditada);

        if (fila is null)
            return;

        List<string> errores =
            new();

        if (string.IsNullOrWhiteSpace(
                fila.NumeroFicha))
        {
            errores.Add(
                "La ficha es obligatoria.");
        }

        if (string.IsNullOrWhiteSpace(
                fila.Clasificacion))
        {
            errores.Add(
                "La clasificación es obligatoria.");
        }

        if (string.IsNullOrWhiteSpace(
                fila.Autor))
        {
            errores.Add(
                "El autor es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(
                fila.Titulo))
        {
            errores.Add(
                "El título es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(
                fila.CodigoBarras))
        {
            errores.Add(
                "El código de barras es obligatorio.");
        }

        if (fila.AnioPublicacion.HasValue &&
            (fila.AnioPublicacion.Value < 1000 ||
             fila.AnioPublicacion.Value >
             DateTime.Now.Year))
        {
            errores.Add(
                "El año de publicación no es válido.");
        }

        if (!string.IsNullOrWhiteSpace(
                fila.CodigoBarras))
        {
            bool repetido =
                filas.Any(x =>
                    x.NumeroFila !=
                    fila.NumeroFila &&
                    x.CodigoBarras.Equals(
                        fila.CodigoBarras,
                        StringComparison.OrdinalIgnoreCase));

            if (repetido)
            {
                errores.Add(
                    "El código de barras está repetido dentro del archivo.");
            }
        }

        /*
         * YaExiste viene de la validación original contra
         * la base de datos.
         *
         * Si el administrador modifica el código que originalmente
         * existía, ya no podemos asumir que el nuevo código exista.
         * La capa Business volverá a comprobarlo al importar.
         */
        if (!fila.CodigoBarras.Equals(
                codigoAnterior,
                StringComparison.OrdinalIgnoreCase))
        {
            fila.YaExiste = false;
        }

        fila.Estado =
            "Disponible";

        fila.EsValida =
            errores.Count == 0;

        fila.Error =
            errores.Count == 0
                ? null
                : string.Join(
                    " ",
                    errores);
    }

    // =========================================================
    // HELPERS
    // =========================================================

    private static bool Contiene(
        string? valor,
        string termino)
    {
        return !string.IsNullOrWhiteSpace(valor) &&
               valor.Contains(
                   termino,
                   StringComparison.OrdinalIgnoreCase);
    }

    private static string? LimpiarOpcional(
        string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return null;

        return valor.Trim();
    }

    private static IntegracionFilaViewModel
        MapearFila(
            IntegracionFilaDto fila)
    {
        return new IntegracionFilaViewModel
        {
            NumeroFila =
                fila.NumeroFila,

            NumeroFicha =
                fila.NumeroFicha,

            Clasificacion =
                fila.Clasificacion,

            Autor =
                fila.Autor,

            Titulo =
                fila.Titulo,

            AnioPublicacion =
                fila.AnioPublicacion,

            CodigoBarras =
                fila.CodigoBarras,

            NumeroInscripcion =
                fila.NumeroInscripcion,

            Estado =
                fila.Estado,

            Biblioteca =
                fila.Biblioteca,

            EsValida =
                fila.EsValida,

            YaExiste =
                fila.YaExiste,

            Error =
                fila.Error
        };
    }
}