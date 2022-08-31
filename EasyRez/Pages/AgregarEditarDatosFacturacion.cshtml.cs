using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EasyRez.Models;

namespace EasyRez.Pages
{
    public class AgregarEditarDatosFacturacionModel : PageModel
    {
        DatosFacturacionDataAccessLayer datosFacturacionDataAccessLayer = new DatosFacturacionDataAccessLayer();
        [BindProperty]
        public DatosFacturacion? datosFacturacion { get; set; }

        public ActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            datosFacturacionDataAccessLayer.AgregarEditarDatosFacturacion(datosFacturacion);

            return RedirectToPage("./DatosFacturacionIndex");
        }

        public ActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return Page();
            }
            datosFacturacion = datosFacturacionDataAccessLayer.ObtenerDatosFacturacionData(id);

            if (datosFacturacion == null)
            {
                return NotFound();
            }
            return Page();
        }


    }
}
