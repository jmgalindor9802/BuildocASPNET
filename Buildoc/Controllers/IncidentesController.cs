using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Buildoc.Data;
using Buildoc.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Buildoc.Controllers
{
    public class IncidentesController : Controller
    {
        private readonly ApplicationDbContext _context;
		private readonly UserManager<Usuario> _userManager;
        private readonly IEmailSender _emailSender;

        public IncidentesController(ApplicationDbContext context, UserManager<Usuario> userManager, IEmailSender emailSender)
        {
            _context = context;
			_userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Incidentes
        public async Task<IActionResult> Index()
        {
            // Obtener el usuario logueado
            var usuarioLogueado = await _userManager.GetUserAsync(User);
            if (usuarioLogueado == null)
            {
                return Unauthorized(); // Si no se puede obtener el usuario logueado, retorna no autorizado
            }

            // Obtener los proyectos donde el usuario logueado es el coordinador
            var proyectosDondeEsCoordinador = await _context.Proyectos
                .Where(p => p.CoordinadorId == usuarioLogueado.Id)
                .Select(p => p.Id)
                .ToListAsync();

            // Obtener todos los incidentes asociados a esos proyectos
            var todosIncidentes = await _context.Incidentes
                .Include(i => i.Proyecto)
                .Include(i => i.TipoIncidente)
                .Include(i => i.Usuario)
                .Where(i => proyectosDondeEsCoordinador.Contains(i.ProyectoId))
                .ToListAsync();

            // Filtrar incidentes activos (estado true)
            var incidentesActivos = todosIncidentes
                .Where(i => i.Estado == true)
                .ToList();

            // Filtrar incidentes archivados (estado false)
            var incidentesArchivados = todosIncidentes
                .Where(i => i.Estado == false)
                .ToList();

            // Contadores
            var totalesIncidentes = todosIncidentes.Count();
            var activosIncidentes = incidentesActivos.Count();
            var archivadosIncidentes = incidentesArchivados.Count();

            // Pasar contadores a la vista
            ViewBag.TotalesIncidentes = totalesIncidentes;
            ViewBag.ActivosIncidentes = activosIncidentes;
            ViewBag.ArchivadosIncidentes = archivadosIncidentes;

            // Retornar solo los incidentes activos para la vista Index
            return View(incidentesActivos);
        }
        
        public async Task<IActionResult> IncidenteArchivados()
        {
            // Obtener el usuario logueado
            var usuarioLogueado = await _userManager.GetUserAsync(User);
            if (usuarioLogueado == null)
            {
                return Unauthorized(); // Si no se puede obtener el usuario logueado, retorna no autorizado
            }

            // Obtener los proyectos donde el usuario logueado es el coordinador
            var proyectosDondeEsCoordinador = await _context.Proyectos
                .Where(p => p.CoordinadorId == usuarioLogueado.Id)
                .Select(p => p.Id)
                .ToListAsync();

            // Obtener todos los incidentes asociados a esos proyectos
            var todosIncidentes = await _context.Incidentes
                .Include(i => i.Proyecto)
                .Include(i => i.TipoIncidente)
                .Include(i => i.Usuario)
                .Where(i => proyectosDondeEsCoordinador.Contains(i.ProyectoId))
                .ToListAsync();

            // Filtrar incidentes archivados (estado false)
            var incidentesArchivados = todosIncidentes
                .Where(i => i.Estado == false)
                .ToList();

            // Retornar solo los incidentes activos para la vista Index
            return View(incidentesArchivados);
        }

        [Authorize(Roles = "Coordinador,Residente")]
        // GET: Incidentes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incidente = await _context.Incidentes
                .Include(i => i.Proyecto)
                .Include(i => i.TipoIncidente)
                .Include(i => i.Usuario)
                .Include(i => i.Afectados)  // Incluir los afectados
                .FirstOrDefaultAsync(m => m.Id == id);

            if (incidente == null)
            {
                return NotFound();
            }

            return PartialView(incidente);
        }


        // GET: Incidentes/Create
        public IActionResult Create()
        {
            // Obtener el ID del usuario actual
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Obtener los proyectos creados por el coordinador
            var proyectos = _context.Proyectos
                .Where(p => p.CoordinadorId == userId)
                .ToList();
            ViewData["ProyectoId"] = new SelectList(proyectos, "Id", "Nombre");
            ViewData["TipoIncidenteId"] = new SelectList(_context.TipoIncidentes, "Id", "Titulo");
            // Obtener todas las categorías del enum con sus descripciones
            var categoriasTotales = Enum.GetValues(typeof(Buildoc.Models.CategoriaEnum))
                .Cast<Buildoc.Models.CategoriaEnum>()
                .Select(c => new SelectListItem { Value = ((int)c).ToString(), Text = c.GetDescription() })
                .ToList();

            // Obtener las categorías que tienen un título registrado en la tabla TipoIncidentes
            var categoriasValidas = _context.TipoIncidentes
                .Where(t => !string.IsNullOrEmpty(t.Titulo))
                .Select(t => t.Categoria)
                .Distinct()
                .ToList();

            // Filtrar las categorías totales para obtener solo las válidas
            var categoriasConDescripcion = categoriasTotales
                .Where(c => categoriasValidas.Contains((Buildoc.Models.CategoriaEnum)int.Parse(c.Value)))
                .ToList();

            ViewBag.CategoriaTipoIncidente = categoriasConDescripcion;
            return PartialView();
        }

        [HttpGet]
        public async Task<IActionResult> GetTiposDeIncidentePorCategoria(int categoriaId)
        {
            var categoria = (CategoriaEnum)categoriaId;
            var tipos = await _context.TipoIncidentes
                .Where(t => t.Categoria == categoria && !string.IsNullOrEmpty(t.Titulo))
                .Select(t => new { id = t.Id, nombre = t.Titulo })
                .ToListAsync();
            return Json(tipos);
        }

        // Método para obtener la descripción del tipo de incidente
        [HttpGet]
        public async Task<IActionResult> GetTipoIncidenteDetalles(Guid tipoId)
        {
            var tipo = await _context.TipoIncidentes
                .Where(t => t.Id == tipoId)
                .Select(t => new
                {
                    categoria = t.Categoria.GetDescription(),
                    titulo = t.Titulo,
                    gravedad = t.Gravedad,
                    descripcion = t.Descripcion
                })
                .FirstOrDefaultAsync();
            return Json(tipo);
        }
        // POST: Incidentes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Incidente incidente, List<Afectado> afectados, bool switchAfectados, string CategoriaTipoIncidente)
        {
            if (!switchAfectados)
            {
                // Si no se activa el switch de afectados, limpiamos la lista de afectados
                afectados = new List<Afectado>();
                incidente.Afectados.Clear();
            }
            else
            {
                // Si el switch está activado, vinculamos directamente la lista de afectados al incidente
                incidente.Afectados = afectados;
            }

            if (ModelState.IsValid)
            {
                incidente.Id = Guid.NewGuid();
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                incidente.UsuarioId = userId;
                incidente.Estado = true;

                // Agregar el incidente a la base de datos
                _context.Add(incidente);
                await _context.SaveChangesAsync();

                // Preparar el mensaje de correo electrónico para el coordinador
                var proyecto = await _context.Proyectos
                    .Include(p => p.Coordinador)
                    .FirstOrDefaultAsync(p => p.Id == incidente.ProyectoId);

                if (proyecto != null && proyecto.Coordinador != null)
                {
                    var coordinador = await _userManager.FindByIdAsync(proyecto.CoordinadorId);
                    var subjectCoordinador = "Incidente reportado";
                    var htmlMessageCoordinador = $@"
                <p>Hola {coordinador.Nombres},</p>
                <p>Se reportó un incidente '<strong>{incidente.Titulo}</strong>' en el proyecto '<strong>{proyecto.Nombre}</strong>'.</p>
                <p>El incidente es de tipo '<strong>{incidente.TipoIncidente?.Titulo}</strong>'.</p>
                <p>Descripción: {incidente.Descripcion}</p>
                <p>Por favor, revisa el incidente y toma las medidas necesarias.</p>
                <p>Saludos,</p>
                <p>El equipo de <strong>Buildoc</strong></p>";

                    // Enviar el correo electrónico al coordinador
                    await _emailSender.SendEmailAsync(coordinador.Email, subjectCoordinador, htmlMessageCoordinador);
                }

                TempData["SuccessMessage"] = "¡El incidente se ha creado exitosamente!";
                return Json(new { success = true });
            }

            // Si llegamos aquí, hubo algún error en el modelo
            ViewData["ProyectoId"] = new SelectList(_context.Proyectos, "Id", "Nombre", incidente.ProyectoId);
            ViewData["TipoIncidenteId"] = new SelectList(_context.TipoIncidentes, "Id", "Titulo", incidente.TipoIncidenteId);

            // Pasar nuevamente las categorías a la vista en caso de error
            var categorias = Enum.GetValues(typeof(CategoriaEnum))
                                 .Cast<CategoriaEnum>()
                                 .Select(c => new { Id = (int)c, Name = c.GetDescription() })
                                 .ToList();
            ViewData["CategoriaTipoIncidente"] = new SelectList(categorias, "Id", "Name");

            return PartialView(incidente);
        }


        /*
         private async Task CreateAfectados(List<Afectado> afectados, Guid incidenteId)
        {
            if (ModelState.IsValid)
            {
                foreach (var afectado in afectados)
                {
                    if (afectado != null) // Asegúrate de que el afectado no sea nulo
                    {
                        afectado.Id = Guid.NewGuid(); // Generar un nuevo ID para el afectado
                        afectado.IncidenteId = incidenteId; // Asociar el afectado con el incidente
                        _context.Add(afectado); // Agregar el afectado al contexto
                    }
                }
                await _context.SaveChangesAsync(); // Guardar los cambios en la base de datos
            }
        }
         */

        // GET: Incidentes/Edit/5
        [Authorize(Roles = "Coordinador")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incidente = await _context.Incidentes
                .Include(i => i.Afectados)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (incidente == null)
            {
                return NotFound();
            }
            ViewData["ProyectoId"] = new SelectList(_context.Proyectos, "Id", "Nombre", incidente.ProyectoId);
            ViewData["TipoIncidenteId"] = new SelectList(_context.TipoIncidentes, "Id", "Id", incidente.TipoIncidenteId);
            return PartialView(incidente);
        }

        // POST: Incidentes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Coordinador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Titulo,FechaCreacion,Descripcion,FechaIncidente,Estado,ProyectoId,TipoIncidenteId")] Incidente incidente, List<Afectado> afectados, bool switchAfectados)
        {
            if (id != incidente.Id)
            {
                return NotFound();
            }
            if (!switchAfectados)
            {
                // Limpiar el objeto afectados si el switch no está marcado
                afectados = new List<Afectado>();
                afectados.Clear();
                ModelState.Remove("Afectados");
                ModelState.Remove("Afectados[0].Nombre");
                ModelState.Remove("Afectados[0].Apellido");
                ModelState.Remove("Afectados[0].CorreoElectronico");
                ModelState.Remove("Afectados[0].Cedula");
                ModelState.Remove("Afectados[0].Defuncion");
                ModelState.Remove("Afectados[0].ActividadRealizada");
                ModelState.Remove("Afectados[0].AsociadaProyecto");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener incidente existente para mantener el UsuarioId original
                    var existingIncidente = await _context.Incidentes
                        .FirstOrDefaultAsync(i => i.Id == incidente.Id);
                    if (existingIncidente == null)
                    {
                        return NotFound();
                    }
                    incidente.UsuarioId = incidente.UsuarioId;
                    // Actualizar los valores del tipo de incidente
                    _context.Entry(existingIncidente).CurrentValues.SetValues(incidente);
                    _context.Update(existingIncidente);
                    await _context.SaveChangesAsync();
                    // Solo agregar afectados si el switch está marcado
                    if (switchAfectados)
                    {
                        //await CreateAfectados(afectados, incidente.Id);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IncidenteExists(incidente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "¡El incidente se ha editado exitosamente!";
                return Json(new { success = true });
            }
            ViewData["ProyectoId"] = new SelectList(_context.Proyectos, "Id", "Nombre", incidente.ProyectoId);
            ViewData["TipoIncidenteId"] = new SelectList(_context.TipoIncidentes, "Id", "Titulo", incidente.TipoIncidenteId);
            return PartialView(incidente);
        }

        // GET: Incidentes/Delete/5
        [Authorize(Roles = "Coordinador")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incidente = await _context.Incidentes
                .Include(i => i.Proyecto)
                .Include(i => i.TipoIncidente)
                .Include(i => i.Usuario)
                .Include(i => i.Afectados)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (incidente == null)
            {
                return NotFound();
            }

            return View(incidente);
        }

        // POST: Incidentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Coordinador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var incidente = await _context.Incidentes.FindAsync(id);
            if (incidente != null)
            {
                // Cambiar el estado del incidente a true (archivado)
                incidente.Estado = true;

                // Actualizar el incidente en el contexto
                _context.Update(incidente);

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(IncidenteArchivados));
        }

        private bool IncidenteExists(Guid id)
        {
            return _context.Incidentes.Any(e => e.Id == id);
        }
    }
}
