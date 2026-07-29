using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.DataAccess.Repositories;

public interface IEjemplarRepository
{
    Task<List<Ejemplar>> ObtenerTodosAsync(
        string? busqueda = null);

    Task<Ejemplar?> ObtenerPorIdAsync(int id);

    Task<bool> ExisteCodigoBarrasAsync(
        string codigoBarras,
        int? idExcluir = null);

    Task<bool> ExisteMaterialBibliograficoAsync(
        int idMaterialBibliografico);

    Task AgregarAsync(Ejemplar ejemplar);

    void Actualizar(Ejemplar ejemplar);

    Task GuardarCambiosAsync();

    Task<int> ContarAsync();

    Task<int> ContarPorEstadoAsync(
        EstadoEjemplar estado);
}