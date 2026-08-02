using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.DataAccess.Repositories;

public interface IPrestamoRepository
{
    Task<List<Prestamo>> ObtenerTodosAsync(
        string? busqueda = null,
        EstadoPrestamo? estado = null,
        bool soloAtrasados = false);

    Task<Prestamo?> ObtenerPorIdAsync(int id);

    Task<UsuarioBiblioteca?> ObtenerUsuarioPorIdAsync(
        int idUsuarioBiblioteca);

    Task<Ejemplar?> ObtenerEjemplarPorIdAsync(
        int idEjemplar);

    Task<List<UsuarioBiblioteca>> ObtenerUsuariosActivosAsync();

    Task<List<Ejemplar>> ObtenerEjemplaresDisponiblesAsync();

    Task<bool> ExistePrestamoActivoPorEjemplarAsync(
        int idEjemplar);

    Task AgregarAsync(Prestamo prestamo);

    void ActualizarPrestamo(Prestamo prestamo);

    void ActualizarEjemplar(Ejemplar ejemplar);

    Task GuardarCambiosAsync();

    Task<int> ContarActivosAsync();

    Task<int> ContarAtrasadosAsync();
}