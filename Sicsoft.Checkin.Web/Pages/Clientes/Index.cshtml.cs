using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using ConectorEcommerce.Models;
using InversionGloblalWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Refit;
using Sicsoft.Checkin.Web.Servicios;

namespace ConectorEcommerce.Pages.Clientes
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<ClientesViewModel, int> cliente;

        [BindProperty]
        public ClientesViewModel[] Cliente { get; set; }


        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        public IndexModel(ICrudApi<ClientesViewModel, int> cliente)
        {
            this.cliente = cliente;

        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "7").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }




                Cliente = await cliente.ObtenerLista(filtro);



                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }


        public async Task<IActionResult> OnGetActualizaClienteAsync(string id)
        {
            try
            {
                filtro = new ParametrosFiltros();
                filtro.CardCode = id;
                await cliente.ActualizarCliente(filtro);


                var resp = new
                {
                    respuesta = 1,
                    msj = "Actualizado Exitosamente"
                };



                return new JsonResult(resp);
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);



                var resp = new
                {
                    respuesta = 0,
                    msj = error.Message
                };
                return new JsonResult(resp);
            }
        }
    }
}
