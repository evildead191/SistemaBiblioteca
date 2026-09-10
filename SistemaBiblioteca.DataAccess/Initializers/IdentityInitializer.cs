using Microsoft.AspNetCore.Identity;
using SistemaBiblioteca.Entities.Models;

namespace SistemaBiblioteca.DataAccess.Initializers;

public static class IdentityInitializer
{
    public const string RolAdministrador = "Administrador";

    public static async Task InicializarAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        string correoAdministrador,
        string contrasenaAdministrador)
    {
        if (!await roleManager.RoleExistsAsync(RolAdministrador))
        {
            IdentityResult resultadoRol =
                await roleManager.CreateAsync(
                    new IdentityRole(RolAdministrador));

            if (!resultadoRol.Succeeded)
            {
                throw new InvalidOperationException(
                    ConstruirMensajeError(
                        "No fue posible crear el rol Administrador.",
                        resultadoRol.Errors));
            }
        }

        ApplicationUser? administrador =
            await userManager.FindByEmailAsync(
                correoAdministrador);

        if (administrador is null)
        {
            administrador = new ApplicationUser
            {
                UserName = correoAdministrador,
                Email = correoAdministrador,
                EmailConfirmed = true,

                Nombre = "Administrador",
                PrimerApellido = "Sistema",
                SegundoApellido = null,

                Activo = true,
                FechaCreacion = DateTime.Now
            };

            IdentityResult resultadoUsuario =
                await userManager.CreateAsync(
                    administrador,
                    contrasenaAdministrador);

            if (!resultadoUsuario.Succeeded)
            {
                throw new InvalidOperationException(
                    ConstruirMensajeError(
                        "No fue posible crear el usuario administrador.",
                        resultadoUsuario.Errors));
            }
        }

        if (!await userManager.IsInRoleAsync(
                administrador,
                RolAdministrador))
        {
            IdentityResult resultadoAsignacion =
                await userManager.AddToRoleAsync(
                    administrador,
                    RolAdministrador);

            if (!resultadoAsignacion.Succeeded)
            {
                throw new InvalidOperationException(
                    ConstruirMensajeError(
                        "No fue posible asignar el rol Administrador.",
                        resultadoAsignacion.Errors));
            }
        }
    }

    private static string ConstruirMensajeError(
        string mensaje,
        IEnumerable<IdentityError> errores)
    {
        string detalle = string.Join(
            " | ",
            errores.Select(x => x.Description));

        return $"{mensaje} {detalle}";
    }
}