using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.DataAccess.Repositories;

public interface IIntegracionRepository
{
    Task AgregarHistorialAsync(
        IntegracionHistorial historial);

    Task<List<IntegracionHistorial>> ObtenerHistorialAsync();

    Task GuardarCambiosAsync();
}