using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConectorEcommerce.Models
{
    public class RecibidoC
    {
        public EncOrden EncOrden { get; set; }
        
        public DetOrden[] DetOrden { get; set; }
    }
}
