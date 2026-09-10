using SistemaBiblioteca.Business.DTOs.Integracion;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.Business.Interfaces;

public interface IIntegracionService
{
    Task<IntegracionVistaPreviaDto> GenerarVistaPreviaAsync(
        Stream archivo,
        string nombreArchivo);

    Task<IntegracionResultadoDto> ImportarAsync(
        List<IntegracionFilaDto> filas,
        string nombreArchivo,
        string usuarioEjecutor);

    Task<List<IntegracionHistorial>>
        ObtenerHistorialAsync();
}