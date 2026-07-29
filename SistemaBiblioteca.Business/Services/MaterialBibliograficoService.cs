using SistemaBiblioteca.Business.DTOs.MaterialBibliografico;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.DataAccess.Repositories;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.Business.Services;

public class MaterialBibliograficoService
    : IMaterialBibliograficoService
{
    private readonly IMaterialBibliograficoRepository _repository;

    public MaterialBibliograficoService(
        IMaterialBibliograficoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MaterialBibliograficoDto>> ObtenerTodosAsync(
        string? busqueda = null)
    {
        List<MaterialBibliograficoListadoResult> resultados =
            await _repository.ObtenerTodosAsync(busqueda);

        return resultados
            .Select(x => MapearADto(
                x.Material,
                x.CantidadEjemplares))
            .ToList();
    }

    public async Task<MaterialBibliograficoDto?> ObtenerPorIdAsync(int id)
    {
        MaterialBibliografico? material =
            await _repository.ObtenerPorIdAsync(id);

        return material is null
            ? null
            : MapearADto(material);
    }

    public async Task<int> ContarAsync()
    {
        return await _repository.ContarAsync();
    }
    public async Task<MaterialBibliograficoEditDto?> ObtenerParaEditarAsync(
        int id)
    {
        MaterialBibliografico? material =
            await _repository.ObtenerPorIdAsync(id);

        if (material is null)
        {
            return null;
        }

        return new MaterialBibliograficoEditDto
        {
            IdMaterialBibliografico =
                material.IdMaterialBibliografico,

            NumeroFicha = material.NumeroFicha,
            Clasificacion = material.Clasificacion,
            Autor = material.Autor,
            Titulo = material.Titulo,
            AnioPublicacion = material.AnioPublicacion,
            Activo = material.Activo
        };
    }

    public async Task<(bool Exitoso, string Mensaje)> CrearAsync(
        MaterialBibliograficoCreateDto dto)
    {
        string numeroFicha = dto.NumeroFicha.Trim();

        bool fichaExistente =
            await _repository.ExisteNumeroFichaAsync(numeroFicha);

        if (fichaExistente)
        {
            return (
                false,
                "Ya existe un material bibliográfico con ese número de ficha."
            );
        }

        MaterialBibliografico material = new()
        {
            NumeroFicha = numeroFicha,
            Clasificacion = dto.Clasificacion.Trim(),
            Autor = dto.Autor.Trim(),
            Titulo = dto.Titulo.Trim(),
            AnioPublicacion = dto.AnioPublicacion,
            Activo = true
        };

        await _repository.AgregarAsync(material);
        await _repository.GuardarCambiosAsync();

        return (
            true,
            "El material bibliográfico fue registrado correctamente."
        );
    }

    public async Task<(bool Exitoso, string Mensaje)> EditarAsync(
        MaterialBibliograficoEditDto dto)
    {
        MaterialBibliografico? material =
            await _repository.ObtenerPorIdAsync(
                dto.IdMaterialBibliografico);

        if (material is null)
        {
            return (
                false,
                "No se encontró el material bibliográfico."
            );
        }

        string numeroFicha = dto.NumeroFicha.Trim();
        string clasificacion = dto.Clasificacion.Trim();
        string autor = dto.Autor.Trim();
        string titulo = dto.Titulo.Trim();

        bool fichaExistente =
            await _repository.ExisteNumeroFichaAsync(
                numeroFicha,
                dto.IdMaterialBibliografico);

        if (fichaExistente)
        {
            return (
                false,
                "Ya existe otro material con ese número de ficha."
            );
        }

        bool huboCambios =
            material.NumeroFicha != numeroFicha ||
            material.Clasificacion != clasificacion ||
            material.Autor != autor ||
            material.Titulo != titulo ||
            material.AnioPublicacion != dto.AnioPublicacion ||
            material.Activo != dto.Activo;

        if (!huboCambios)
        {
            return (
                false,
                "No se realizaron modificaciones."
            );
        }

        material.NumeroFicha = numeroFicha;
        material.Clasificacion = clasificacion;
        material.Autor = autor;
        material.Titulo = titulo;
        material.AnioPublicacion = dto.AnioPublicacion;
        material.Activo = dto.Activo;

        _repository.Actualizar(material);
        await _repository.GuardarCambiosAsync();

        return (
            true,
            "El material bibliográfico fue actualizado correctamente."
        );
    }

    public async Task<(bool Exitoso, string Mensaje)>
        CambiarEstadoAsync(int id)
    {
        MaterialBibliografico? material =
            await _repository.ObtenerPorIdAsync(id);

        if (material is null)
        {
            return (
                false,
                "No se encontró el material bibliográfico."
            );
        }

        material.Activo = !material.Activo;

        _repository.Actualizar(material);
        await _repository.GuardarCambiosAsync();

        string mensaje = material.Activo
            ? "El material fue activado correctamente."
            : "El material fue desactivado correctamente.";

        return (true, mensaje);
    }

    private static MaterialBibliograficoDto MapearADto(
        MaterialBibliografico material,
        int cantidadEjemplares = 0)
    {
        return new MaterialBibliograficoDto
        {
            IdMaterialBibliografico =
                material.IdMaterialBibliografico,

            NumeroFicha = material.NumeroFicha,
            Clasificacion = material.Clasificacion,
            Autor = material.Autor,
            Titulo = material.Titulo,
            AnioPublicacion = material.AnioPublicacion,
            Activo = material.Activo,
            CantidadEjemplares = cantidadEjemplares
        };
    }
}