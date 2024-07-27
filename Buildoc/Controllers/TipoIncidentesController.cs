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
    public class TipoIncidentesController : Controller
    {
        private readonly ApplicationDbContext _context;
		private readonly UserManager<Usuario> _userManager;

		public TipoIncidentesController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
			_userManager = userManager;
		}

        // GET: TipoIncidentes
        public async Task<IActionResult> Index()
        {
            // Obtener el usuario logueado
            var usuarioLogueado = await _userManager.GetUserAsync(User);
            if (usuarioLogueado == null)
            {
                return Unauthorized(); // Si no se puede obtener el usuario logueado, retorna no autorizado
            }
            // Filtrar tipo de incidente donde el coordinador es el usuario logueado
            var tipoIncidente = await _context.TipoIncidentes
                .Where(t => t.UsuarioId == usuarioLogueado.Id)
                .ToListAsync();
            //Obtener la cantidad de tipos de incidentes creados por el coordinador logeado
            var tipoIncidentesTotales = tipoIncidente.Count();

            //Retornar a la vista la cantidad de tipos de incidentes creados por el coordinador logeado
            ViewBag.tipoIncidentesTotales = tipoIncidentesTotales;
            return View(tipoIncidente);
        }

        // GET: TipoIncidentes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoIncidente = await _context.TipoIncidentes
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoIncidente == null)
            {
                return NotFound();
            }

            return PartialView(tipoIncidente);
        }

        // GET: TipoIncidentes/Create
        public IActionResult Create()
        {
            return PartialView();
        }

        // POST: TipoIncidentes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Categoria,Titulo,Descripcion,Gravedad")] TipoIncidente tipoIncidente)
        {
            if (ModelState.IsValid)
            {
                tipoIncidente.Id = Guid.NewGuid();
				// Obtener el ID del usuario actual
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				tipoIncidente.UsuarioId = userId;
				_context.Add(tipoIncidente);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "¡El tipo de incidente se ha creado exitosamente!";
                return Json(new { success = true });
            }
            
            return PartialView(tipoIncidente);
        }

        // GET: TipoIncidentes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoIncidente = await _context.TipoIncidentes.FindAsync(id);
            if (tipoIncidente == null)
            {
                return NotFound();
            }
            return PartialView(tipoIncidente);
        }

        // POST: TipoIncidentes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Categoria,Titulo,Descripcion,Gravedad")] TipoIncidente tipoIncidente)
        {
            if (id != tipoIncidente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el tipo de incidente existente para mantener el UsuarioId original
                    var existingTipoIncidente = await _context.TipoIncidentes
                        .FirstOrDefaultAsync(ti => ti.Id == tipoIncidente.Id);
                    if (existingTipoIncidente == null)
                    {
                        return NotFound();
                    }
                    tipoIncidente.UsuarioId = existingTipoIncidente.UsuarioId;

                    // Actualizar los valores del tipo de incidente
                    _context.Entry(existingTipoIncidente).CurrentValues.SetValues(tipoIncidente);
                    // Marcar la entidad para su actualización
                    _context.Update(existingTipoIncidente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoIncidenteExists(tipoIncidente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "¡El tipo de incidente se ha editado exitosamente!";
                return Json(new { success = true });
            }
            return PartialView(tipoIncidente);
        }

        // GET: TipoIncidentes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoIncidente = await _context.TipoIncidentes
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoIncidente == null)
            {
                return NotFound();
            }

            return PartialView(tipoIncidente);
        }

        // POST: TipoIncidentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tipoIncidente = await _context.TipoIncidentes.FindAsync(id);
            if (tipoIncidente != null)
            {
                _context.TipoIncidentes.Remove(tipoIncidente);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "¡El tipo de incidente se ha eliminado exitosamente!";
            return Json(new { success = true });
        }

        private bool TipoIncidenteExists(Guid id)
        {
            return _context.TipoIncidentes.Any(e => e.Id == id);
        }
    }
}
