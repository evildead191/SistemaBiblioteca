using Microsoft.EntityFrameworkCore;
using SistemaBiblioteca.DataAccess.Context;
using SistemaBiblioteca.Business.Interfaces;
using SistemaBiblioteca.Business.Services;
using SistemaBiblioteca.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("SistemaBibliotecaConnection")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión SistemaBibliotecaConnection.");

builder.Services.AddDbContext<SistemaBibliotecaDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<
    IMaterialBibliograficoRepository,
    MaterialBibliograficoRepository>();

builder.Services.AddScoped<
    IMaterialBibliograficoService,
    MaterialBibliograficoService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
