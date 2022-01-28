using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConectorEcommerce.Models
{
    public class EncOrden
    {
        public string CardCode { get; set; }
        public string Moneda { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime FechaEntrega { get; set; }

        public string TipoDocumento { get; set; }

        public string NumAtCard { get; set; }

        public string Comentarios { get; set; }
        public int CodVendedor { get; set; }
    }
}
