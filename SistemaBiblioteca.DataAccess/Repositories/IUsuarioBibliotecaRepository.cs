using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.DataAccess.Repositories;

public interface IUsuarioBibliotecaRepository
{
    Task<List<UsuarioBiblioteca>> ObtenerTodosAsync(
        string? busqueda = null,
        TipoUsuarioBiblioteca? tipoUsuario = null);

    Task<UsuarioBiblioteca?> ObtenerPorIdAsync(
        int idUsuarioBiblioteca);

    Task<bool> ExisteIdentificacionAsync(
        string identificacion,
        int? idExcluir = null);

    Task AgregarAsync(
        UsuarioBiblioteca usuarioBiblioteca);

    void Actualizar(
        UsuarioBiblioteca usuarioBiblioteca);

    Task GuardarCambiosAsync();

    Task<int> ContarAsync();

    Task<int> ContarActivosAsync();
}