using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ConectorEcommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sicsoft.Checkin.Web.Servicios;

namespace ConectorEcommerce.Pages.Clientes
{
    public class NuevoModel : PageModel
    {
        private readonly ICrudApi<ClientesViewModel, int> cliente;

        [BindProperty]
        public ClientesViewModel Input { get; set; }

        public NuevoModel(ICrudApi<ClientesViewModel, int> cliente)
        {
            this.cliente = cliente;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var Roles = ((ClaimsIdentity)User.Identity).Claims.Where(d => d.Type == "Roles").Select(s1 => s1.Value).FirstOrDefault().Split("|");
            if (string.IsNullOrEmpty(Roles.Where(a => a == "13").FirstOrDefault()))
            {
                return RedirectToPage("/NoPermiso");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                Input.CardCode = Input.CardName;
                await cliente.Agregar(Input);
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
