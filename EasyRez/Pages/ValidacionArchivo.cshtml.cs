using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EasyRez.Pages
{
    public class ValidacionArchivoModel : PageModel
    {
        private readonly ILogger<ValidacionArchivoModel> _logger;

        public ValidacionArchivoModel(ILogger<ValidacionArchivoModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}