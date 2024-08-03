using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Buildoc.Data;
using Buildoc.Models;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Buildoc.Controllers
{
    public class InspeccionesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<Usuario> _userManager;

        public InspeccionesController(IEmailSender emailSender, ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
        }


        private async Task<IEnumerable<Proyecto>> GetProyectosForCoordinadorAsync()
        {
            var coordinadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.Proyectos
                                 .Where(p => p.CoordinadorId == coordinadorId)
                                 .ToListAsync();
        }
        //Detalles dle tipo de inspeccion para el create
        public async Task<IActionResult> GetTipoInspeccionDetails(int id)
        {
            var tipoInspeccion = await _context.TipoInspeccion
                .Where(t => t.Id == id)
                .Select(t => new
                {
                    t.Nombre,
                    t.Categoria,
                    t.Descripcion
                })
                .FirstOrDefaultAsync();

            if (tipoInspeccion == null)
            {
                return NotFound();
            }

            return Json(tipoInspeccion);
        }


        // GET: Inspecciones
        public async Task<IActionResult> Index()
        {
            // Obtén el ID del coordinador logueado
            var coordinadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtén los proyectos asociados al coordinador logueado
            var proyectos = await _context.Proyectos
                                          .Where(p => p.CoordinadorId == coordinadorId)
                                          .Select(p => p.Id)
                                          .ToListAsync();

          

            // Filtra las inspecciones basadas en los proyectos asociados al coordinador
            var inspecciones = await _context.Inspeccion
                .Include(i => i.Inspector)
                .Include(i => i.Proyecto)
                .Include(i => i.TipoInspeccion)
                .Where(i => proyectos.Contains(i.ProyectoId))
                .ToListAsync();

            // Calcula el número de inspecciones con estado "Programada"
            var countProgramadas = inspecciones.Count(i => i.Estado == EstadoInspeccion.Programada);

            // Calcula el número de inspecciones con estado "En revision"
            var countPendienteRevision = inspecciones.Count(i => i.Estado == EstadoInspeccion.PendientesDeRevision);

            // Calcula el número de inspecciones con estado "Vencidas o sin responder"
            var countSinResponder = inspecciones.Count(i => i.Estado == EstadoInspeccion.SinResponder);
            // Calcula el número de inspecciones con estado "Aprobadas"
            var countAprobadas = inspecciones.Count(i => i.Estado == EstadoInspeccion.Aprobada);


            // Pasa los datos a la vista
            ViewBag.CountProgramadas = countProgramadas;
            ViewBag.CountPendienteRevision = countPendienteRevision;
            ViewBag.CountSinResponder = countSinResponder;
            ViewBag.CountAprobadas = countAprobadas;
            return View(inspecciones);
        }

        // GET: Inpsecciones Aprobadas
        public async Task<IActionResult> Aprobadas()
        {

            // Obtén el ID del coordinador logueado
            var coordinadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtén los proyectos asociados al coordinador logueado
            var proyectos = await _context.Proyectos
                                          .Where(p => p.CoordinadorId == coordinadorId)
                                          .Select(p => p.Id)
                                          .ToListAsync();



            // Filtra las inspecciones basadas en los proyectos asociados al coordinador
            var inspecciones = await _context.Inspeccion
                .Include(i => i.Inspector)
                .Include(i => i.Proyecto)
                .Include(i => i.TipoInspeccion)
                .Where(i => proyectos.Contains(i.ProyectoId))
                .ToListAsync();

            var inspeccionesAprobadas = await _context.Inspeccion
                .Where(p => p.Estado == EstadoInspeccion.Aprobada)
                .ToListAsync();

            return View(inspeccionesAprobadas);
        }

        // GET: Inpsecciones Pendientes de Revision
        public async Task<IActionResult> PendientesRevision()
        {

            // Obtén el ID del coordinador logueado
            var coordinadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtén los proyectos asociados al coordinador logueado
            var proyectos = await _context.Proyectos
                                          .Where(p => p.CoordinadorId == coordinadorId)
                                          .Select(p => p.Id)
                                          .ToListAsync();



            // Filtra las inspecciones basadas en los proyectos asociados al coordinador
            var inspecciones = await _context.Inspeccion
                .Include(i => i.Inspector)
                .Include(i => i.Proyecto)
                .Include(i => i.TipoInspeccion)
                .Where(i => proyectos.Contains(i.ProyectoId))
                .ToListAsync();

            var inspeccionesPendientesRevision = await _context.Inspeccion
                .Where(p => p.Estado == EstadoInspeccion.PendientesDeRevision)
                .ToListAsync();

            return View(inspeccionesPendientesRevision);
        }

        // GET: Inpsecciones sin responder
        public async Task<IActionResult> SinResponder()
        {

            // Obtén el ID del coordinador logueado
            var coordinadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtén los proyectos asociados al coordinador logueado
            var proyectos = await _context.Proyectos
                                          .Where(p => p.CoordinadorId == coordinadorId)
                                          .Select(p => p.Id)
                                          .ToListAsync();



            // Filtra las inspecciones basadas en los proyectos asociados al coordinador
            var inspecciones = await _context.Inspeccion
                .Include(i => i.Inspector)
                .Include(i => i.Proyecto)
                .Include(i => i.TipoInspeccion)
                .Where(i => proyectos.Contains(i.ProyectoId))
                .ToListAsync();

            var inspeccionesSinResponder = await _context.Inspeccion
                .Where(p => p.Estado == EstadoInspeccion.SinResponder)
                .ToListAsync();

            return View(inspeccionesSinResponder);
        }


        // GET: Inpsecciones Programadas
        public async Task<IActionResult> Programadas()
        {

            // Obtén el ID del coordinador logueado
            var coordinadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtén los proyectos asociados al coordinador logueado
            var proyectos = await _context.Proyectos
                                          .Where(p => p.CoordinadorId == coordinadorId)
                                          .Select(p => p.Id)
                                          .ToListAsync();

            // Filtra las inspecciones basadas en los proyectos asociados al coordinador
            var inspecciones = await _context.Inspeccion
                .Include(i => i.Inspector)
                .Include(i => i.Proyecto)
                .Include(i => i.TipoInspeccion)
                .Where(i => proyectos.Contains(i.ProyectoId))
                .ToListAsync();

            var inspeccionesProgramadas = await _context.Inspeccion
                .Where(p => p.Estado == EstadoInspeccion.Programada)
                .ToListAsync();

            return View(inspeccionesProgramadas);
        }


        // GET: Inspecciones/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspeccion = await _context.Inspeccion
                .Include(i => i.Inspector)
                .Include(i => i.Proyecto)
                .Include(i => i.TipoInspeccion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspeccion == null)
            {
                return NotFound();
            }

            return View(inspeccion);
        }

        // GET: Inspecciones/Create
        public async Task<IActionResult> Create()
        {
            ViewData["InspectorId"] = new SelectList(_context.Users, "Id", "NombreCompleto");
            ViewData["ProyectoId"] = new SelectList(await GetProyectosForCoordinadorAsync(), "Id", "Nombre");
            ViewData["TipoInspeccionId"] = new SelectList(_context.TipoInspeccion, "Id", "Nombre");
            return View();
        }

        // POST: Inspecciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FechaInspeccion,Objetivo,Descripcion,TipoInspeccionId,ProyectoId,InspectorId,Resultado,Estado")] Inspeccion inspeccion)
        {
			

			if (!ModelState.IsValid)
			{
				// Obtener errores de validación
				var errors = ModelState.Values.SelectMany(v => v.Errors)
											  .Select(e => e.ErrorMessage)
											  .ToList();
				return Json(new { success = false, message = "Los datos están incompletos o inválidos. Inténtelo nuevamente", errors });
			}
			if (inspeccion.FechaInspeccion < DateTime.Now.Date)
			{
				// Enviar mensaje de error como JSON
				return Json(new { success = false, message = "La fecha de la inspección no puede ser anterior a la fecha actual." });
			}
			if (ModelState.IsValid)
            {
                inspeccion.Id = Guid.NewGuid();
                inspeccion.Estado = EstadoInspeccion.Programada;
                _context.Add(inspeccion);
                await _context.SaveChangesAsync();

                // Obtener el inspector asignado
                var inspector = await _userManager.FindByIdAsync(inspeccion.InspectorId);
                var proyecto = await _context.Proyectos.FindAsync(inspeccion.ProyectoId);

                // Preparar el mensaje de correo electrónico en formato HTML para el inspector
                var subject = "Nueva Inspección Asignada";
                var htmlMessage = $@"
                <p>Hola {inspector.Nombres},</p>
                <p>Se ha creado una nueva inspección para el proyecto '<strong>{inspeccion.Proyecto.Nombre}</strong>'.</p>
                <p>Fecha de Inspección: {inspeccion.FechaInspeccion.ToShortDateString()}</p>
                <p>Objetivo: {inspeccion.Objetivo}</p>
                <p>Descripción: {inspeccion.Descripcion}</p>

                <p>Saludos,</p>
                <p>El equipo de <strong>Buildoc</strong></p>";

                // Enviar el correo electrónico al inspector
                await _emailSender.SendEmailAsync(inspector.Email, subject, htmlMessage);


                TempData["SuccessMessage"] = "¡La inspección se ha creado exitosamente!";
                return Json(new { success = true });
            }
            ViewData["InspectorId"] = new SelectList(_context.Users, "Id", "NombreCompleto", inspeccion.InspectorId);
            ViewData["ProyectoId"] = new SelectList(await GetProyectosForCoordinadorAsync(), "Id", "Nombre", inspeccion.ProyectoId);
            ViewData["TipoInspeccionId"] = new SelectList(_context.TipoInspeccion, "Id", "Nombre", inspeccion.TipoInspeccionId);
            return PartialView("Create",inspeccion);
        }

        // GET: Inspecciones/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {

            ViewBag.EstadoList = Enum.GetValues(typeof(EstadoInspeccion))
      .Cast<EstadoInspeccion>()
      .Select(e => new SelectListItem
      {
          Value = e.ToString(),
          Text = e.GetType()
                    .GetField(e.ToString())
                    .GetCustomAttributes(typeof(DisplayAttribute), false)
                    .SingleOrDefault() is DisplayAttribute displayAttribute ? displayAttribute.Name : e.ToString()
      }).ToList();

            if (id == null)
            {
                return NotFound();
            }

            var inspeccion = await _context.Inspeccion.FindAsync(id);
            if (inspeccion == null)
            {
                return NotFound();
            }

			

			ViewData["InspectorId"] = new SelectList(_context.Users, "Id", "NombreCompleto", inspeccion.InspectorId);
            ViewData["ProyectoId"] = new SelectList(await GetProyectosForCoordinadorAsync(), "Id", "Nombre", inspeccion.ProyectoId);
            ViewData["TipoInspeccionId"] = new SelectList(_context.TipoInspeccion, "Id", "Nombre", inspeccion.TipoInspeccionId);
			// Pasar el estado de la inspección a la vista
			ViewData["Estado"] = inspeccion.Estado;
			return PartialView("Edit",inspeccion);
        }

        // POST: Inspecciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FechaInspeccion,Objetivo,Descripcion,TipoInspeccionId,ProyectoId,InspectorId,Resultado,Estado")] Inspeccion inspeccion)
        {
			var inspeccionOriginal = await _context.Inspeccion.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
			if (inspeccionOriginal == null)
			{
				return NotFound();
			}

			if (inspeccionOriginal.Estado == EstadoInspeccion.PendientesDeRevision || inspeccionOriginal.Estado == EstadoInspeccion.Aprobada)
			{
				return Json(new { success = false, message = "No se puede editar una inspección que está pendiente de revisión o finalizada." });
			}

			if (id != inspeccion.Id)
            {
                return NotFound();
            }
			if (inspeccion.FechaInspeccion < DateTime.Now.Date)
			{
				// Enviar mensaje de error como JSON
				return Json(new { success = false, message = "La fecha de la inspección no puede ser anterior a la fecha actual." });
			}
			if (!ModelState.IsValid)
			{
				// Obtener errores de validación
				var errors = ModelState.Values.SelectMany(v => v.Errors)
											  .Select(e => e.ErrorMessage)
											  .ToList();
				return Json(new { success = false, message = "Los datos están incompletos o inválidos. Inténtelo nuevamente", errors });
			}
			if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspeccion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InspeccionExists(inspeccion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "¡La inspección se ha editado exitosamente!";
                return Json(new { success = true });
            }
            ViewData["InspectorId"] = new SelectList(_context.Users, "Id", "NombreCompleto", inspeccion.InspectorId);
            ViewData["ProyectoId"] = new SelectList(await GetProyectosForCoordinadorAsync(), "Id", "Nombre", inspeccion.ProyectoId);
            ViewData["TipoInspeccionId"] = new SelectList(_context.TipoInspeccion, "Id", "Nombre", inspeccion.TipoInspeccionId);
            return PartialView("Edit",inspeccion);
        }

        // GET: Inspecciones/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspeccion = await _context.Inspeccion
                .Include(i => i.Inspector)
                .Include(i => i.Proyecto)
                .Include(i => i.TipoInspeccion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspeccion == null)
            {
                return NotFound();
            }

            return View(inspeccion);
        }

        // POST: Inspecciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var inspeccion = await _context.Inspeccion.FindAsync(id);
            if (inspeccion != null)
            {
                _context.Inspeccion.Remove(inspeccion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InspeccionExists(Guid id)
        {
            return _context.Inspeccion.Any(e => e.Id == id);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeState(Guid id, EstadoInspeccion newState)
        {
            var inspeccion = await _context.Inspeccion.FindAsync(id);
            if (inspeccion == null)
            {
                return Json(new { success = false, message = "Inspección no encontrada." });
            }

            // Verifica que el nuevo estado sea diferente al actual y que sea un estado permitido
            if (inspeccion.Estado == newState)
            {
                return Json(new { success = false, message = "La inspección ya se encuentra en el estado seleccionado." });
            }

            if (newState != EstadoInspeccion.Aprobada && newState != EstadoInspeccion.Desaprobada)
            {
                return Json(new { success = false, message = "Estado inválido. Solo se permite cambiar a 'Aprobada' o 'Desaprobada'." });
            }

            try
            {
                inspeccion.Estado = newState;
                _context.Update(inspeccion);
                await _context.SaveChangesAsync();
                if (newState == EstadoInspeccion.Aprobada)
                {
                    TempData["SuccessMessage"] = "La inspección ha sido aprobada exitosamente.";
                }
                else if (newState == EstadoInspeccion.Desaprobada)
                {
                    TempData["WarningMessage"] = "La inspección ha sido desaprobada.";
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al cambiar el estado: " + ex.Message });
            }
        }

    }
}
