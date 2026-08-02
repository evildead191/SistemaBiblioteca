using SistemaBiblioteca.Business.DTOs.Prestamo;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.DataAccess.Repositories;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.Business.Services;

public class PrestamoService : IPrestamoService
{
    private readonly IPrestamoRepository _prestamoRepository;

    public PrestamoService(
        IPrestamoRepository prestamoRepository)
    {
        _prestamoRepository = prestamoRepository;
    }

    public async Task<List<PrestamoDto>> ObtenerTodosAsync(
        string? busqueda = null,
        EstadoPrestamo? estado = null,
        bool soloAtrasados = false)
    {
        List<Prestamo> prestamos =
            await _prestamoRepository.ObtenerTodosAsync(
                busqueda,
                estado,
                soloAtrasados);

        return prestamos
            .Select(MapearADto)
            .ToList();
    }

    public async Task<PrestamoDetalleDto?>
        ObtenerPorIdAsync(int id)
    {
        if (id <= 0)
        {
            return null;
        }

        Prestamo? prestamo =
            await _prestamoRepository.ObtenerPorIdAsync(id);

        if (prestamo is null)
        {
            return null;
        }

        return MapearADetalleDto(prestamo);
    }

    public async Task<List<UsuarioBiblioteca>>
        ObtenerUsuariosActivosAsync()
    {
        return await _prestamoRepository
            .ObtenerUsuariosActivosAsync();
    }

    public async Task<List<Ejemplar>>
        ObtenerEjemplaresDisponiblesAsync()
    {
        return await _prestamoRepository
            .ObtenerEjemplaresDisponiblesAsync();
    }

    public async Task<string?> RegistrarAsync(
        PrestamoCreateDto dto)
    {
        if (dto.IdUsuarioBiblioteca <= 0)
        {
            return "Debe seleccionar un usuario válido.";
        }

        if (dto.IdEjemplar <= 0)
        {
            return "Debe seleccionar un ejemplar válido.";
        }

        DateTime fechaPrestamo =
            dto.FechaPrestamo.Date;

        DateTime fechaLimiteDevolucion =
            dto.FechaLimiteDevolucion.Date;

        if (fechaLimiteDevolucion < fechaPrestamo)
        {
            return "La fecha límite de devolución no puede ser anterior a la fecha del préstamo.";
        }

        UsuarioBiblioteca? usuario =
            await _prestamoRepository
                .ObtenerUsuarioPorIdAsync(
                    dto.IdUsuarioBiblioteca);

        if (usuario is null)
        {
            return "El usuario seleccionado no existe.";
        }

        if (!usuario.Activo)
        {
            return "El usuario seleccionado se encuentra inactivo.";
        }

        Ejemplar? ejemplar =
            await _prestamoRepository
                .ObtenerEjemplarPorIdAsync(
                    dto.IdEjemplar);

        if (ejemplar is null)
        {
            return "El ejemplar seleccionado no existe.";
        }

        if (!ejemplar.Activo)
        {
            return "El ejemplar seleccionado se encuentra inactivo.";
        }

        if (!ejemplar.MaterialBibliografico.Activo)
        {
            return "El material bibliográfico asociado al ejemplar se encuentra inactivo.";
        }

        if (ejemplar.Estado != EstadoEjemplar.Disponible)
        {
            return ObtenerMensajeEstadoEjemplar(
                ejemplar.Estado);
        }

        bool existePrestamoActivo =
            await _prestamoRepository
                .ExistePrestamoActivoPorEjemplarAsync(
                    dto.IdEjemplar);

        if (existePrestamoActivo)
        {
            return "El ejemplar ya posee un préstamo activo.";
        }

        Prestamo prestamo = new()
        {
            IdUsuarioBiblioteca =
                dto.IdUsuarioBiblioteca,

            IdEjemplar =
                dto.IdEjemplar,

            FechaPrestamo =
                fechaPrestamo,

            FechaLimiteDevolucion =
                fechaLimiteDevolucion,

            FechaDevolucionReal =
                null,

            Estado =
                EstadoPrestamo.Activo,

            Observaciones =
                LimpiarTexto(dto.Observaciones)
        };

        ejemplar.Estado = EstadoEjemplar.Prestado;

        await _prestamoRepository
            .AgregarAsync(prestamo);

        _prestamoRepository
            .ActualizarEjemplar(ejemplar);

        await _prestamoRepository
            .GuardarCambiosAsync();

        return null;
    }

    public async Task<PrestamoDevolucionDto?>
        ObtenerParaDevolucionAsync(
            int idPrestamo)
    {
        if (idPrestamo <= 0)
        {
            return null;
        }

        Prestamo? prestamo =
            await _prestamoRepository
                .ObtenerPorIdAsync(idPrestamo);

        if (prestamo is null)
        {
            return null;
        }

        if (prestamo.Estado != EstadoPrestamo.Activo)
        {
            return null;
        }

        if (prestamo.FechaDevolucionReal.HasValue)
        {
            return null;
        }

        return new PrestamoDevolucionDto
        {
            IdPrestamo =
                prestamo.IdPrestamo,

            FechaDevolucionReal =
                DateTime.Today,

            Observaciones =
                prestamo.Observaciones
        };
    }

    public async Task<string?> RegistrarDevolucionAsync(
        PrestamoDevolucionDto dto)
    {
        if (dto.IdPrestamo <= 0)
        {
            return "El préstamo seleccionado no es válido.";
        }

        Prestamo? prestamo =
            await _prestamoRepository
                .ObtenerPorIdAsync(dto.IdPrestamo);

        if (prestamo is null)
        {
            return "El préstamo seleccionado no existe.";
        }

        if (prestamo.Estado != EstadoPrestamo.Activo)
        {
            return "El préstamo ya no se encuentra activo.";
        }

        if (prestamo.FechaDevolucionReal.HasValue)
        {
            return "La devolución de este préstamo ya fue registrada.";
        }

        DateTime fechaDevolucion =
            dto.FechaDevolucionReal.Date;

        if (fechaDevolucion < prestamo.FechaPrestamo.Date)
        {
            return "La fecha de devolución no puede ser anterior a la fecha del préstamo.";
        }

        Ejemplar? ejemplar = prestamo.Ejemplar;

        if (ejemplar is null)
        {
            ejemplar = await _prestamoRepository
                .ObtenerEjemplarPorIdAsync(
                    prestamo.IdEjemplar);
        }

        if (ejemplar is null)
        {
            return "No fue posible localizar el ejemplar asociado al préstamo.";
        }

        prestamo.FechaDevolucionReal =
            fechaDevolucion;

        prestamo.Estado =
            EstadoPrestamo.Devuelto;

        prestamo.Observaciones =
            LimpiarTexto(dto.Observaciones);

        ejemplar.Estado =
            EstadoEjemplar.Disponible;

        _prestamoRepository
            .ActualizarPrestamo(prestamo);

        _prestamoRepository
            .ActualizarEjemplar(ejemplar);

        await _prestamoRepository
            .GuardarCambiosAsync();

        return null;
    }

    public async Task<int>
        ContarPrestamosActivosAsync()
    {
        return await _prestamoRepository
            .ContarActivosAsync();
    }

    public async Task<int>
        ContarPrestamosAtrasadosAsync()
    {
        return await _prestamoRepository
            .ContarAtrasadosAsync();
    }

    private static PrestamoDto MapearADto(
        Prestamo prestamo)
    {
        return new PrestamoDto
        {
            IdPrestamo =
                prestamo.IdPrestamo,

            IdUsuarioBiblioteca =
                prestamo.IdUsuarioBiblioteca,

            IdentificacionUsuario =
                prestamo.UsuarioBiblioteca
                    .Identificacion,

            NombreUsuario =
                prestamo.UsuarioBiblioteca
                    .NombreCompleto,

            TipoUsuario =
                prestamo.UsuarioBiblioteca
                    .TipoUsuario.ToString(),

            IdEjemplar =
                prestamo.IdEjemplar,

            CodigoEjemplar =
                prestamo.Ejemplar
                    .CodigoBarras,

            TituloMaterial =
                prestamo.Ejemplar
                    .MaterialBibliografico
                    .Titulo,

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
                prestamo.DiasPrestado,

            EstaAtrasado =
                prestamo.EstaAtrasado,

            DiasAtraso =
                prestamo.DiasAtraso
        };
    }

    private static PrestamoDetalleDto
        MapearADetalleDto(
            Prestamo prestamo)
    {
        return new PrestamoDetalleDto
        {
            IdPrestamo =
                prestamo.IdPrestamo,

            IdUsuarioBiblioteca =
                prestamo.IdUsuarioBiblioteca,

            IdentificacionUsuario =
                prestamo.UsuarioBiblioteca
                    .Identificacion,

            NombreUsuario =
                prestamo.UsuarioBiblioteca
                    .NombreCompleto,

            TipoUsuario =
                prestamo.UsuarioBiblioteca
                    .TipoUsuario.ToString(),

            SeccionODepartamento =
                prestamo.UsuarioBiblioteca
                    .SeccionODepartamento,

            CorreoUsuario =
                prestamo.UsuarioBiblioteca
                    .Correo,

            TelefonoUsuario =
                prestamo.UsuarioBiblioteca
                    .Telefono,

            IdEjemplar =
                prestamo.IdEjemplar,

            CodigoEjemplar =
                prestamo.Ejemplar
                    .CodigoBarras,

            NumeroInscripcion =
                prestamo.Ejemplar
                    .NumeroInscripcion,

            TituloMaterial =
                prestamo.Ejemplar
                    .MaterialBibliografico
                    .Titulo,

            AutorMaterial =
                prestamo.Ejemplar
                    .MaterialBibliografico
                    .Autor,

            Biblioteca =
                prestamo.Ejemplar
                    .Biblioteca,

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
                prestamo.DiasPrestado,

            EstaAtrasado =
                prestamo.EstaAtrasado,

            DiasAtraso =
                prestamo.DiasAtraso
        };
    }

    private static string?
        LimpiarTexto(string? texto)
    {
        return string.IsNullOrWhiteSpace(texto)
            ? null
            : texto.Trim();
    }

    private static string
        ObtenerMensajeEstadoEjemplar(
            EstadoEjemplar estado)
    {
        return estado switch
        {
            EstadoEjemplar.Prestado =>
                "El ejemplar ya se encuentra prestado.",

            EstadoEjemplar.Mantenimiento =>
                "El ejemplar se encuentra en mantenimiento.",

            EstadoEjemplar.FueraDeServicio =>
                "El ejemplar se encuentra fuera de servicio.",

            _ =>
                "El ejemplar no se encuentra disponible."
        };
    }
}