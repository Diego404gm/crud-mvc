using CrudNet7MVC.Datos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

//  Configuración de la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));

//  Configuración de autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acceso/Login";          // si no está logueado, lo manda aquí
        options.LogoutPath = "/Acceso/Logout";        // opcional
        options.AccessDeniedPath = "/Home/Error";     // si no tiene permisos
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // duración de la cookie
        options.SlidingExpiration = true;             // renueva si el usuario sigue activo
    });

builder.Services.AddAuthorization();

//  MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

//  Manejo de errores
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

//  Middleware
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//  Ruta por defecto (login al inicio)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}");

app.Run();
