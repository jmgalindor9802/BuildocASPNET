using Buildoc.Data;
using Buildoc.Models;
using Buildoc.Models.Proyectos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static Buildoc.Models.Proyecto;
using Buildoc.ViewModels;
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

                // Verificar si el usuario es Administrador, Coordinador o Residente
                if (await _userManager.IsInRoleAsync(usuario, "Administrador"))
                {
                    // Para el Administrador, sin restricciones, mostrar todos los proyectos
                    var proyectosEnCurso = _context.Proyectos
                        .Select(p => new ProyectoHomeViewModel
                        {
                            ProyectoNombre = p.Nombre,
                            NumeroInspeccionesProgramadas = p.Inspecciones.Count(i => i.Estado == EstadoInspeccion.Programada),
                            NumeroInspeccionesPendientes = p.Inspecciones.Count(i => i.Estado == EstadoInspeccion.PendientesDeRevision),
                            NumeroIncidentesActivos = p.Incidentes.Count(i => i.Estado == EstadoIncidenteEnum.Activo)
                        })
                        .ToList();

                    return View("IndexAdmin", proyectosEnCurso);
                }
                else if (await _userManager.IsInRoleAsync(usuario, "Coordinador"))
                {
                    // Para el Coordinador, mostrar los proyectos donde es coordinador
                    var proyectosEnCurso = _context.Proyectos
                        .Where(p => p.CoordinadorId == usuario.Id)
                        .Select(p => new ProyectoHomeViewModel
                        {
                            ProyectoNombre = p.Nombre,
                            NumeroInspeccionesProgramadas = p.Inspecciones.Count(i => i.Estado == EstadoInspeccion.Programada),
                            NumeroInspeccionesPendientes = p.Inspecciones.Count(i => i.Estado == EstadoInspeccion.PendientesDeRevision),
                            NumeroIncidentesActivos = p.Incidentes.Count(i => i.Estado == EstadoIncidenteEnum.Activo)
                        })
                        .ToList();

                    return View("IndexAdmin", proyectosEnCurso);
                }
                else if (await _userManager.IsInRoleAsync(usuario, "Residente"))
                {
                    // Para el Residente, mostrar los proyectos donde es residente y las inspecciones/incidentes asociados
                    var proyectosEnCurso = _context.Proyectos
                        .Where(p => p.Residentes.Any(r => r.Id == usuario.Id))  // Filtrar por residentes
                        .Select(p => new ProyectoHomeViewModel
                        {
                            ProyectoNombre = p.Nombre,
                            NumeroInspeccionesProgramadas = p.Inspecciones.Count(i => i.Estado == EstadoInspeccion.Programada && i.InspectorId == usuario.Id),
                            NumeroInspeccionesPendientes = p.Inspecciones.Count(i => i.Estado == EstadoInspeccion.PendientesDeRevision && i.InspectorId == usuario.Id),
                            NumeroIncidentesActivos = p.Incidentes.Count(i => i.UsuarioId == usuario.Id && i.Estado == EstadoIncidenteEnum.Activo)  // Incidentes reportados por el usuario
                        })
                        .ToList();

                    return View("IndexAdmin", proyectosEnCurso);
                }
            }

            // Redirigir a la vista para usuarios no autenticados
            return View("Index");
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
