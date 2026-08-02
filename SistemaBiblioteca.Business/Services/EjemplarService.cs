using SistemaBiblioteca.Business.DTOs.Ejemplar;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.DataAccess.Repositories;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.Business.Services;

public class EjemplarService : IEjemplarService
{
    private readonly IEjemplarRepository _repository;

    public EjemplarService(
        IEjemplarRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<EjemplarDto>> ObtenerTodosAsync(
        string? busqueda = null)
    {
        List<Ejemplar> ejemplares =
            await _repository.ObtenerTodosAsync(busqueda);

        return ejemplares
            .Select(MapearADto)
            .ToList();
    }

    public async Task<EjemplarDto?> ObtenerPorIdAsync(int id)
    {
        Ejemplar? ejemplar =
            await _repository.ObtenerPorIdAsync(id);

        return ejemplar is null
            ? null
            : MapearADto(ejemplar);
    }

    public async Task<EjemplarEditDto?> ObtenerParaEditarAsync(
        int id)
    {
        Ejemplar? ejemplar =
            await _repository.ObtenerPorIdAsync(id);

        if (ejemplar is null)
        {
            return null;
        }

        return new EjemplarEditDto
        {
            IdEjemplar = ejemplar.IdEjemplar,
            CodigoBarras = ejemplar.CodigoBarras,
            NumeroInscripcion = ejemplar.NumeroInscripcion,

            IdMaterialBibliografico =
                ejemplar.IdMaterialBibliografico,

            Estado = ejemplar.Estado,
            Biblioteca = ejemplar.Biblioteca,
            Activo = ejemplar.Activo
        };
    }

    public async Task<(bool Exitoso, string Mensaje)> CrearAsync(
        EjemplarCreateDto dto)
    {
        string codigoBarras =
            dto.CodigoBarras.Trim();

        bool codigoExistente =
            await _repository.ExisteCodigoBarrasAsync(
                codigoBarras);

        if (codigoExistente)
        {
            return (
                false,
                "Ya existe un ejemplar con ese código de barras."
            );
        }

        bool materialExistente =
            await _repository.ExisteMaterialBibliograficoAsync(
                dto.IdMaterialBibliografico);

        if (!materialExistente)
        {
            return (
                false,
                "El material bibliográfico seleccionado no existe o se encuentra inactivo."
            );
        }

        Ejemplar ejemplar = new()
        {
            CodigoBarras = codigoBarras,

            NumeroInscripcion =
                LimpiarTextoOpcional(
                    dto.NumeroInscripcion),

            IdMaterialBibliografico =
                dto.IdMaterialBibliografico,

            Estado = dto.Estado,

            Biblioteca = "Escuela Reverendo Francisco Schmitz",

            Activo = true
        };

        await _repository.AgregarAsync(ejemplar);
        await _repository.GuardarCambiosAsync();

        return (
            true,
            "El ejemplar fue registrado correctamente."
        );
    }

    public async Task<(bool Exitoso, string Mensaje)> EditarAsync(
        EjemplarEditDto dto)
    {
        Ejemplar? ejemplar =
            await _repository.ObtenerPorIdAsync(
                dto.IdEjemplar);

        if (ejemplar is null)
        {
            return (
                false,
                "No se encontró el ejemplar."
            );
        }

        string codigoBarras =
            dto.CodigoBarras.Trim();

        string? numeroInscripcion =
            LimpiarTextoOpcional(
                dto.NumeroInscripcion);

        bool codigoExistente =
            await _repository.ExisteCodigoBarrasAsync(
                codigoBarras,
                dto.IdEjemplar);

        if (codigoExistente)
        {
            return (
                false,
                "Ya existe otro ejemplar con ese código de barras."
            );
        }

        bool materialExistente =
            await _repository.ExisteMaterialBibliograficoAsync(
                dto.IdMaterialBibliografico);

        if (!materialExistente)
        {
            return (
                false,
                "El material bibliográfico seleccionado no existe o se encuentra inactivo."
            );
        }

        bool huboCambios =
            ejemplar.CodigoBarras != codigoBarras ||
            ejemplar.NumeroInscripcion != numeroInscripcion ||
            ejemplar.IdMaterialBibliografico !=
                dto.IdMaterialBibliografico ||
            ejemplar.Estado != dto.Estado ||
            ejemplar.Activo != dto.Activo;

        if (!huboCambios)
        {
            return (
                false,
                "No se realizaron modificaciones."
            );
        }

        ejemplar.CodigoBarras = codigoBarras;
        ejemplar.NumeroInscripcion = numeroInscripcion;

        ejemplar.IdMaterialBibliografico =
            dto.IdMaterialBibliografico;

        ejemplar.Estado = dto.Estado;
        ejemplar.Activo = dto.Activo;

        _repository.Actualizar(ejemplar);
        await _repository.GuardarCambiosAsync();

        return (
            true,
            "El ejemplar fue actualizado correctamente."
        );
    }

    public async Task<int> ContarAsync()
    {
        return await _repository.ContarAsync();
    }

    public async Task<int> ContarPorEstadoAsync(
        EstadoEjemplar estado)
    {
        return await _repository
            .ContarPorEstadoAsync(estado);
    }

    public async Task<(bool Exitoso, string Mensaje)>
        CambiarEstadoAsync(int id)
    {
        Ejemplar? ejemplar =
            await _repository.ObtenerPorIdAsync(id);

        if (ejemplar is null)
        {
            return (
                false,
                "No se encontró el ejemplar."
            );
        }

        ejemplar.Activo = !ejemplar.Activo;

        _repository.Actualizar(ejemplar);
        await _repository.GuardarCambiosAsync();

        string mensaje = ejemplar.Activo
            ? "El ejemplar fue activado correctamente."
            : "El ejemplar fue desactivado correctamente.";

        return (true, mensaje);
    }

    private static EjemplarDto MapearADto(
        Ejemplar ejemplar)
    {
        return new EjemplarDto
        {
            IdEjemplar = ejemplar.IdEjemplar,
            CodigoBarras = ejemplar.CodigoBarras,
            NumeroInscripcion = ejemplar.NumeroInscripcion,

            IdMaterialBibliografico =
                ejemplar.IdMaterialBibliografico,

            MaterialBibliografico =
                ejemplar.MaterialBibliografico?.Titulo
                ?? string.Empty,

            Estado = ejemplar.Estado,
            Biblioteca = ejemplar.Biblioteca,
            Activo = ejemplar.Activo
        };
    }

    private static string? LimpiarTextoOpcional(
        string? texto)
    {
        return string.IsNullOrWhiteSpace(texto)
            ? null
            : texto.Trim();
    }
}