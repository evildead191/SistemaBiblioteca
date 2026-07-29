using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.DataAccess.Repositories;

public interface IMaterialBibliograficoRepository
{
    Task<List<MaterialBibliograficoListadoResult>> ObtenerTodosAsync(
        string? busqueda = null);

    Task<MaterialBibliografico?> ObtenerPorIdAsync(int id);

    Task<bool> ExisteNumeroFichaAsync(
        string numeroFicha,
        int? idExcluir = null);

    Task AgregarAsync(MaterialBibliografico material);

    void Actualizar(MaterialBibliografico material);

    Task GuardarCambiosAsync();

    Task<int> ContarAsync();
}