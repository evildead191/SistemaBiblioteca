using SistemaBiblioteca.Business.DTOs.Ejemplar;
using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Business.Interfaces;

public interface IEjemplarService
{
    Task<List<EjemplarDto>> ObtenerTodosAsync(
        string? busqueda = null);

    Task<EjemplarDto?> ObtenerPorIdAsync(int id);

    Task<EjemplarEditDto?> ObtenerParaEditarAsync(int id);

    Task<(bool Exitoso, string Mensaje)> CrearAsync(
        EjemplarCreateDto dto);

    Task<(bool Exitoso, string Mensaje)> EditarAsync(
        EjemplarEditDto dto);

    Task<(bool Exitoso, string Mensaje)> CambiarEstadoAsync(
        int id);

    Task<int> ContarAsync();

    Task<int> ContarPorEstadoAsync(
        EstadoEjemplar estado);

    Task<(string? UltimoCodigo, string SiguienteCodigo)>
    ObtenerInformacionCodigosAsync();
}