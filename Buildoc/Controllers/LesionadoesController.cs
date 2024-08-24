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
    public class LesionadoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LesionadoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Lesionadoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Lesionados.ToListAsync());
        }

        // GET: Lesionadoes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesionado = await _context.Lesionados
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lesionado == null)
            {
                return NotFound();
            }

            return View(lesionado);
        }

        // GET: Lesionadoes/Create
        public IActionResult Create(Guid? incidenteId)
        {
            if (incidenteId == null)
            {
                return NotFound();
            }
            // Crear una instancia del ViewModel y asociar el incidenteId
            var viewModel = new LesionadoViewModel
            {
                IncidenteId = incidenteId.Value
            };

            // Pasar el incidenteId a la vista
            ViewBag.IncidenteId = incidenteId;

            return PartialView(viewModel); ;
        }

        // POST: Lesionadoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid incidenteId, LesionadoViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var lesionado in model.Lesionados)
                {
                    lesionado.Id = Guid.NewGuid();

                    // Agregar el lesionado a la base de datos
                    _context.Lesionados.Add(lesionado);
                    await _context.SaveChangesAsync();

                    // Crear una relación entre el incidente y el lesionado
                    var incidenteLesionado = new IncidenteLesionado
                    {
                        Id = Guid.NewGuid(),
                        IncidenteId = incidenteId,
                        LesionadoId = lesionado.Id,
                        Defuncion = model.IncidenteLesionados.FirstOrDefault()?.Defuncion ?? false,
                        ActividadRealizada = model.IncidenteLesionados.FirstOrDefault()?.ActividadRealizada,
                        AsociadaProyecto = model.IncidenteLesionados.FirstOrDefault()?.AsociadaProyecto ?? false,
                        GeneroAfectado = model.IncidenteLesionados.FirstOrDefault()?.GeneroAfectado,
                        Hospitalizado = model.IncidenteLesionados.FirstOrDefault()?.Hospitalizado ?? false,
                        PrimerosAuxilios = model.IncidenteLesionados.FirstOrDefault()?.PrimerosAuxilios ?? false
                    };

                    // Agregar la relación a la base de datos
                    _context.IncidenteLesionados.Add(incidenteLesionado);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Pasar el incidenteId de nuevo a la vista si hay un error
            ViewBag.IncidenteId = incidenteId;
            return PartialView(model);
        }

        // GET: Lesionadoes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesionado = await _context.Lesionados.FindAsync(id);
            if (lesionado == null)
            {
                return NotFound();
            }
            return View(lesionado);
        }

        // POST: Lesionadoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nombre,Apellido,CorreoElectronico,Cedula")] Lesionado lesionado)
        {
            if (id != lesionado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lesionado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LesionadoExists(lesionado.Id))
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
            return View(lesionado);
        }

        // GET: Lesionadoes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesionado = await _context.Lesionados
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lesionado == null)
            {
                return NotFound();
            }

            return View(lesionado);
        }

        // POST: Lesionadoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lesionado = await _context.Lesionados.FindAsync(id);
            if (lesionado != null)
            {
                _context.Lesionados.Remove(lesionado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LesionadoExists(Guid id)
        {
            return _context.Lesionados.Any(e => e.Id == id);
        }
    }
}
