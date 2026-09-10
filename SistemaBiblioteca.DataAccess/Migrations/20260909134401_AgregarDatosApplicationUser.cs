using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaBiblioteca.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDatosApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrimerApellido",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SegundoApellido",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql(
    "UPDATE AspNetUsers " +
    "SET Activo = 1, " +
    "FechaCreacion = GETDATE(), " +
    "Nombre = 'Administrador', " +
    "PrimerApellido = 'Sistema' " +
    "WHERE Email = 'admin@biblioteca.local';");
        }

        
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PrimerApellido",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SegundoApellido",
                table: "AspNetUsers");
        }
    }
}
