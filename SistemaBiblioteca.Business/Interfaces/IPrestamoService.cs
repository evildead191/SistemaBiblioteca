using SistemaBiblioteca.Business.DTOs.Prestamo;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.Business.Interfaces;

public interface IPrestamoService
{
    Task<List<PrestamoDto>> ObtenerTodosAsync(
        string? busqueda = null,
        EstadoPrestamo? estado = null,
        bool soloAtrasados = false);

    Task<PrestamoDetalleDto?> ObtenerPorIdAsync(int id);

    Task<List<UsuarioBiblioteca>> ObtenerUsuariosActivosAsync();

    Task<List<Ejemplar>> ObtenerEjemplaresDisponiblesAsync();

    Task<string?> RegistrarAsync(PrestamoCreateDto dto);

    Task<PrestamoDevolucionDto?> ObtenerParaDevolucionAsync(
        int idPrestamo);

    Task<string?> RegistrarDevolucionAsync(
        PrestamoDevolucionDto dto);

    Task<int> ContarPrestamosActivosAsync();

    Task<int> ContarPrestamosAtrasadosAsync();
}