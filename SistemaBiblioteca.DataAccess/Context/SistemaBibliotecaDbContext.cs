using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.DataAccess.Context;

public class SistemaBibliotecaDbContext : DbContext
{
    public SistemaBibliotecaDbContext(
        DbContextOptions<SistemaBibliotecaDbContext> options)
        : base(options)
    {
    }

    public DbSet<MaterialBibliografico> MaterialesBibliograficos { get; set; }

    public DbSet<Ejemplar> Ejemplares { get; set; }

    public DbSet<UsuarioBiblioteca> UsuariosBiblioteca { get; set; }

    public DbSet<Prestamo> Prestamos { get; set; }

    public DbSet<IntegracionHistorial> IntegracionesHistorial { get; set; }

    public DbSet<Auditoria> Auditorias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigurarMaterialBibliografico(modelBuilder);
        ConfigurarEjemplar(modelBuilder);
        ConfigurarUsuarioBiblioteca(modelBuilder);
        ConfigurarPrestamo(modelBuilder);
        ConfigurarIntegracionHistorial(modelBuilder);
        ConfigurarAuditoria(modelBuilder);
    }

    private static void ConfigurarMaterialBibliografico(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MaterialBibliografico>(entity =>
        {
            entity.HasKey(x => x.IdMaterialBibliografico);

            entity.Property(x => x.NumeroFicha)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Clasificacion)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Autor)
                .HasMaxLength(250)
                .IsRequired();

            entity.Property(x => x.Titulo)
                .HasMaxLength(500)
                .IsRequired();

            entity.HasIndex(x => x.NumeroFicha);
        });
    }

    private static void ConfigurarEjemplar(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ejemplar>(entity =>
        {
            entity.HasKey(x => x.IdEjemplar);

            entity.Property(x => x.CodigoBarras)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.NumeroInscripcion)
                .HasMaxLength(100);

            entity.Property(x => x.Biblioteca)
                .HasMaxLength(250);

            entity.HasIndex(x => x.CodigoBarras)
                .IsUnique();

            entity.HasOne(x => x.MaterialBibliografico)
                .WithMany(x => x.Ejemplares)
                .HasForeignKey(x => x.IdMaterialBibliografico)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurarUsuarioBiblioteca(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UsuarioBiblioteca>(entity =>
        {
            entity.HasKey(x => x.IdUsuarioBiblioteca);

            entity.Property(x => x.Identificacion)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.NombreCompleto)
                .HasMaxLength(250)
                .IsRequired();

            entity.Property(x => x.SeccionODepartamento)
                .HasMaxLength(150);

            entity.Property(x => x.Correo)
                .HasMaxLength(250);

            entity.Property(x => x.Telefono)
                .HasMaxLength(50);

            entity.HasIndex(x => x.Identificacion)
                .IsUnique();
        });
    }

    private static void ConfigurarPrestamo(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.HasKey(x => x.IdPrestamo);

            entity.Property(x => x.Observaciones)
                .HasMaxLength(1000);

            entity.HasOne(x => x.UsuarioBiblioteca)
                .WithMany(x => x.Prestamos)
                .HasForeignKey(x => x.IdUsuarioBiblioteca)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Ejemplar)
                .WithMany(x => x.Prestamos)
                .HasForeignKey(x => x.IdEjemplar)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurarIntegracionHistorial(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IntegracionHistorial>(entity =>
        {
            entity.HasKey(x => x.IdIntegracionHistorial);

            entity.Property(x => x.NombreArchivo)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(x => x.UsuarioEjecutor)
                .HasMaxLength(250)
                .IsRequired();

            entity.Property(x => x.Estado)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Detalle)
                .HasMaxLength(2000);
        });
    }

    private static void ConfigurarAuditoria(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auditoria>(entity =>
        {
            entity.HasKey(x => x.IdAuditoria);

            entity.Property(x => x.UsuarioSistema)
                .HasMaxLength(250)
                .IsRequired();

            entity.Property(x => x.Accion)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Modulo)
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(x => x.Detalle)
                .HasMaxLength(2000);
        });
    }
}