using Buildoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Buildoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Redirigir a la vista para usuarios autenticados
                return View("IndexAdmin");
            }
            else
            {
                // Redirigir a la vista para usuarios no autenticados
                return View("Index");
            }
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
