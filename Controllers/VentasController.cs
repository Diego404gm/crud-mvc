using Microsoft.AspNetCore.Mvc;
using CrudNet7MVC.Models;
using System.Threading.Tasks;
using CrudNet7MVC.Datos;
using CrudNet7MVC.Dtos;
using System.ComponentModel.DataAnnotations;

namespace CrudNet7MVC.Controllers
{
    public class VentasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Wizard()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GuardarDatosWizard([FromBody] Ventas datos)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var venta = new Ventas
                    {
                        Monto = datos.Monto,
                        Descripcion = datos.Descripcion,
                        IVA = datos.IVA,
                        Total = datos.Total,
                        RazonSocial = datos.RazonSocial,
                        RFC = datos.RFC,
                        DomicilioFiscal = datos.DomicilioFiscal,
                        Telefono = datos.Telefono,
                        CorreoElectronico = datos.CorreoElectronico,
                        NombreContacto = datos.NombreContacto,
                        TelefonoContacto = datos.TelefonoContacto,
                        CelularContacto = datos.CelularContacto,
                        EmailContacto = datos.EmailContacto
                    };

                    _context.Ventas.Add(venta);
                    _context.SaveChanges();

                    return Ok(new { message = "Datos guardados con exito" });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Error al guardar los datos", details = ex.Message });
                }
            }
            return BadRequest(ModelState);
        }
        public IActionResult Resumen(int id)
        {
           
            var venta = _context.Ventas.FirstOrDefault(v => v.Id == id);

            if (venta == null)
            {
                return NotFound();
            }

            return View(venta); 
        }
      
        public IActionResult InicioResumen()
        {
            var ventas = _context.Ventas.ToList();  
            return View(ventas);  
        }

        public IActionResult EditarInformacion(int id)
        {
          
            var venta = _context.Ventas.FirstOrDefault(v => v.Id == id);
            if (venta == null)
            {
                return NotFound();  
            }

            return View("EditarInformacion", venta);  
        }

        [HttpPost]
        public IActionResult EditarInformacion([FromBody] Ventas venta)
        {
            if (venta == null || venta.Id <= 0)
            {
                return Json(new { success = false, message = "Datos invalidos." });
            }

            var ventaExistente = _context.Ventas.FirstOrDefault(v => v.Id == venta.Id);
            if (ventaExistente == null)
            {
                return Json(new { success = false, message = "no se encontro." });
            }

            try
            {
                ventaExistente.Monto = venta.Monto;
                ventaExistente.Descripcion = venta.Descripcion;
                ventaExistente.IVA = venta.IVA;
                ventaExistente.Total = venta.Total;
                ventaExistente.RazonSocial = venta.RazonSocial;
                ventaExistente.RFC = venta.RFC;
                ventaExistente.DomicilioFiscal = venta.DomicilioFiscal;
                ventaExistente.Telefono = venta.Telefono;
                ventaExistente.NombreContacto = venta.NombreContacto;
                ventaExistente.TelefonoContacto = venta.TelefonoContacto;
                ventaExistente.CelularContacto = venta.CelularContacto;
                ventaExistente.EmailContacto = venta.EmailContacto;

                _context.SaveChanges();
                return Json(new { success = true, message = "Datos actualizados.", redirectUrl = "/Ventas/InicioResumen" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            try
            {
                var venta = _context.Ventas.FirstOrDefault(v => v.Id == id);
                if (venta == null)
                {
                    return NotFound();  
                }

                _context.Ventas.Remove(venta);
                _context.SaveChanges();

                return RedirectToAction("InicioResumen");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { message = ex.Message });
            }
        }


    }
}

