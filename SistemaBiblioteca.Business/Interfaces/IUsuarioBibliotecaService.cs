using SistemaBiblioteca.Business.DTOs.UsuarioBiblioteca;
using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Business.Interfaces;

public interface IUsuarioBibliotecaService
{
    Task<List<UsuarioBibliotecaDto>> ObtenerTodosAsync(
        string? busqueda = null,
        TipoUsuarioBiblioteca? tipoUsuario = null);

    Task<UsuarioBibliotecaDto?> ObtenerPorIdAsync(
        int idUsuarioBiblioteca);

    Task<UsuarioBibliotecaEditDto?> ObtenerParaEditarAsync(
        int idUsuarioBiblioteca);

    Task<string> CrearAsync(
        UsuarioBibliotecaCreateDto dto);

    Task<string> EditarAsync(
        UsuarioBibliotecaEditDto dto);

    Task<string> CambiarEstadoAsync(
        int idUsuarioBiblioteca);

    Task<int> ContarUsuariosAsync();

    Task<int> ContarUsuariosActivosAsync();
}