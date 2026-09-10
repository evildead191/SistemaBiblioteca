using ClosedXML.Excel;
using SistemaBiblioteca.Business.DTOs.Integracion;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.DataAccess.Repositories;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.Business.Services;

public class IntegracionService : IIntegracionService
{
    private readonly IMaterialBibliograficoRepository
        _materialRepository;

    private readonly IEjemplarRepository
        _ejemplarRepository;

    private readonly IIntegracionRepository
        _integracionRepository;

    private static readonly string[] EncabezadosEsperados =
    {
        "NumeroFicha",
        "Clasificacion",
        "Autor",
        "Titulo",
        "AnioPublicacion",
        "CodigoBarras",
        "NumeroInscripcion",
        "Estado",
        "Biblioteca"
    };

    public IntegracionService(
        IMaterialBibliograficoRepository materialRepository,
        IEjemplarRepository ejemplarRepository,
        IIntegracionRepository integracionRepository)
    {
        _materialRepository = materialRepository;
        _ejemplarRepository = ejemplarRepository;
        _integracionRepository = integracionRepository;
    }

    public async Task<IntegracionVistaPreviaDto>
        GenerarVistaPreviaAsync(
            Stream archivo,
            string nombreArchivo)
    {
        if (archivo is null || !archivo.CanRead)
        {
            throw new ArgumentException(
                "No fue posible leer el archivo seleccionado.");
        }

        if (!Path.GetExtension(nombreArchivo)
            .Equals(
                ".xlsx",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "El archivo debe tener formato .xlsx.");
        }

        using XLWorkbook libro = new(archivo);

        IXLWorksheet? hoja =
            libro.Worksheets.FirstOrDefault();

        if (hoja is null)
        {
            throw new InvalidOperationException(
                "El archivo no contiene hojas para importar.");
        }

        ValidarEncabezados(hoja);

        List<string> codigosRegistrados =
            await _ejemplarRepository
                .ObtenerCodigosBarrasAsync();

        HashSet<string> codigosExistentes = new(
            codigosRegistrados,
            StringComparer.OrdinalIgnoreCase);

        HashSet<string> codigosEncontradosEnArchivo =
            new(StringComparer.OrdinalIgnoreCase);

        IntegracionVistaPreviaDto vistaPrevia = new()
        {
            NombreArchivo = nombreArchivo
        };

        IXLRow? ultimaFilaUtilizada =
            hoja.LastRowUsed();

        if (ultimaFilaUtilizada is null ||
            ultimaFilaUtilizada.RowNumber() <= 1)
        {
            return vistaPrevia;
        }

        int numeroUltimaFila =
            ultimaFilaUtilizada.RowNumber();

        for (int numeroFila = 2;
             numeroFila <= numeroUltimaFila;
             numeroFila++)
        {
            IXLRow filaExcel =
                hoja.Row(numeroFila);

            if (FilaVacia(filaExcel))
                continue;

            IntegracionFilaDto fila =
                CrearFila(
                    filaExcel,
                    numeroFila);

            ValidarFila(
                fila,
                codigosExistentes,
                codigosEncontradosEnArchivo);

            vistaPrevia.Filas.Add(fila);
        }

        return vistaPrevia;
    }

    public async Task<IntegracionResultadoDto>
        ImportarAsync(
            List<IntegracionFilaDto> filas,
            string nombreArchivo,
            string usuarioEjecutor)
    {
        if (filas is null || filas.Count == 0)
        {
            throw new ArgumentException(
                "No existen filas para importar.");
        }

        IntegracionResultadoDto resultado = new()
        {
            FilasProcesadas = filas.Count
        };

        List<IntegracionFilaDto> filasImportables =
            filas
                .Where(x =>
                    x.EsValida &&
                    !x.YaExiste)
                .ToList();

        resultado.FilasConError =
            filas.Count(x => !x.EsValida);

        resultado.FilasOmitidas =
            filas.Count(x => x.YaExiste);

        if (filasImportables.Count == 0)
        {
            resultado.Exitoso = false;

            resultado.Mensaje =
                "No existen filas válidas nuevas para importar.";

            await RegistrarHistorialAsync(
                nombreArchivo,
                usuarioEjecutor,
                resultado,
                "Sin registros",
                resultado.Mensaje);

            return resultado;
        }

        List<string> codigosRegistrados =
            await _ejemplarRepository
                .ObtenerCodigosBarrasAsync();

        HashSet<string> codigosExistentes = new(
            codigosRegistrados,
            StringComparer.OrdinalIgnoreCase);

        HashSet<string> codigosProcesados =
            new(StringComparer.OrdinalIgnoreCase);

        List<IntegracionFilaDto> filasFinales =
            new();

        foreach (IntegracionFilaDto fila
                 in filasImportables)
        {
            if (codigosExistentes.Contains(
                    fila.CodigoBarras) ||
                !codigosProcesados.Add(
                    fila.CodigoBarras))
            {
                resultado.FilasOmitidas++;
                continue;
            }

            filasFinales.Add(fila);
        }

        if (filasFinales.Count == 0)
        {
            resultado.Exitoso = false;

            resultado.Mensaje =
                "Todos los ejemplares del archivo ya se encuentran registrados.";

            await RegistrarHistorialAsync(
                nombreArchivo,
                usuarioEjecutor,
                resultado,
                "Sin registros",
                resultado.Mensaje);

            return resultado;
        }

        IEnumerable<IGrouping<string, IntegracionFilaDto>>
            gruposPorFicha =
                filasFinales.GroupBy(
                    x => x.NumeroFicha,
                    StringComparer.OrdinalIgnoreCase);

        foreach (IGrouping<string, IntegracionFilaDto>
                 grupo in gruposPorFicha)
        {
            IntegracionFilaDto primeraFila =
                grupo.First();

            MaterialBibliografico? material =
                await _materialRepository
                    .ObtenerPorNumeroFichaAsync(
                        primeraFila.NumeroFicha);

            if (material is null)
            {
                material = new MaterialBibliografico
                {
                    NumeroFicha =
                        primeraFila.NumeroFicha,

                    Clasificacion =
                        primeraFila.Clasificacion,

                    Autor =
                        primeraFila.Autor,

                    Titulo =
                        primeraFila.Titulo,

                    AnioPublicacion =
                        primeraFila.AnioPublicacion,

                    Activo = true
                };

                await _materialRepository
                    .AgregarAsync(material);

                resultado.MaterialesCreados++;
            }

            foreach (IntegracionFilaDto fila
                     in grupo)
            {
                Ejemplar ejemplar = new()
                {
                    CodigoBarras =
                        fila.CodigoBarras,

                    NumeroInscripcion =
                        fila.NumeroInscripcion,

                    Estado =
                        EstadoEjemplar.Disponible,

                    Biblioteca =
                        string.IsNullOrWhiteSpace(
                            fila.Biblioteca)
                            ? "Escuela Reverendo Francisco Schmitz"
                            : fila.Biblioteca.Trim(),

                    Activo = true,

                    MaterialBibliografico =
                        material
                };

                await _ejemplarRepository
                    .AgregarAsync(ejemplar);

                resultado.EjemplaresCreados++;
            }
        }

        /*
         * Ambos repositorios utilizan el mismo DbContext
         * Scoped. Un único SaveChanges guarda materiales
         * y ejemplares pendientes.
         */
        await _ejemplarRepository
            .GuardarCambiosAsync();

        resultado.FilasImportadas =
            resultado.EjemplaresCreados;

        resultado.Exitoso = true;

        resultado.Mensaje =
            $"Importación completada. " +
            $"{resultado.MaterialesCreados} materiales y " +
            $"{resultado.EjemplaresCreados} ejemplares fueron registrados.";

        await RegistrarHistorialAsync(
            nombreArchivo,
            usuarioEjecutor,
            resultado,
            "Completado",
            resultado.Mensaje);

        return resultado;
    }

    public async Task<List<IntegracionHistorial>>
        ObtenerHistorialAsync()
    {
        return await _integracionRepository
            .ObtenerHistorialAsync();
    }

    private async Task RegistrarHistorialAsync(
        string nombreArchivo,
        string usuarioEjecutor,
        IntegracionResultadoDto resultado,
        string estado,
        string? detalle)
    {
        IntegracionHistorial historial = new()
        {
            NombreArchivo =
                string.IsNullOrWhiteSpace(nombreArchivo)
                    ? "Archivo sin nombre"
                    : nombreArchivo,

            FechaProceso = DateTime.Now,

            UsuarioEjecutor =
                string.IsNullOrWhiteSpace(usuarioEjecutor)
                    ? "Administrador"
                    : usuarioEjecutor,

            FilasProcesadas =
                resultado.FilasProcesadas,

            FilasImportadas =
                resultado.FilasImportadas,

            FilasConError =
                resultado.FilasConError,

            Estado = estado,

            Detalle = detalle
        };

        await _integracionRepository
            .AgregarHistorialAsync(historial);

        await _integracionRepository
            .GuardarCambiosAsync();
    }

    private static void ValidarEncabezados(
        IXLWorksheet hoja)
    {
        for (int columna = 1;
             columna <= EncabezadosEsperados.Length;
             columna++)
        {
            string encabezado =
                hoja.Cell(1, columna)
                    .GetString()
                    .Trim();

            string esperado =
                EncabezadosEsperados[
                    columna - 1];

            if (!encabezado.Equals(
                    esperado,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"La columna {columna} debe llamarse " +
                    $"'{esperado}'. Se encontró " +
                    $"'{encabezado}'.");
            }
        }
    }

    private static IntegracionFilaDto CrearFila(
        IXLRow filaExcel,
        int numeroFila)
    {
        string textoAnio =
            filaExcel.Cell(5)
                .GetString()
                .Trim();

        int? anioPublicacion = null;

        if (int.TryParse(
                textoAnio,
                out int anio))
        {
            anioPublicacion = anio;
        }

        return new IntegracionFilaDto
        {
            NumeroFila = numeroFila,

            NumeroFicha =
                filaExcel.Cell(1)
                    .GetString()
                    .Trim(),

            Clasificacion =
                filaExcel.Cell(2)
                    .GetString()
                    .Trim(),

            Autor =
                filaExcel.Cell(3)
                    .GetString()
                    .Trim(),

            Titulo =
                filaExcel.Cell(4)
                    .GetString()
                    .Trim(),

            AnioPublicacion =
                anioPublicacion,

            CodigoBarras =
                filaExcel.Cell(6)
                    .GetString()
                    .Trim(),

            NumeroInscripcion =
                ObtenerTextoOpcional(
                    filaExcel.Cell(7)),

            Estado =
                filaExcel.Cell(8)
                    .GetString()
                    .Trim(),

            Biblioteca =
                ObtenerTextoOpcional(
                    filaExcel.Cell(9))
        };
    }

    private static void ValidarFila(
        IntegracionFilaDto fila,
        HashSet<string> codigosExistentes,
        HashSet<string> codigosEncontradosEnArchivo)
    {
        List<string> errores = new();

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

        if (!string.Equals(
                fila.Estado,
                "Disponible",
                StringComparison.OrdinalIgnoreCase))
        {
            errores.Add(
                "El estado debe ser Disponible.");
        }

        if (!string.IsNullOrWhiteSpace(
                fila.CodigoBarras))
        {
            if (codigosExistentes.Contains(
                    fila.CodigoBarras))
            {
                fila.YaExiste = true;
            }
            else if (!codigosEncontradosEnArchivo
                .Add(fila.CodigoBarras))
            {
                errores.Add(
                    "El código de barras está repetido dentro del archivo.");
            }
        }

        fila.EsValida =
            errores.Count == 0;

        fila.Error =
            errores.Count > 0
                ? string.Join(" ", errores)
                : null;
    }

    private static bool FilaVacia(
        IXLRow fila)
    {
        for (int columna = 1;
             columna <= EncabezadosEsperados.Length;
             columna++)
        {
            if (!fila.Cell(columna).IsEmpty())
                return false;
        }

        return true;
    }

    private static string? ObtenerTextoOpcional(
        IXLCell celda)
    {
        string valor =
            celda.GetString().Trim();

        return string.IsNullOrWhiteSpace(valor)
            ? null
            : valor;
    }
}