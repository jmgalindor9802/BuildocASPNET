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
    public class SeguimientoesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly IEmailSender _emailSender;

        public SeguimientoesController(ApplicationDbContext context, UserManager<Usuario> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Seguimientoes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Seguimientos.Include(s => s.Incidente).Include(s => s.Usuario);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Seguimientoes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seguimiento = await _context.Seguimientos
                .Include(s => s.Incidente)
                .Include(s => s.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seguimiento == null)
            {
                return NotFound();
            }

            return View(seguimiento);
        }

        // GET: Seguimientoes/Create
        public IActionResult Create(Guid? incidenteId)
        {
            if (incidenteId == null)
            {
                return NotFound();
            }
            var seguimiento = new Seguimiento
            {
                IncidenteId = incidenteId.Value
            };
            return View(seguimiento);
        }

        // POST: Seguimientoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titulo,Descripcion")] Seguimiento seguimiento)
        {
            if (ModelState.IsValid)
            {
                seguimiento.Id = Guid.NewGuid();
                // Obtener el ID del usuario actual
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                seguimiento.UsuarioId = userId;
                _context.Add(seguimiento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(seguimiento);
        }

        // GET: Seguimientoes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seguimiento = await _context.Seguimientos.FindAsync(id);
            if (seguimiento == null)
            {
                return NotFound();
            }
            ViewData["IncidenteId"] = new SelectList(_context.Incidentes, "Id", "Id", seguimiento.IncidenteId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id", seguimiento.UsuarioId);
            return View(seguimiento);
        }

        // POST: Seguimientoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Titulo,Descripcion,FechaCreacion,IncidenteId,UsuarioId")] Seguimiento seguimiento)
        {
            if (id != seguimiento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(seguimiento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeguimientoExists(seguimiento.Id))
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
            ViewData["IncidenteId"] = new SelectList(_context.Incidentes, "Id", "Id", seguimiento.IncidenteId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id", seguimiento.UsuarioId);
            return View(seguimiento);
        }

        // GET: Seguimientoes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var seguimiento = await _context.Seguimientos
                .Include(s => s.Incidente)
                .Include(s => s.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seguimiento == null)
            {
                return NotFound();
            }

            return View(seguimiento);
        }

        // POST: Seguimientoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var seguimiento = await _context.Seguimientos.FindAsync(id);
            if (seguimiento != null)
            {
                _context.Seguimientos.Remove(seguimiento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SeguimientoExists(Guid id)
        {
            return _context.Seguimientos.Any(e => e.Id == id);
        }
    }
}
