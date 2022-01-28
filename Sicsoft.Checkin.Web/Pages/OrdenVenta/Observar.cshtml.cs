using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ConectorEcommerce.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;

namespace ConectorEcommerce.Pages.OrdenVenta
{
    public class ObservarModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<OrdenVentaViewModel, int> service;
        private readonly ICrudApi<InventarioViewModel, int> serviceP;
        private readonly ICrudApi<ClientesViewModel, int> serviceC;
        private readonly ICrudApi<UsuariosViewModel, int> serviceLogin;

        private readonly ICrudApi<ImpuestosViewModel, int> serviceI;

        [BindProperty]
        public string NombreVendedor { get; set; }

        [BindProperty]
        public OrdenVentaViewModel Orden { get; set; }
        [BindProperty]
        public InventarioViewModel[] Inventario { get; set; }
        [BindProperty]
        public UsuariosViewModel[] Usuarios { get; set; }
        [BindProperty]
        public ClientesViewModel[] Clientes { get; set; }
        [BindProperty]
        public ImpuestosViewModel[] Impuestos { get; set; }

        public ObservarModel(ICrudApi<OrdenVentaViewModel, int> service, ICrudApi<InventarioViewModel, int> serviceP, ICrudApi<ClientesViewModel, int> serviceC, ICrudApi<ImpuestosViewModel, int> serviceI, ICrudApi<UsuariosViewModel, int> serviceLogin)
        {
            this.service = service;
            this.serviceP = serviceP;
            this.serviceC = serviceC;
            this.serviceI = serviceI;
            this.serviceLogin = serviceLogin;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "12").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                Inventario = await serviceP.ObtenerLista("");
                Clientes = await serviceC.ObtenerLista("");
                Impuestos = await serviceI.ObtenerLista("");
                Usuarios = await serviceLogin.ObtenerLista("");
                Orden = await service.ObtenerPorId(id);
                var CodVendedor = Orden.CodVendedor.ToString();
                NombreVendedor = Usuarios.Where(a => a.CodigoVendedor == CodVendedor).FirstOrDefault().Nombre;
                return Page();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
