using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaBiblioteca.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auditorias",
                columns: table => new
                {
                    IdAuditoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioSistema = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Modulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Detalle = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditorias", x => x.IdAuditoria);
                });

            migrationBuilder.CreateTable(
                name: "IntegracionesHistorial",
                columns: table => new
                {
                    IdIntegracionHistorial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreArchivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FechaProceso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioEjecutor = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FilasProcesadas = table.Column<int>(type: "int", nullable: false),
                    FilasImportadas = table.Column<int>(type: "int", nullable: false),
                    FilasConError = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Detalle = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegracionesHistorial", x => x.IdIntegracionHistorial);
                });

            migrationBuilder.CreateTable(
                name: "MaterialesBibliograficos",
                columns: table => new
                {
                    IdMaterialBibliografico = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroFicha = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Clasificacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Autor = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AnioPublicacion = table.Column<int>(type: "int", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialesBibliograficos", x => x.IdMaterialBibliografico);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosBiblioteca",
                columns: table => new
                {
                    IdUsuarioBiblioteca = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identificacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TipoUsuario = table.Column<int>(type: "int", nullable: false),
                    SeccionODepartamento = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Correo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosBiblioteca", x => x.IdUsuarioBiblioteca);
                });

            migrationBuilder.CreateTable(
                name: "Ejemplares",
                columns: table => new
                {
                    IdEjemplar = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoBarras = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumeroInscripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IdMaterialBibliografico = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Biblioteca = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ejemplares", x => x.IdEjemplar);
                    table.ForeignKey(
                        name: "FK_Ejemplares_MaterialesBibliograficos_IdMaterialBibliografico",
                        column: x => x.IdMaterialBibliografico,
                        principalTable: "MaterialesBibliograficos",
                        principalColumn: "IdMaterialBibliografico",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prestamos",
                columns: table => new
                {
                    IdPrestamo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuarioBiblioteca = table.Column<int>(type: "int", nullable: false),
                    IdEjemplar = table.Column<int>(type: "int", nullable: false),
                    FechaPrestamo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaLimiteDevolucion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaDevolucionReal = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestamos", x => x.IdPrestamo);
                    table.ForeignKey(
                        name: "FK_Prestamos_Ejemplares_IdEjemplar",
                        column: x => x.IdEjemplar,
                        principalTable: "Ejemplares",
                        principalColumn: "IdEjemplar",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prestamos_UsuariosBiblioteca_IdUsuarioBiblioteca",
                        column: x => x.IdUsuarioBiblioteca,
                        principalTable: "UsuariosBiblioteca",
                        principalColumn: "IdUsuarioBiblioteca",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ejemplares_CodigoBarras",
                table: "Ejemplares",
                column: "CodigoBarras",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ejemplares_IdMaterialBibliografico",
                table: "Ejemplares",
                column: "IdMaterialBibliografico");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialesBibliograficos_NumeroFicha",
                table: "MaterialesBibliograficos",
                column: "NumeroFicha");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_IdEjemplar",
                table: "Prestamos",
                column: "IdEjemplar");

            migrationBuilder.CreateIndex(
                name: "IX_Prestamos_IdUsuarioBiblioteca",
                table: "Prestamos",
                column: "IdUsuarioBiblioteca");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosBiblioteca_Identificacion",
                table: "UsuariosBiblioteca",
                column: "Identificacion",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auditorias");

            migrationBuilder.DropTable(
                name: "IntegracionesHistorial");

            migrationBuilder.DropTable(
                name: "Prestamos");

            migrationBuilder.DropTable(
                name: "Ejemplares");

            migrationBuilder.DropTable(
                name: "UsuariosBiblioteca");

            migrationBuilder.DropTable(
                name: "MaterialesBibliograficos");
        }
    }
}
