using SistemaBiblioteca.Business.DTOs.MaterialBibliografico;

namespace SistemaBiblioteca.Business.Interfaces;

public interface IMaterialBibliograficoService
{
    Task<List<MaterialBibliograficoDto>> ObtenerTodosAsync(
        string? busqueda = null);

    Task<MaterialBibliograficoDto?> ObtenerPorIdAsync(int id);

    Task<MaterialBibliograficoEditDto?> ObtenerParaEditarAsync(int id);

    Task<(bool Exitoso, string Mensaje)> CrearAsync(
        MaterialBibliograficoCreateDto dto);

    Task<(bool Exitoso, string Mensaje)> EditarAsync(
        MaterialBibliograficoEditDto dto);

    Task<(bool Exitoso, string Mensaje)> CambiarEstadoAsync(int id);
}