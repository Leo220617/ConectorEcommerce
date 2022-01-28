using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConectorEcommerce.Models
{
    public class DetOrden
    {

        public int id { get; set; }
        public string codigo { get; set; }
        public string nombre { get; set; }
        public decimal cantidad { get; set; }
        public decimal descuento { get; set; }
        public string impuesto { get; set; }

        public decimal precioUnitario { get; set; }
        public string comentario { get; set; }
        public bool taxonly { get; set; }
        public decimal total { get; set; }
    }
}
