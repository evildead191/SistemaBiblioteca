using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.Business.Services;
using SistemaBiblioteca.DataAccess.Context;
using SistemaBiblioteca.DataAccess.Initializers;
using SistemaBiblioteca.DataAccess.Repositories;
using SistemaBiblioteca.Entities.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("SistemaBibliotecaConnection")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión SistemaBibliotecaConnection.");

builder.Services.AddDbContext<SistemaBibliotecaDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;

        options.User.RequireUniqueEmail = true;

        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;

        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(5);

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<SistemaBibliotecaDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";

    options.Cookie.HttpOnly = true;
    options.Cookie.Name = "SistemaBiblioteca.Auth";

    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    options.SlidingExpiration = true;

    options.Cookie.SecurePolicy =
        CookieSecurePolicy.Always;

    options.Cookie.SameSite =
        SameSiteMode.Lax;
});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

AuthorizationPolicy politicaAdministrador =
    new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole("Administrador")
        .Build();

builder.Services
    .AddAuthorizationBuilder()
    .SetDefaultPolicy(politicaAdministrador)
    .SetFallbackPolicy(politicaAdministrador);

builder.Services.AddScoped<
    IMaterialBibliograficoRepository,
    MaterialBibliograficoRepository>();

builder.Services.AddScoped<
    IMaterialBibliograficoService,
    MaterialBibliograficoService>();

builder.Services.AddScoped<
    IEjemplarRepository,
    EjemplarRepository>();

builder.Services.AddScoped<
    IEjemplarService,
    EjemplarService>();

builder.Services.AddScoped<
    IUsuarioBibliotecaRepository,
    UsuarioBibliotecaRepository>();

builder.Services.AddScoped<
    IUsuarioBibliotecaService,
    UsuarioBibliotecaService>();

builder.Services.AddScoped<
    IPrestamoRepository,
    PrestamoRepository>();

builder.Services.AddScoped<
    IPrestamoService,
    PrestamoService>();

builder.Services.AddScoped<
    IReporteRepository,
    ReporteRepository>();

builder.Services.AddScoped<
    IReporteService,
    ReporteService>();

builder.Services.AddScoped<
    IReporteExcelService,
    ReporteExcelService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;

    SistemaBibliotecaDbContext context =
        services.GetRequiredService<SistemaBibliotecaDbContext>();

    await context.Database.MigrateAsync();

    RoleManager<IdentityRole> roleManager =
        services.GetRequiredService<RoleManager<IdentityRole>>();

    UserManager<ApplicationUser> userManager =
        services.GetRequiredService<UserManager<ApplicationUser>>();

    string correoAdministrador =
        app.Configuration["Administrador:Correo"]
        ?? throw new InvalidOperationException(
            "No se configuró el correo del administrador.");

    string contrasenaAdministrador =
        app.Configuration["Administrador:Contrasena"]
        ?? throw new InvalidOperationException(
            "No se configuró la contraseña del administrador.");

    await IdentityInitializer.InicializarAsync(
        roleManager,
        userManager,
        correoAdministrador,
        contrasenaAdministrador);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

/*
 * IMPORTANTE:
 * Los archivos estáticos deben quedar disponibles
 * incluso cuando el usuario todavía no inició sesión.
 */
app.UseStaticFiles();

app.MapStaticAssets()
    .AllowAnonymous();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();