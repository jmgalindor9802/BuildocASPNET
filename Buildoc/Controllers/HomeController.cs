using Buildoc.Data;
using Buildoc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Buildoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
		private readonly UserManager<Usuario> _userManager;
		public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _logger = logger;
            _context = context;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
            if (User.Identity.IsAuthenticated)
            {

				var usuario = await _userManager.GetUserAsync(User);
				ViewBag.NombreUsuario = usuario?.Nombres;

				var inspeccionesPendientes = _context.Inspecciones
					   .Where(i => i.Proyecto.CoordinadorId == usuario.Id && i.Estado == EstadoInspeccion.PendientesDeRevision)
					   .ToList();

				ViewBag.NumeroInspeccionesPendientes = inspeccionesPendientes.Count;
				ViewBag.NombreUsuario = usuario.NombreCompleto;

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
