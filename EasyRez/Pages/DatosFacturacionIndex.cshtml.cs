using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EasyRez.Models;

namespace EasyRez.Pages
{
    public class DatosFacturacionIndexModel : PageModel
    {
        DatosFacturacionDataAccessLayer datosFacturacionDataAccessLayer = new DatosFacturacionDataAccessLayer();
        public List<DatosFacturacion> lstDatosFacturacion { get; set; }
        public Paginador<DatosFacturacion> lstDatosFacturacionActual;
        public void OnGet(int tipoEntidadTributaria = -1, int pagina = 1)
        {
            lstDatosFacturacion = datosFacturacionDataAccessLayer.GetAllDatosFacturacion(tipoEntidadTributaria).ToList();
            List<DatosFacturacion> tipo = lstDatosFacturacion.GroupBy(df => df.RFC).Select(gdf => gdf.First()).ToList();

            lstDatosFacturacionActual = new Paginador<DatosFacturacion>();
            lstDatosFacturacionActual.PaginaActual = pagina;
            lstDatosFacturacionActual.TipoEntidadTributaria = tipoEntidadTributaria;
            lstDatosFacturacionActual.TotalPaginas = tipo.Count();
            lstDatosFacturacionActual.Resultado = lstDatosFacturacion.Where(df => df.RFC == tipo[pagina -1].RFC).ToList();
        }
    }
}
