using System.ComponentModel.DataAnnotations;

namespace CrudNet7MVC.Models
{
    public class Usuario
    {

        public int IdUsuario { get; set; }

        public string NombreCompleto { get; set; }

        public string Correo { get; set; }

        [MaxLength(256)]
        public string Clave { get; set; }

    }
}
