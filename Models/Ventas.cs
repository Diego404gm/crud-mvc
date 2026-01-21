using System.ComponentModel.DataAnnotations;

namespace CrudNet7MVC.Models
{
    public class Ventas
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        public decimal Monto { get; set; }
        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El IVA es obligatorio.")]
        public decimal IVA { get; set; }

        [Required(ErrorMessage = "El total es obligatorio.")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "La razón social es obligatoria.")]
        public string RazonSocial { get; set; }

        [Required(ErrorMessage = "El RFC es obligatorio.")]  
        public string RFC { get; set; }

        [Required(ErrorMessage = "El domicilio fiscal es obligatorio.")]
        public string DomicilioFiscal { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        public string CorreoElectronico { get; set; }

        [Required(ErrorMessage = "El nombre de contacto es obligatorio.")]
        public string NombreContacto { get; set; }

        [Required(ErrorMessage = "El teléfono de contacto es obligatorio.")]
        public string TelefonoContacto { get; set; }

        [Required(ErrorMessage = "El celular de contacto es obligatorio.")]
        public string CelularContacto { get; set; }

        [Required(ErrorMessage = "El email de contacto es obligatorio.")]
        public string EmailContacto { get; set; }
    }
}
