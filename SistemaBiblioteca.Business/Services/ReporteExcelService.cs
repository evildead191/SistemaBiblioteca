using ClosedXML.Excel;
using SistemaBiblioteca.Business.DTOs.Reporte;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Business.Services;

public class ReporteExcelService : IReporteExcelService
{
    private const string NombreInstitucion =
        "Escuela Reverendo Francisco Schmitz";

    public byte[] GenerarMateriales(
        IReadOnlyCollection<ReporteMaterialDto> materiales)
    {
        using XLWorkbook libro = new();

        IXLWorksheet hoja =
            libro.Worksheets.Add("Materiales");

        string[] encabezados =
        {
            "Ficha",
            "Clasificación",
            "Título",
            "Autor",
            "Año",
            "Total de ejemplares",
            "Disponibles",
            "Prestados",
            "Estado"
        };

        PrepararEncabezado(
            hoja,
            "Reporte de materiales bibliográficos",
            encabezados);

        int fila = 5;

        foreach (ReporteMaterialDto material in materiales)
        {
            hoja.Cell(fila, 1).Value =
                material.NumeroFicha;

            hoja.Cell(fila, 2).Value =
                material.Clasificacion;

            hoja.Cell(fila, 3).Value =
                material.Titulo;

            hoja.Cell(fila, 4).Value =
                material.Autor;

            if (material.AnioPublicacion.HasValue)
            {
                hoja.Cell(fila, 5).Value =
                    material.AnioPublicacion.Value;
            }
            else
            {
                hoja.Cell(fila, 5).Value =
                    "No indicado";
            }

            hoja.Cell(fila, 6).Value =
                material.TotalEjemplares;

            hoja.Cell(fila, 7).Value =
                material.EjemplaresDisponibles;

            hoja.Cell(fila, 8).Value =
                material.EjemplaresPrestados;

            hoja.Cell(fila, 9).Value =
                material.Activo
                    ? "Activo"
                    : "Inactivo";

            fila++;
        }

        FinalizarHoja(
            hoja,
            encabezados.Length,
            fila - 1);

        return ConvertirABytes(libro);
    }

    public byte[] GenerarEjemplares(
        IReadOnlyCollection<ReporteEjemplarDto> ejemplares)
    {
        using XLWorkbook libro = new();

        IXLWorksheet hoja =
            libro.Worksheets.Add("Ejemplares");

        string[] encabezados =
        {
            "Código de barras",
            "Número de inscripción",
            "Ficha",
            "Material",
            "Autor",
            "Biblioteca",
            "Estado",
            "Estado del registro"
        };

        PrepararEncabezado(
            hoja,
            "Reporte de ejemplares",
            encabezados);

        int fila = 5;

        foreach (ReporteEjemplarDto ejemplar in ejemplares)
        {
            hoja.Cell(fila, 1).Value =
                ejemplar.CodigoBarras;

            hoja.Cell(fila, 2).Value =
                TextoOpcional(
                    ejemplar.NumeroInscripcion);

            hoja.Cell(fila, 3).Value =
                ejemplar.NumeroFicha;

            hoja.Cell(fila, 4).Value =
                ejemplar.TituloMaterial;

            hoja.Cell(fila, 5).Value =
                ejemplar.AutorMaterial;

            hoja.Cell(fila, 6).Value =
                TextoOpcional(
                    ejemplar.Biblioteca);

            hoja.Cell(fila, 7).Value =
                ObtenerNombreEstadoEjemplar(
                    ejemplar.Estado);

            hoja.Cell(fila, 8).Value =
                ejemplar.Activo
                    ? "Activo"
                    : "Inactivo";

            fila++;
        }

        FinalizarHoja(
            hoja,
            encabezados.Length,
            fila - 1);

        return ConvertirABytes(libro);
    }

    public byte[] GenerarUsuarios(
        IReadOnlyCollection<ReporteUsuarioDto> usuarios)
    {
        using XLWorkbook libro = new();

        IXLWorksheet hoja =
            libro.Worksheets.Add("Usuarios");

        string[] encabezados =
        {
            "Identificación",
            "Nombre completo",
            "Tipo de usuario",
            "Sección o departamento",
            "Correo",
            "Teléfono",
            "Total de préstamos",
            "Préstamos activos",
            "Préstamos atrasados",
            "Estado"
        };

        PrepararEncabezado(
            hoja,
            "Reporte de usuarios",
            encabezados);

        int fila = 5;

        foreach (ReporteUsuarioDto usuario in usuarios)
        {
            hoja.Cell(fila, 1).Value =
                usuario.Identificacion;

            hoja.Cell(fila, 2).Value =
                usuario.NombreCompleto;

            hoja.Cell(fila, 3).Value =
                ObtenerNombreTipoUsuario(
                    usuario.TipoUsuario);

            hoja.Cell(fila, 4).Value =
                TextoOpcional(
                    usuario.SeccionODepartamento);

            hoja.Cell(fila, 5).Value =
                TextoOpcional(
                    usuario.Correo);

            hoja.Cell(fila, 6).Value =
                TextoOpcional(
                    usuario.Telefono);

            hoja.Cell(fila, 7).Value =
                usuario.TotalPrestamos;

            hoja.Cell(fila, 8).Value =
                usuario.PrestamosActivos;

            hoja.Cell(fila, 9).Value =
                usuario.PrestamosAtrasados;

            hoja.Cell(fila, 10).Value =
                usuario.Activo
                    ? "Activo"
                    : "Inactivo";

            fila++;
        }

        FinalizarHoja(
            hoja,
            encabezados.Length,
            fila - 1);

        return ConvertirABytes(libro);
    }

    public byte[] GenerarPrestamos(
        IReadOnlyCollection<ReportePrestamoDto> prestamos)
    {
        using XLWorkbook libro = new();

        IXLWorksheet hoja =
            libro.Worksheets.Add("Préstamos");

        string[] encabezados =
        {
            "Identificación",
            "Usuario",
            "Tipo de usuario",
            "Código del ejemplar",
            "Número de inscripción",
            "Material",
            "Autor",
            "Fecha de préstamo",
            "Fecha límite",
            "Fecha de devolución",
            "Estado",
            "Días prestado",
            "Días de atraso",
            "Observaciones"
        };

        PrepararEncabezado(
            hoja,
            "Reporte de préstamos",
            encabezados);

        int fila = 5;

        foreach (ReportePrestamoDto prestamo in prestamos)
        {
            hoja.Cell(fila, 1).Value =
                prestamo.IdentificacionUsuario;

            hoja.Cell(fila, 2).Value =
                prestamo.NombreUsuario;

            hoja.Cell(fila, 3).Value =
                ObtenerNombreTipoUsuario(
                    prestamo.TipoUsuario);

            hoja.Cell(fila, 4).Value =
                prestamo.CodigoBarras;

            hoja.Cell(fila, 5).Value =
                TextoOpcional(
                    prestamo.NumeroInscripcion);

            hoja.Cell(fila, 6).Value =
                prestamo.TituloMaterial;

            hoja.Cell(fila, 7).Value =
                prestamo.AutorMaterial;

            hoja.Cell(fila, 8).Value =
                prestamo.FechaPrestamo;

            hoja.Cell(fila, 9).Value =
                prestamo.FechaLimiteDevolucion;

            if (prestamo.FechaDevolucionReal.HasValue)
            {
                hoja.Cell(fila, 10).Value =
                    prestamo.FechaDevolucionReal.Value;
            }
            else
            {
                hoja.Cell(fila, 10).Value =
                    "Pendiente";
            }

            hoja.Cell(fila, 11).Value =
                prestamo.EstaAtrasado
                    ? "Atrasado"
                    : ObtenerNombreEstadoPrestamo(
                        prestamo.Estado);

            hoja.Cell(fila, 12).Value =
                prestamo.DiasPrestado;

            hoja.Cell(fila, 13).Value =
                prestamo.DiasAtraso;

            hoja.Cell(fila, 14).Value =
                TextoOpcional(
                    prestamo.Observaciones);

            fila++;
        }

        if (fila > 5)
        {
            hoja.Range(
                    5,
                    8,
                    fila - 1,
                    10)
                .Style.DateFormat.Format =
                    "dd/MM/yyyy";
        }

        FinalizarHoja(
            hoja,
            encabezados.Length,
            fila - 1);

        return ConvertirABytes(libro);
    }

    private static void PrepararEncabezado(
        IXLWorksheet hoja,
        string titulo,
        IReadOnlyList<string> encabezados)
    {
        int ultimaColumna =
            encabezados.Count;

        hoja.Cell(1, 1).Value =
            NombreInstitucion;

        hoja.Range(
                1,
                1,
                1,
                ultimaColumna)
            .Merge();

        hoja.Cell(2, 1).Value =
            titulo;

        hoja.Range(
                2,
                1,
                2,
                ultimaColumna)
            .Merge();

        hoja.Cell(3, 1).Value =
            $"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}";

        hoja.Range(
                3,
                1,
                3,
                ultimaColumna)
            .Merge();

        IXLRange tituloInstitucion =
            hoja.Range(
                1,
                1,
                1,
                ultimaColumna);

        tituloInstitucion.Style.Font.Bold = true;
        tituloInstitucion.Style.Font.FontSize = 16;
        tituloInstitucion.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        IXLRange tituloReporte =
            hoja.Range(
                2,
                1,
                2,
                ultimaColumna);

        tituloReporte.Style.Font.Bold = true;
        tituloReporte.Style.Font.FontSize = 14;
        tituloReporte.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        IXLRange fechaGeneracion =
            hoja.Range(
                3,
                1,
                3,
                ultimaColumna);

        fechaGeneracion.Style.Font.Italic = true;
        fechaGeneracion.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        for (int columna = 0;
             columna < encabezados.Count;
             columna++)
        {
            hoja.Cell(4, columna + 1).Value =
                encabezados[columna];
        }

        IXLRange encabezado =
            hoja.Range(
                4,
                1,
                4,
                ultimaColumna);

        encabezado.Style.Font.Bold = true;
        encabezado.Style.Font.FontColor =
            XLColor.White;

        encabezado.Style.Fill.BackgroundColor =
            XLColor.FromHtml("#075C32");

        encabezado.Style.Alignment.Horizontal =
            XLAlignmentHorizontalValues.Center;

        encabezado.Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;

        encabezado.Style.Border.BottomBorder =
            XLBorderStyleValues.Thin;

        hoja.SheetView.FreezeRows(4);
    }

    private static void FinalizarHoja(
        IXLWorksheet hoja,
        int totalColumnas,
        int ultimaFila)
    {
        int filaFinal =
            Math.Max(ultimaFila, 4);

        IXLRange rango =
            hoja.Range(
                4,
                1,
                filaFinal,
                totalColumnas);

        rango.Style.Border.OutsideBorder =
            XLBorderStyleValues.Thin;

        rango.Style.Border.InsideBorder =
            XLBorderStyleValues.Hair;

        rango.Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Center;

        rango.Style.Alignment.WrapText = true;

        hoja.Columns().AdjustToContents(
            8,
            40);

        hoja.Row(1).Height = 24;
        hoja.Row(2).Height = 22;
        hoja.Row(4).Height = 24;

        hoja.PageSetup.PageOrientation =
            XLPageOrientation.Landscape;

        hoja.PageSetup.FitToPages(1, 0);

        hoja.PageSetup.Margins.Top = 0.5;
        hoja.PageSetup.Margins.Bottom = 0.5;
        hoja.PageSetup.Margins.Left = 0.4;
        hoja.PageSetup.Margins.Right = 0.4;
    }

    private static byte[] ConvertirABytes(
        XLWorkbook libro)
    {
        using MemoryStream stream = new();

        libro.SaveAs(stream);

        return stream.ToArray();
    }

    private static string TextoOpcional(
        string? texto)
    {
        return string.IsNullOrWhiteSpace(texto)
            ? "No indicado"
            : texto.Trim();
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
}