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
    public class NuevoModel : PageModel
    {
        private readonly IConfiguration configuration;
        private readonly ICrudApi<OrdenVentaViewModel, int> service;
        private readonly ICrudApi<InventarioViewModel, int> serviceP;
        private readonly ICrudApi<ClientesViewModel, int> serviceC;

        private readonly ICrudApi<ImpuestosViewModel, int> serviceI;


        [BindProperty]
        public string NombreVendedor { get; set; }

        [BindProperty]
        public OrdenVentaViewModel Orden { get; set; }
        [BindProperty]
        public InventarioViewModel[] Inventario { get; set; } 
        [BindProperty]
        public ClientesViewModel[] Clientes { get; set; }
        [BindProperty]
        public ImpuestosViewModel[] Impuestos { get; set; }

        public NuevoModel(ICrudApi<OrdenVentaViewModel, int> service, ICrudApi<InventarioViewModel, int> serviceP, ICrudApi<ClientesViewModel, int> serviceC, ICrudApi<ImpuestosViewModel, int> serviceI)
        {
            this.service = service;
            this.serviceP = serviceP;
            this.serviceC = serviceC;
            this.serviceI = serviceI;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
                if (string.IsNullOrEmpty(Roles.Where(a => a == "10").FirstOrDefault()))
                {
                    return RedirectToPage("/NoPermiso");
                }

                Inventario = await serviceP.ObtenerLista("");
                Clientes = await serviceC.ObtenerLista("");
                Impuestos = await serviceI.ObtenerLista("");

                Inventario = Inventario.OrderBy(a => a.Categoria).ToArray();

                NombreVendedor = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Vendedor").Select(s1 => s1.Value).FirstOrDefault(); 
                Orden = new OrdenVentaViewModel();
                Orden.CodVendedor = Convert.ToInt32(((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "CodVendedor").Select(s1 => s1.Value).FirstOrDefault());
                Orden.Fecha = DateTime.Now;
                Orden.FechaVencimiento = DateTime.Now;
                Orden.FechaEntrega = DateTime.Now;
                return Page();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                await service.Agregar(Orden);
                return RedirectToPage("./Index");
            }
            catch (ApiException ex)
            {
                Errores error = JsonConvert.DeserializeObject<Errores>(ex.Content.ToString());
                ModelState.AddModelError(string.Empty, error.Message);

                return Page();
            }
        }


        public async Task<IActionResult> OnPostGenerar(string recibidos)
        {
            string error = "";


            try
            {

                RecibidoC recibido = JsonConvert.DeserializeObject<RecibidoC>(recibidos);



                Orden = new OrdenVentaViewModel();
               
                Orden.Detalle = new Detalle[recibido.DetOrden.Length];
                Orden.CardCode = recibido.EncOrden.CardCode;
                Orden.Moneda = recibido.EncOrden.Moneda;
                Orden.Fecha = recibido.EncOrden.Fecha;
                Orden.FechaVencimiento = recibido.EncOrden.FechaVencimiento;
                Orden.FechaEntrega = recibido.EncOrden.FechaEntrega;
                Orden.TipoDocumento = recibido.EncOrden.TipoDocumento;
                Orden.NumAtCard = recibido.EncOrden.NumAtCard;
                Orden.Comentarios = recibido.EncOrden.Comentarios;
                Orden.CodVendedor = recibido.EncOrden.CodVendedor;
                 



                short cantidad = 1;

                foreach (var item in recibido.DetOrden)
                {
                    Orden.Detalle[cantidad - 1] = new Detalle();
                    Orden.Detalle[cantidad - 1].ItemCode = item.id.ToString();
                    Orden.Detalle[cantidad - 1].PorcentajeDescuento = item.descuento;
                    Orden.Detalle[cantidad - 1].Cantidad = item.cantidad;

                    Orden.Detalle[cantidad - 1].Impuesto = item.impuesto;
                    Orden.Detalle[cantidad - 1].TaxOnly = item.taxonly;
                    Orden.Detalle[cantidad - 1].PrecioUnitario = item.precioUnitario;



                    cantidad++;
                }


                error += " Antes de Agregar ";
                await service.Agregar(Orden);

                error += " DEspues de agregar";
                return new JsonResult(true);
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
