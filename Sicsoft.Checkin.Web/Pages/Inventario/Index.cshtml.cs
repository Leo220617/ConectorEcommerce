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

namespace ConectorEcommerce.Pages.Inventario
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<InventarioViewModel, int> inventario;

        [BindProperty]
        public InventarioViewModel[] Inventario { get; set; }


        [BindProperty]
        public List<string> Categorias { get; set; } 

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        public IndexModel(ICrudApi<InventarioViewModel, int> inventario)
        {
            this.inventario = inventario;
            
        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "6").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }


                Inventario = await inventario.ObtenerLista(filtro);
                if(!string.IsNullOrEmpty(filtro.Categoria))
                {
                     var Inv = await inventario.ObtenerLista("");

                 Categorias = Inv.Select(a => a.Categoria).Distinct().ToList();
                }
                else
                {
                    Categorias = Inventario.Select(a => a.Categoria).Distinct().ToList();

                }



                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }


        public async Task<IActionResult> OnGetActualizaInventarioAsync(string id, string lp)
        {
            try
            {

                await inventario.ActualizarInventario(id, lp);


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
