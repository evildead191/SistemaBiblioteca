using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaBiblioteca.Entities.Models;
using SistemaBiblioteca.Web.ViewModels.Account;

namespace SistemaBiblioteca.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(
        string? returnUrl = null,
        bool sesionExpirada = false)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(
                "Index",
                "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        ViewData["SesionExpirada"] = sesionExpirada;

        return View(
            new LoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model,
        string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        ApplicationUser? usuario =
            await _userManager.FindByEmailAsync(
                model.Correo);

        if (usuario is null)
        {
            ModelState.AddModelError(
                string.Empty,
                "Correo electrónico o contraseña incorrectos.");

            return View(model);
        }

        if (!usuario.Activo)
        {
            ModelState.AddModelError(
                string.Empty,
                "La cuenta se encuentra inactiva.");

            return View(model);
        }

        bool esAdministrador =
            await _userManager.IsInRoleAsync(
                usuario,
                "Administrador");

        if (!esAdministrador)
        {
            ModelState.AddModelError(
                string.Empty,
                "No tiene permisos para acceder al sistema.");

            return View(model);
        }

        Microsoft.AspNetCore.Identity.SignInResult resultado =
            await _signInManager.CheckPasswordSignInAsync(
                usuario,
                model.Contrasena,
                lockoutOnFailure: true);

        if (resultado.IsLockedOut)
        {
            ModelState.AddModelError(
                string.Empty,
                "La cuenta está bloqueada temporalmente por varios intentos fallidos.");

            return View(model);
        }

        if (!resultado.Succeeded)
        {
            ModelState.AddModelError(
                string.Empty,
                "Correo electrónico o contraseña incorrectos.");

            return View(model);
        }

        /*
         * Al iniciar una nueva sesión se cambia el SecurityStamp.
         * Esto invalida las sesiones anteriores de la misma cuenta.
         */
        IdentityResult resultadoSecurityStamp =
            await _userManager.UpdateSecurityStampAsync(
                usuario);

        if (!resultadoSecurityStamp.Succeeded)
        {
            ModelState.AddModelError(
                string.Empty,
                "No fue posible iniciar sesión. Intente nuevamente.");

            return View(model);
        }

        await _signInManager.SignInAsync(
            usuario,
            isPersistent: model.Recordarme);

        if (!string.IsNullOrWhiteSpace(returnUrl)
            && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction(
            "Index",
            "Home");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(
        string? motivo = null)
    {
        await _signInManager.SignOutAsync();

        if (motivo == "inactividad")
        {
            return RedirectToAction(
                nameof(Login),
                new
                {
                    sesionExpirada = true
                });
        }

        return RedirectToAction(
            nameof(Login));
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}