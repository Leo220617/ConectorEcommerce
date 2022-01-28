﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConectorEcommerce.Models
{
    public class BitacoraViewModel
    {
        public int id { get; set; }

        public string Descripcion { get; set; }

        public string StackTrace { get; set; }

        public DateTime? Fecha { get; set; }
    }
}
