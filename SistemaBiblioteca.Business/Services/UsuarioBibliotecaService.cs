using SistemaBiblioteca.Business.DTOs.UsuarioBiblioteca;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.DataAccess.Repositories;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.Business.Services;

public class UsuarioBibliotecaService
    : IUsuarioBibliotecaService
{
    private readonly IUsuarioBibliotecaRepository
        _usuarioBibliotecaRepository;

    public UsuarioBibliotecaService(
        IUsuarioBibliotecaRepository usuarioBibliotecaRepository)
    {
        _usuarioBibliotecaRepository =
            usuarioBibliotecaRepository;
    }

    public async Task<List<UsuarioBibliotecaDto>> ObtenerTodosAsync(
        string? busqueda = null,
        TipoUsuarioBiblioteca? tipoUsuario = null)
    {
        List<UsuarioBiblioteca> usuarios =
            await _usuarioBibliotecaRepository
                .ObtenerTodosAsync(busqueda, tipoUsuario);

        return usuarios
            .Select(MapearADto)
            .ToList();
    }

    public async Task<UsuarioBibliotecaDto?> ObtenerPorIdAsync(
        int idUsuarioBiblioteca)
    {
        UsuarioBiblioteca? usuario =
            await _usuarioBibliotecaRepository
                .ObtenerPorIdAsync(idUsuarioBiblioteca);

        return usuario is null
            ? null
            : MapearADto(usuario);
    }

    public async Task<UsuarioBibliotecaEditDto?> ObtenerParaEditarAsync(
        int idUsuarioBiblioteca)
    {
        UsuarioBiblioteca? usuario =
            await _usuarioBibliotecaRepository
                .ObtenerPorIdAsync(idUsuarioBiblioteca);

        if (usuario is null)
        {
            return null;
        }

        return new UsuarioBibliotecaEditDto
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
                usuario.Activo
        };
    }

    public async Task<string> CrearAsync(
        UsuarioBibliotecaCreateDto dto)
    {
        ValidarTipoUsuario(dto.TipoUsuario);

        string identificacion =
            NormalizarTextoObligatorio(dto.Identificacion);

        string nombreCompleto =
            NormalizarTextoObligatorio(dto.NombreCompleto);

        bool identificacionExiste =
            await _usuarioBibliotecaRepository
                .ExisteIdentificacionAsync(identificacion);

        if (identificacionExiste)
        {
            throw new InvalidOperationException(
                "Ya existe un usuario registrado con esa identificación.");
        }

        UsuarioBiblioteca usuarioBiblioteca = new()
        {
            Identificacion =
                identificacion,

            NombreCompleto =
                nombreCompleto,

            TipoUsuario =
                dto.TipoUsuario,

            SeccionODepartamento =
                NormalizarTextoOpcional(
                    dto.SeccionODepartamento),

            Correo =
                NormalizarTextoOpcional(dto.Correo),

            Telefono =
                NormalizarTextoOpcional(dto.Telefono),

            Activo = true
        };

        await _usuarioBibliotecaRepository
            .AgregarAsync(usuarioBiblioteca);

        await _usuarioBibliotecaRepository
            .GuardarCambiosAsync();

        return "El usuario de biblioteca fue registrado correctamente.";
    }

    public async Task<string> EditarAsync(
        UsuarioBibliotecaEditDto dto)
    {
        ValidarTipoUsuario(dto.TipoUsuario);

        UsuarioBiblioteca? usuarioBiblioteca =
            await _usuarioBibliotecaRepository
                .ObtenerPorIdAsync(dto.IdUsuarioBiblioteca);

        if (usuarioBiblioteca is null)
        {
            throw new KeyNotFoundException(
                "No se encontró el usuario de biblioteca que desea editar.");
        }

        string identificacion =
            NormalizarTextoObligatorio(dto.Identificacion);

        string nombreCompleto =
            NormalizarTextoObligatorio(dto.NombreCompleto);

        bool identificacionExiste =
            await _usuarioBibliotecaRepository
                .ExisteIdentificacionAsync(
                    identificacion,
                    dto.IdUsuarioBiblioteca);

        if (identificacionExiste)
        {
            throw new InvalidOperationException(
                "Ya existe otro usuario registrado con esa identificación.");
        }

        usuarioBiblioteca.Identificacion =
            identificacion;

        usuarioBiblioteca.NombreCompleto =
            nombreCompleto;

        usuarioBiblioteca.TipoUsuario =
            dto.TipoUsuario;

        usuarioBiblioteca.SeccionODepartamento =
            NormalizarTextoOpcional(
                dto.SeccionODepartamento);

        usuarioBiblioteca.Correo =
            NormalizarTextoOpcional(dto.Correo);

        usuarioBiblioteca.Telefono =
            NormalizarTextoOpcional(dto.Telefono);

        usuarioBiblioteca.Activo =
            dto.Activo;

        _usuarioBibliotecaRepository
            .Actualizar(usuarioBiblioteca);

        await _usuarioBibliotecaRepository
            .GuardarCambiosAsync();

        return "La información del usuario fue actualizada correctamente.";
    }

    public async Task<string> CambiarEstadoAsync(
        int idUsuarioBiblioteca)
    {
        UsuarioBiblioteca? usuarioBiblioteca =
            await _usuarioBibliotecaRepository
                .ObtenerPorIdAsync(idUsuarioBiblioteca);

        if (usuarioBiblioteca is null)
        {
            throw new KeyNotFoundException(
                "No se encontró el usuario de biblioteca.");
        }

        usuarioBiblioteca.Activo =
            !usuarioBiblioteca.Activo;

        _usuarioBibliotecaRepository
            .Actualizar(usuarioBiblioteca);

        await _usuarioBibliotecaRepository
            .GuardarCambiosAsync();

        return usuarioBiblioteca.Activo
            ? "El usuario fue activado correctamente."
            : "El usuario fue desactivado correctamente.";
    }

    public async Task<int> ContarUsuariosAsync()
    {
        return await _usuarioBibliotecaRepository
            .ContarAsync();
    }

    public async Task<int> ContarUsuariosActivosAsync()
    {
        return await _usuarioBibliotecaRepository
            .ContarActivosAsync();
    }

    private static UsuarioBibliotecaDto MapearADto(
        UsuarioBiblioteca usuarioBiblioteca)
    {
        return new UsuarioBibliotecaDto
        {
            IdUsuarioBiblioteca =
                usuarioBiblioteca.IdUsuarioBiblioteca,

            Identificacion =
                usuarioBiblioteca.Identificacion,

            NombreCompleto =
                usuarioBiblioteca.NombreCompleto,

            TipoUsuario =
                usuarioBiblioteca.TipoUsuario,

            SeccionODepartamento =
                usuarioBiblioteca.SeccionODepartamento,

            Correo =
                usuarioBiblioteca.Correo,

            Telefono =
                usuarioBiblioteca.Telefono,

            Activo =
                usuarioBiblioteca.Activo
        };
    }

    private static void ValidarTipoUsuario(
        TipoUsuarioBiblioteca tipoUsuario)
    {
        if (!Enum.IsDefined(tipoUsuario))
        {
            throw new InvalidOperationException(
                "Debe seleccionar un tipo de usuario válido.");
        }
    }

    private static string NormalizarTextoObligatorio(
        string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return string.Empty;
        }

        return texto.Trim();
    }

    private static string? NormalizarTextoOpcional(
        string? texto)
    {
        return string.IsNullOrWhiteSpace(texto)
            ? null
            : texto.Trim();
    }
}