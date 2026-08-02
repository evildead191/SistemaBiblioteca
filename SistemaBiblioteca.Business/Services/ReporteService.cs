using SistemaBiblioteca.Business.DTOs.Reporte;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.DataAccess.Repositories;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.Business.Services;

public class ReporteService : IReporteService
{
    private readonly IReporteRepository _repository;

    public ReporteService(
        IReporteRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ReporteMaterialDto>>
        ObtenerMaterialesAsync(
            ReporteMaterialFiltroDto filtro)
    {
        filtro ??= new ReporteMaterialFiltroDto();

        List<MaterialBibliografico> materiales =
            await _repository.ObtenerMaterialesAsync(
                LimpiarTexto(filtro.Busqueda),
                filtro.Activo);

        return materiales
            .Select(MapearMaterial)
            .ToList();
    }

    public async Task<List<ReporteEjemplarDto>>
        ObtenerEjemplaresAsync(
            ReporteEjemplarFiltroDto filtro)
    {
        filtro ??= new ReporteEjemplarFiltroDto();

        List<Ejemplar> ejemplares =
            await _repository.ObtenerEjemplaresAsync(
                LimpiarTexto(filtro.Busqueda),
                filtro.Estado,
                filtro.Activo);

        return ejemplares
            .Select(MapearEjemplar)
            .ToList();
    }

    public async Task<List<ReporteUsuarioDto>>
        ObtenerUsuariosAsync(
            ReporteUsuarioFiltroDto filtro)
    {
        filtro ??= new ReporteUsuarioFiltroDto();

        List<UsuarioBiblioteca> usuarios =
            await _repository.ObtenerUsuariosAsync(
                LimpiarTexto(filtro.Busqueda),
                filtro.TipoUsuario,
                filtro.Activo);

        return usuarios
            .Select(MapearUsuario)
            .ToList();
    }

    public async Task<List<ReportePrestamoDto>>
        ObtenerPrestamosAsync(
            ReportePrestamoFiltroDto filtro)
    {
        filtro ??= new ReportePrestamoFiltroDto();

        List<Prestamo> prestamos =
            await _repository.ObtenerPrestamosAsync(
                LimpiarTexto(filtro.Busqueda),
                filtro.Estado,
                filtro.FechaInicial,
                filtro.FechaFinal,
                filtro.SoloAtrasados);

        return prestamos
            .Select(MapearPrestamo)
            .ToList();
    }

    private static ReporteMaterialDto MapearMaterial(
        MaterialBibliografico material)
    {
        return new ReporteMaterialDto
        {
            IdMaterialBibliografico =
                material.IdMaterialBibliografico,

            NumeroFicha =
                material.NumeroFicha,

            Clasificacion =
                material.Clasificacion,

            Autor =
                material.Autor,

            Titulo =
                material.Titulo,

            AnioPublicacion =
                material.AnioPublicacion,

            Activo =
                material.Activo,

            TotalEjemplares =
                material.Ejemplares.Count,

            EjemplaresDisponibles =
                material.Ejemplares.Count(x =>
                    x.Activo &&
                    x.Estado ==
                        EstadoEjemplar.Disponible),

            EjemplaresPrestados =
                material.Ejemplares.Count(x =>
                    x.Activo &&
                    x.Estado ==
                        EstadoEjemplar.Prestado)
        };
    }

    private static ReporteEjemplarDto MapearEjemplar(
        Ejemplar ejemplar)
    {
        return new ReporteEjemplarDto
        {
            IdEjemplar =
                ejemplar.IdEjemplar,

            CodigoBarras =
                ejemplar.CodigoBarras,

            NumeroInscripcion =
                ejemplar.NumeroInscripcion,

            IdMaterialBibliografico =
                ejemplar.IdMaterialBibliografico,

            NumeroFicha =
                ejemplar.MaterialBibliografico?
                    .NumeroFicha
                ?? string.Empty,

            TituloMaterial =
                ejemplar.MaterialBibliografico?
                    .Titulo
                ?? string.Empty,

            AutorMaterial =
                ejemplar.MaterialBibliografico?
                    .Autor
                ?? string.Empty,

            Estado =
                ejemplar.Estado,

            Biblioteca =
                ejemplar.Biblioteca,

            Activo =
                ejemplar.Activo
        };
    }

    private static ReporteUsuarioDto MapearUsuario(
        UsuarioBiblioteca usuario)
    {
        DateTime hoy = DateTime.Today;

        return new ReporteUsuarioDto
        {
            IdUsuarioBiblioteca =
                usuario.IdUsuarioBiblioteca,

            Identificacion =
                usuario.Identificacion,

            NombreCompleto =
                usuario.NombreCompleto,

            TipoUsuario =
                usuario.TipoUsuario,

            SeccionODepartamento =
                usuario.SeccionODepartamento,

            Correo =
                usuario.Correo,

            Telefono =
                usuario.Telefono,

            Activo =
                usuario.Activo,

            TotalPrestamos =
                usuario.Prestamos.Count,

            PrestamosActivos =
                usuario.Prestamos.Count(x =>
                    x.Estado ==
                        EstadoPrestamo.Activo),

            PrestamosAtrasados =
                usuario.Prestamos.Count(x =>
                    x.Estado ==
                        EstadoPrestamo.Activo &&
                    x.FechaLimiteDevolucion.Date < hoy)
        };
    }

    private static ReportePrestamoDto MapearPrestamo(
        Prestamo prestamo)
    {
        DateTime fechaFinal =
            prestamo.FechaDevolucionReal
            ?? DateTime.Now;

        int diasPrestado =
            Math.Max(
                0,
                (fechaFinal.Date -
                 prestamo.FechaPrestamo.Date).Days);

        bool estaAtrasado =
            prestamo.Estado ==
                EstadoPrestamo.Activo &&
            DateTime.Today >
                prestamo.FechaLimiteDevolucion.Date;

        int diasAtraso =
            estaAtrasado
                ? (DateTime.Today -
                   prestamo.FechaLimiteDevolucion.Date).Days
                : 0;

        return new ReportePrestamoDto
        {
            IdPrestamo =
                prestamo.IdPrestamo,

            IdUsuarioBiblioteca =
                prestamo.IdUsuarioBiblioteca,

            IdentificacionUsuario =
                prestamo.UsuarioBiblioteca?
                    .Identificacion
                ?? string.Empty,

            NombreUsuario =
                prestamo.UsuarioBiblioteca?
                    .NombreCompleto
                ?? string.Empty,

            TipoUsuario =
                prestamo.UsuarioBiblioteca?
                    .TipoUsuario
                ?? default,

            IdEjemplar =
                prestamo.IdEjemplar,

            CodigoBarras =
                prestamo.Ejemplar?
                    .CodigoBarras
                ?? string.Empty,

            NumeroInscripcion =
                prestamo.Ejemplar?
                    .NumeroInscripcion,

            TituloMaterial =
                prestamo.Ejemplar?
                    .MaterialBibliografico?
                    .Titulo
                ?? string.Empty,

            AutorMaterial =
                prestamo.Ejemplar?
                    .MaterialBibliografico?
                    .Autor
                ?? string.Empty,

            FechaPrestamo =
                prestamo.FechaPrestamo,

            FechaLimiteDevolucion =
                prestamo.FechaLimiteDevolucion,

            FechaDevolucionReal =
                prestamo.FechaDevolucionReal,

            Estado =
                prestamo.Estado,

            Observaciones =
                prestamo.Observaciones,

            DiasPrestado =
                diasPrestado,

            EstaAtrasado =
                estaAtrasado,

            DiasAtraso =
                diasAtraso
        };
    }

    private static string? LimpiarTexto(
        string? texto)
    {
        return string.IsNullOrWhiteSpace(texto)
            ? null
            : texto.Trim();
    }
}