using System.ComponentModel.DataAnnotations.Schema;
using SistemaBiblioteca.Entities.Enums;

namespace SistemaBiblioteca.Entities.Models;

public class Prestamo
{
    public int IdPrestamo { get; set; }

    public int IdUsuarioBiblioteca { get; set; }

    public int IdEjemplar { get; set; }

    public DateTime FechaPrestamo { get; set; }
        = DateTime.Now;

    public DateTime FechaLimiteDevolucion { get; set; }

    public DateTime? FechaDevolucionReal { get; set; }

    public EstadoPrestamo Estado { get; set; }
        = EstadoPrestamo.Activo;

    public string? Observaciones { get; set; }

    public UsuarioBiblioteca UsuarioBiblioteca { get; set; }
        = null!;

    public Ejemplar Ejemplar { get; set; }
        = null!;

    [NotMapped]
    public int DiasPrestado
    {
        get
        {
            DateTime fechaFinal = FechaDevolucionReal ?? DateTime.Now;

            return Math.Max(
                0,
                (fechaFinal.Date - FechaPrestamo.Date).Days
            );
        }
    }

    [NotMapped]
    public bool EstaAtrasado =>
        Estado == EstadoPrestamo.Activo &&
        DateTime.Today > FechaLimiteDevolucion.Date;

    [NotMapped]
    public int DiasAtraso =>
        EstaAtrasado
            ? (DateTime.Today - FechaLimiteDevolucion.Date).Days
            : 0;
}