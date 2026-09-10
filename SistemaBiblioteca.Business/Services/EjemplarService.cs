using SistemaBiblioteca.Business.DTOs.Ejemplar;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.DataAccess.Repositories;
using SistemaBiblioteca.Entities.Enums;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.Business.Services;

public class EjemplarService : IEjemplarService
{
    private const string BibliotecaInstitucional =
        "Escuela Reverendo Francisco Schmitz";

    private const string PrefijoCodigoPredeterminado =
        "EJ";

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

    public async Task<EjemplarDto?> ObtenerPorIdAsync(
        int id)
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
            IdEjemplar =
                ejemplar.IdEjemplar,

            CodigoBarras =
                ejemplar.CodigoBarras,

            NumeroInscripcion =
                ejemplar.NumeroInscripcion,

            IdMaterialBibliografico =
                ejemplar.IdMaterialBibliografico,

            Estado =
                ejemplar.Estado,

            Biblioteca =
                ejemplar.Biblioteca,

            Activo =
                ejemplar.Activo
        };
    }

    public async Task<(bool Exitoso, string Mensaje)>
        CrearAsync(
            EjemplarCreateDto dto)
    {
        if (dto.Cantidad < 1 ||
            dto.Cantidad > 100)
        {
            return (
                false,
                "La cantidad debe estar entre 1 y 100 ejemplares."
            );
        }

        if (dto.Estado == EstadoEjemplar.Prestado)
        {
            return (
                false,
                "Un ejemplar no puede registrarse inicialmente como prestado."
            );
        }

        bool materialExistente =
            await _repository
                .ExisteMaterialBibliograficoAsync(
                    dto.IdMaterialBibliografico);

        if (!materialExistente)
        {
            return (
                false,
                "El material bibliográfico seleccionado no existe o se encuentra inactivo."
            );
        }

        if (dto.GenerarCodigosAutomaticamente)
        {
            return await CrearAutomaticamenteAsync(dto);
        }

        return await CrearManualAsync(dto);
    }

    public async Task<(bool Exitoso, string Mensaje)>
        EditarAsync(
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

        if (string.IsNullOrWhiteSpace(codigoBarras))
        {
            return (
                false,
                "Ingrese el código de barras."
            );
        }

        bool codigoExistente =
            await _repository
                .ExisteCodigoBarrasAsync(
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
            await _repository
                .ExisteMaterialBibliograficoAsync(
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

        ejemplar.CodigoBarras =
            codigoBarras;

        ejemplar.NumeroInscripcion =
            numeroInscripcion;

        ejemplar.IdMaterialBibliografico =
            dto.IdMaterialBibliografico;

        ejemplar.Estado =
            dto.Estado;

        ejemplar.Activo =
            dto.Activo;

        /*
         * Biblioteca NO se modifica aquí.
         *
         * Esto evita que al editar un ejemplar
         * se pierda el valor institucional y aparezca
         * nuevamente como "No indicada".
         */

        _repository.Actualizar(ejemplar);

        await _repository.GuardarCambiosAsync();

        return (
            true,
            "El ejemplar fue actualizado correctamente."
        );
    }

    public async Task<(string? UltimoCodigo,
        string SiguienteCodigo)>
        ObtenerInformacionCodigosAsync()
    {
        List<string> codigos =
            await _repository.ObtenerCodigosBarrasAsync();

        int ultimoNumero =
            ObtenerMayorNumero(
                codigos,
                PrefijoCodigoPredeterminado);

        string? ultimoCodigo =
            ultimoNumero > 0
                ? ConstruirCodigo(
                    PrefijoCodigoPredeterminado,
                    ultimoNumero)
                : null;

        string siguienteCodigo =
            ConstruirCodigo(
                PrefijoCodigoPredeterminado,
                ultimoNumero + 1);

        return (
            ultimoCodigo,
            siguienteCodigo
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
        CambiarEstadoAsync(
            int id)
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

        ejemplar.Activo =
            !ejemplar.Activo;

        _repository.Actualizar(ejemplar);

        await _repository.GuardarCambiosAsync();

        string mensaje =
            ejemplar.Activo
                ? "El ejemplar fue activado correctamente."
                : "El ejemplar fue desactivado correctamente.";

        return (
            true,
            mensaje
        );
    }

    private async Task<(bool Exitoso, string Mensaje)>
        CrearManualAsync(
            EjemplarCreateDto dto)
    {
        if (dto.Cantidad != 1)
        {
            return (
                false,
                "Para registrar varios ejemplares debe utilizar la generación automática de códigos."
            );
        }

        string codigoBarras =
            dto.CodigoBarras.Trim();

        if (string.IsNullOrWhiteSpace(codigoBarras))
        {
            return (
                false,
                "Ingrese el código de barras."
            );
        }

        bool codigoExistente =
            await _repository
                .ExisteCodigoBarrasAsync(
                    codigoBarras);

        if (codigoExistente)
        {
            return (
                false,
                "Ya existe un ejemplar con ese código de barras."
            );
        }

        Ejemplar ejemplar = new()
        {
            CodigoBarras =
                codigoBarras,

            NumeroInscripcion =
                LimpiarTextoOpcional(
                    dto.NumeroInscripcion),

            IdMaterialBibliografico =
                dto.IdMaterialBibliografico,

            Estado =
                dto.Estado,

            Biblioteca =
                BibliotecaInstitucional,

            Activo =
                true
        };

        await _repository.AgregarAsync(
            ejemplar);

        await _repository.GuardarCambiosAsync();

        return (
            true,
            "El ejemplar fue registrado correctamente."
        );
    }

    private async Task<(bool Exitoso, string Mensaje)>
        CrearAutomaticamenteAsync(
            EjemplarCreateDto dto)
    {
        string codigoBase =
            LimpiarCodigoBase(
                dto.CodigoBarras);

        if (string.IsNullOrWhiteSpace(codigoBase))
        {
            codigoBase =
                PrefijoCodigoPredeterminado;
        }

        List<string> codigosExistentes =
            await _repository
                .ObtenerCodigosBarrasAsync();

        int ultimoNumero =
            ObtenerMayorNumero(
                codigosExistentes,
                codigoBase);

        List<Ejemplar> ejemplares =
            new();

        for (int i = 1;
             i <= dto.Cantidad;
             i++)
        {
            int numero =
                ultimoNumero + i;

            string codigoGenerado =
                ConstruirCodigo(
                    codigoBase,
                    numero);

            Ejemplar ejemplar = new()
            {
                CodigoBarras =
                    codigoGenerado,

                NumeroInscripcion =
                    dto.Cantidad == 1
                        ? LimpiarTextoOpcional(
                            dto.NumeroInscripcion)
                        : null,

                IdMaterialBibliografico =
                    dto.IdMaterialBibliografico,

                Estado =
                    dto.Estado,

                Biblioteca =
                    BibliotecaInstitucional,

                Activo =
                    true
            };

            ejemplares.Add(ejemplar);
        }

        await _repository.AgregarRangoAsync(
            ejemplares);

        await _repository.GuardarCambiosAsync();

        if (dto.Cantidad == 1)
        {
            return (
                true,
                $"El ejemplar fue registrado correctamente con el código {ejemplares[0].CodigoBarras}."
            );
        }

        string codigoInicial =
            ejemplares.First().CodigoBarras;

        string codigoFinal =
            ejemplares.Last().CodigoBarras;

        return (
            true,
            $"Se registraron {dto.Cantidad} ejemplares correctamente. Códigos generados: {codigoInicial} al {codigoFinal}."
        );
    }

    private static int ObtenerMayorNumero(
        IEnumerable<string> codigos,
        string codigoBase)
    {
        string prefijo =
            $"{codigoBase}-";

        int mayorNumero = 0;

        foreach (string codigo in codigos)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                continue;
            }

            if (!codigo.StartsWith(
                    prefijo,
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string parteNumerica =
                codigo[prefijo.Length..];

            if (!int.TryParse(
                    parteNumerica,
                    out int numero))
            {
                continue;
            }

            if (numero > mayorNumero)
            {
                mayorNumero = numero;
            }
        }

        return mayorNumero;
    }

    private static string ConstruirCodigo(
        string codigoBase,
        int numero)
    {
        return $"{codigoBase}-{numero:D4}";
    }

    private static string LimpiarCodigoBase(
        string? codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            return string.Empty;
        }

        return codigo
            .Trim()
            .TrimEnd('-');
    }

    private static EjemplarDto MapearADto(
        Ejemplar ejemplar)
    {
        return new EjemplarDto
        {
            IdEjemplar =
                ejemplar.IdEjemplar,

            CodigoBarras =
                ejemplar.CodigoBarras,

            NumeroInscripcion =
                ejemplar.NumeroInscripcion,

            IdMaterialBibliografico =
                ejemplar.IdMaterialBibliografico,

            MaterialBibliografico =
                ejemplar.MaterialBibliografico?
                    .Titulo
                ?? string.Empty,

            Estado =
                ejemplar.Estado,

            Biblioteca =
                ejemplar.Biblioteca,

            Activo =
                ejemplar.Activo
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