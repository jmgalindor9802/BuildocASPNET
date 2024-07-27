using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Buildoc.Data;
using Buildoc.Models;

namespace Buildoc.Controllers
{
    public class IncidentesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IncidentesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Incidentes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Incidentes.Include(i => i.Proyecto).Include(i => i.TipoIncidente).Include(i => i.Usuario);
            return View(await applicationDbContext.ToListAsync());
        }

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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (incidente == null)
            {
                return NotFound();
            }

            return View(incidente);
        }

        // GET: Incidentes/Create
        public IActionResult Create()
        {
            ViewData["ProyectoId"] = new SelectList(_context.Proyectos, "Id", "Departamento");
            ViewData["TipoIncidenteId"] = new SelectList(_context.TipoIncidentes, "Id", "Id");
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id");
            return View();
        }

        // POST: Incidentes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,FechaCreacion,Descripcion,FechaIncidente,HoraIncidente,Estado,Sugerencia,ProyectoId,UsuarioId,TipoIncidenteId")] Incidente incidente)
        {
            if (ModelState.IsValid)
            {
                incidente.Id = Guid.NewGuid();
                _context.Add(incidente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProyectoId"] = new SelectList(_context.Proyectos, "Id", "Departamento", incidente.ProyectoId);
            ViewData["TipoIncidenteId"] = new SelectList(_context.TipoIncidentes, "Id", "Id", incidente.TipoIncidenteId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id", incidente.UsuarioId);
            return View(incidente);
        }

        // GET: Incidentes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incidente = await _context.Incidentes.FindAsync(id);
            if (incidente == null)
            {
                return NotFound();
            }
            ViewData["ProyectoId"] = new SelectList(_context.Proyectos, "Id", "Departamento", incidente.ProyectoId);
            ViewData["TipoIncidenteId"] = new SelectList(_context.TipoIncidentes, "Id", "Id", incidente.TipoIncidenteId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id", incidente.UsuarioId);
            return View(incidente);
        }

        // POST: Incidentes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Titulo,FechaCreacion,Descripcion,FechaIncidente,HoraIncidente,Estado,Sugerencia,ProyectoId,UsuarioId,TipoIncidenteId")] Incidente incidente)
        {
            if (id != incidente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(incidente);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProyectoId"] = new SelectList(_context.Proyectos, "Id", "Departamento", incidente.ProyectoId);
            ViewData["TipoIncidenteId"] = new SelectList(_context.TipoIncidentes, "Id", "Id", incidente.TipoIncidenteId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id", incidente.UsuarioId);
            return View(incidente);
        }

        // GET: Incidentes/Delete/5
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (incidente == null)
            {
                return NotFound();
            }

            return View(incidente);
        }

        // POST: Incidentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var incidente = await _context.Incidentes.FindAsync(id);
            if (incidente != null)
            {
                _context.Incidentes.Remove(incidente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IncidenteExists(Guid id)
        {
            return _context.Incidentes.Any(e => e.Id == id);
        }
    }
}
