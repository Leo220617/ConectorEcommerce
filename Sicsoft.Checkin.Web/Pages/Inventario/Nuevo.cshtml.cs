using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ConectorEcommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sicsoft.Checkin.Web.Servicios;

namespace ConectorEcommerce.Pages.Inventario
{
    public class NuevoModel : PageModel
    {
        private readonly ICrudApi<InventarioViewModel, int> items;
        private readonly ICrudApi<CategoriaViewModel, int> cats;
        private readonly ICrudApi<ListaPrecioViewModel, int> lp;


        [BindProperty]
        public InventarioViewModel Input { get; set; }
        [BindProperty]
        public ListaPrecioViewModel[] ListaP { get; set; }

        [BindProperty]
        public CategoriaViewModel[] Categorias { get; set; }

        public NuevoModel(ICrudApi<InventarioViewModel, int> items, ICrudApi<CategoriaViewModel, int> cats, ICrudApi<ListaPrecioViewModel, int> lp)
        {
            this.items = items;
            this.cats = cats;
            this.lp = lp;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
            if (string.IsNullOrEmpty(Roles.Where(a => a == "14").FirstOrDefault()))
            {
                return RedirectToPage("/NoPermiso");
            }

            Categorias = await cats.ObtenerLista("");
            ListaP = await lp.ObtenerLista("");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                
                await items.Agregar(Input);
                return Redirect("./Index");
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
                //return Redirect("/Error");
                return Page();
            }
        }
    }
}
