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

namespace ConectorEcommerce.Pages.OrdenVenta
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<OrdenVentaViewModel, int> ov;
        private readonly ICrudApi<ImpuestosViewModel, int> imp;


        [BindProperty]
        public OrdenVentaViewModel[] Ordenes { get; set; }


        [BindProperty]
        public ImpuestosViewModel[] Impuestos { get; set; }

        [BindProperty(SupportsGet = true)]
        public ParametrosFiltros filtro { get; set; }

        public IndexModel(ICrudApi<OrdenVentaViewModel, int> ov, ICrudApi<ImpuestosViewModel, int> imp)
        {
            this.ov = ov;
            this.imp = imp;

        }
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles1 = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles1.Where(a => a == "9").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                Impuestos = await imp.ObtenerLista("");

                DateTime time = new DateTime();

                if (time == filtro.FechaInicial)
                {


                    filtro.FechaInicial = DateTime.Now;

                    filtro.FechaInicial = new DateTime(filtro.FechaInicial.Year, filtro.FechaInicial.Month, 1);


                    DateTime primerDia = new DateTime(filtro.FechaInicial.Year, filtro.FechaInicial.Month, 1);


                    DateTime ultimoDia = primerDia.AddMonths(1).AddDays(-1);

                    filtro.FechaFinal = ultimoDia;



                }
                Ordenes = await ov.ObtenerLista(filtro);



                return Page();
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }



        public async Task<IActionResult> OnPostGenerar(int id)
        {
            string error = "";


            try
            {

              



               var Orden = new OrdenVentaViewModel();

                Orden.id = id;
              


                error += " Antes de Agregar ";
                var resp = await ov.Agregar(Orden);

                error += " DEspues de agregar"; 
                if(resp.ProcesadaSAP)
                {
                    return new JsonResult(true);

                }
                else
                {
                    return new JsonResult(false);

                }
            }
            catch (ApiException ex)
            {
                Errores errores = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, errores.Message);
                return new JsonResult(error);
                //return new JsonResult(false);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);

                return new JsonResult(false);
            }
        }
    }
}
