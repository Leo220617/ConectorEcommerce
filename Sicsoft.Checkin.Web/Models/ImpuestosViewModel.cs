using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConectorEcommerce.Models
{
    public class ImpuestosViewModel
    {
        public int id { get; set; }

        public int idSAP { get; set; }

        [StringLength(50)]
        public string Nombre { get; set; }
        public decimal Valor { get; set; }
    }
}
