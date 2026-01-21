using Microsoft.AspNetCore.Mvc;
using CrudNet7MVC.Datos;
using CrudNet7MVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using CrudNet7MVC.ViewModels;
using Microsoft.IdentityModel.Protocols.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography;
using System.Text;

namespace CrudNet7MVC.Controllers
{
    public class AccesoController : Controller
    {
        private readonly ApplicationDbContext _contexto;

        public AccesoController(ApplicationDbContext contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public IActionResult Registrarse()
        {
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrarse(UsuarioVM modelo)
        {
            if (modelo.Clave != modelo.ConfirmarClave)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View(modelo);
            }


            string claveEncriptada = EncriptarClave(modelo.Clave);

            Usuario usuario = new Usuario()
            {
                NombreCompleto = modelo.NombreCompleto,
                Correo = modelo.Correo,
                Clave = claveEncriptada
            };

            try
            {
                await _contexto.Usuarios.AddAsync(usuario);
                await _contexto.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ViewData["Mensaje"] = "Ocurrió un error al registrar el usuario: " + ex.InnerException?.Message;
                return View(modelo);
            }

            if (usuario.IdUsuario != 0)
            {
                Contacto contacto = new Contacto()
                {
                    Nombre = usuario.NombreCompleto,
                    Telefono = "",
                    Celular = "",
                    Email = usuario.Correo,
                    FechaCreacion = DateTime.Now
                };

                try
                {
                    await _contexto.Contacto.AddAsync(contacto);
                    await _contexto.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    ViewData["Mensaje"] = "Ocurrió un error al crear el contacto: " + ex.InnerException?.Message;
                    return View(modelo);
                }

                return RedirectToAction("Login", "Acceso");
            }

            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View(modelo);
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated) return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM modelo)
        {
            string claveEncriptada = EncriptarClave(modelo.Clave);

            Usuario? usuario_encontrado = await _contexto.Usuarios
                .Where(u =>
                    u.Correo == modelo.Correo &&
                    u.Clave == claveEncriptada
                )
                .FirstOrDefaultAsync();

            if (usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.NombreCompleto)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            await SincronizarUsuarioConContacto(usuario_encontrado);

            return RedirectToAction("Index", "Home");
        }

        private async Task SincronizarUsuarioConContacto(Usuario usuario)
        {
            var contactoExistente = await _contexto.Contacto.FirstOrDefaultAsync(c => c.Email == usuario.Correo);

            if (contactoExistente == null)
            {
                Contacto nuevoContacto = new Contacto()
                {
                    Nombre = usuario.NombreCompleto,
                    Email = usuario.Correo,
                    Telefono = "",
                    Celular = "",
                    FechaCreacion = DateTime.Now
                };

                await _contexto.Contacto.AddAsync(nuevoContacto);
                await _contexto.SaveChangesAsync();
            }
        }


        private string EncriptarClave(string clave)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(clave));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
