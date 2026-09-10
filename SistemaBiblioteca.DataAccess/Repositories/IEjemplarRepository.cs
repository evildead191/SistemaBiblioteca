using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.DataAccess.Repositories;

public interface IEjemplarRepository
{
    Task<List<Ejemplar>> ObtenerTodosAsync(
        string? busqueda = null,
        int pagina = 1,
        int tamanoPagina = 25);

    Task<int> ContarFiltradosAsync(
        string? busqueda = null);

    Task<Ejemplar?> ObtenerPorIdAsync(int id);

    Task<bool> ExisteCodigoBarrasAsync(
        string codigoBarras,
        int? idExcluir = null);

    Task<List<string>> ObtenerCodigosBarrasAsync();

    Task<bool> ExisteMaterialBibliograficoAsync(
        int idMaterialBibliografico);

    Task AgregarAsync(Ejemplar ejemplar);

    Task AgregarRangoAsync(
        IEnumerable<Ejemplar> ejemplares);

    void Actualizar(Ejemplar ejemplar);

    Task GuardarCambiosAsync();

    Task<int> ContarAsync();

    Task<int> ContarPorEstadoAsync(
        EstadoEjemplar estado);
}