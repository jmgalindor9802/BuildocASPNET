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

namespace Buildoc.Controllers
{
    public class NovedadesIncidentesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly IEmailSender _emailSender;

        public NovedadesIncidentesController(ApplicationDbContext context, UserManager<Usuario> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: novedadesIncidentees
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.NovedadesIncidentes.Include(s => s.Incidente).Include(s => s.Usuario);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> LineaTiempo(Guid incidenteId)
        {
			var novedadesIncidentes = await _context.NovedadesIncidentes
                .Where(s => s.IncidenteId == incidenteId)
		        .Include(s => s.Usuario)
		        .ToListAsync();
			ViewBag.IncidenteId = incidenteId;
			return PartialView(novedadesIncidentes);
		}

        // GET: novedadesIncidentees/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var novedadesIncidente = await _context.NovedadesIncidentes
                .Include(s => s.Incidente)
                .Include(s => s.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (novedadesIncidente == null)
            {
                return NotFound();
            }

            return View(novedadesIncidente);
        }

        // GET: novedadesIncidentees/Create
        [HttpGet]
        public async Task<IActionResult> Create(Guid? incidenteId)
        {
            if (incidenteId == null)
            {
                return NotFound();
            }
            var incidente = await _context.Incidentes
                .Include(i => i.TipoIncidente) // Incluir el tipo de incidente si deseas mostrarlo
                .Include(i => i.Proyecto) // Incluir la información del proyecto
                /*.Include(i =>i.Afectados)*/ //Incluir los afectados
                .FirstOrDefaultAsync(i => i.Id == incidenteId);

            if (incidente == null)
            {
                return NotFound();
            }

            // Crear una instancia de NovedadesIncidente con el incidente actual
            var novedadesIncidente = new NovedadesIncidente
            {
                IncidenteId = incidenteId.Value,
                Incidente = incidente // Pasar la información del incidente
            };

            // Pasar los estados posibles a la vista
            ViewBag.Estados = new SelectList(Enum.GetValues(typeof(EstadoIncidenteEnum)));
            return PartialView(novedadesIncidente);
        }

        // POST: novedadesIncidentees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IncidenteId, Titulo, Descripcion")] NovedadesIncidente novedadesIncidente, string EstadoIncidenteNovedad)
        {
            if (ModelState.IsValid)
            {
                novedadesIncidente.Id = Guid.NewGuid();
                // Obtener el ID del usuario actual
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                novedadesIncidente.UsuarioId = userId;

                // Convertir el valor del select a enum
                EstadoIncidenteEnum? estadoSeleccionado = null;
                if (!string.IsNullOrEmpty(EstadoIncidenteNovedad))
                {
                    if (Enum.TryParse(EstadoIncidenteNovedad, out EstadoIncidenteEnum estado))
                    {
                        estadoSeleccionado = estado;
                        novedadesIncidente.EstadoNovedad = estado;  // Asignar el estado seleccionado al campo EstadoNovedad
                    }
                }

                // Buscar el incidente en la base de datos
                var incidente = await _context.Incidentes.FindAsync(novedadesIncidente.IncidenteId);
                if (incidente != null)
                {
                    // Actualizar el estado del incidente si se ha seleccionado un estado
                    if (estadoSeleccionado.HasValue)
                    {
                        incidente.Estado = estadoSeleccionado.Value;
                        _context.Update(incidente);
                    }
                }


                // Agregar la novedad
                _context.Add(novedadesIncidente);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "¡La novedad del incidente se ha creado exitosamente!";
                return Json(new { success = true });
            }
            else
            {
                // Obtener errores de validación
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = "Los datos están incompletos o inválidos. Inténtelo nuevamente", errors });
            }

            // Pasar los estados posibles a la vista si la validación falla
            ViewBag.Estados = new SelectList(Enum.GetValues(typeof(EstadoIncidenteEnum)));
            return PartialView(novedadesIncidente);
        }


        // GET: novedadesIncidentees/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var novedadesIncidente = await _context.NovedadesIncidentes.FindAsync(id);
            if (novedadesIncidente == null)
            {
                return NotFound();
            }
            ViewData["IncidenteId"] = new SelectList(_context.Incidentes, "Id", "Id", novedadesIncidente.IncidenteId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id", novedadesIncidente.UsuarioId);
            return View(novedadesIncidente);
        }

        // POST: novedadesIncidentees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Titulo,Descripcion,FechaCreacion,IncidenteId,UsuarioId")] NovedadesIncidente novedadesIncidente)
        {
            if (id != novedadesIncidente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(novedadesIncidente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!novedadesIncidenteExists(novedadesIncidente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IncidenteId"] = new SelectList(_context.Incidentes, "Id", "Id", novedadesIncidente.IncidenteId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id", novedadesIncidente.UsuarioId);
            return View(novedadesIncidente);
        }

        // GET: novedadesIncidentees/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var novedadesIncidente = await _context.NovedadesIncidentes
                .Include(s => s.Incidente)
                .Include(s => s.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (novedadesIncidente == null)
            {
                return NotFound();
            }

            return View(novedadesIncidente);
        }

        // POST: novedadesIncidentees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var novedadesIncidente = await _context.NovedadesIncidentes.FindAsync(id);
            if (novedadesIncidente != null)
            {
                _context.NovedadesIncidentes.Remove(novedadesIncidente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool novedadesIncidenteExists(Guid id)
        {
            return _context.NovedadesIncidentes.Any(e => e.Id == id);
        }
    }
}
