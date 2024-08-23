using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Buildoc.Data;
using Buildoc.Models;
using Buildoc.Models.Inspecciones;
using Microsoft.AspNetCore.Identity;

namespace Buildoc.Controllers
{
    public class RespuestaInspeccionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        public RespuestaInspeccionController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: RespuestaInspeccion
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.RespuestaInspeccion.Include(r => r.Inspeccion).Include(r => r.InspeccionAdicional);
            return View(await applicationDbContext.ToListAsync());
        }
        // Método para obtener los detalles de la inspección
        [HttpGet]
        public JsonResult InspeccionDetalles(Guid id)
        {
            var inspeccion = _context.Inspecciones
                .Include(i => i.TipoInspeccion)
                .Include(i => i.Proyecto)
                .Include(i => i.Inspector)
                .FirstOrDefault(i => i.Id == id);

            if (inspeccion == null)
            {
                return Json(new { success = false, message = "Inspección no encontrada." });
            }

            // Construir un objeto anónimo con los datos necesarios para la vista
            var inspeccionData = new
            {
                FechaInspeccion = inspeccion.FechaInspeccion,
                TipoInspeccion = new { Nombre = inspeccion.TipoInspeccion.Nombre },
                Proyecto = new { Nombre = inspeccion.Proyecto.Nombre },
                Inspector = new { Nombre = inspeccion.Inspector.NombreCompleto },
                Descripcion = inspeccion.Descripcion,
                DuracionHoras = inspeccion.DuracionHoras,
                EsTodoElDia = inspeccion.EsTodoElDia,
                Estado = inspeccion.Estado
            };
            
            return Json(new { success = true, data = inspeccionData });
        }


        // GET: RespuestaInspeccion/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var respuestaInspeccion = await _context.RespuestaInspeccion
                .Include(r => r.Inspeccion)
                .Include(r => r.InspeccionAdicional)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (respuestaInspeccion == null)
            {
                return NotFound();
            }

            return PartialView(respuestaInspeccion);
        }

        // GET: RespuestaInspeccion/Create
        public IActionResult Create(Guid inspeccionId)
        {
            var inspeccion = _context.Inspeccion
       .Include(i => i.Proyecto) 
       .FirstOrDefault(i => i.Id == inspeccionId);
            //if (inspeccion == null)
            //{
            //    return NotFound(); // Manejo si la inspección no se encuentra
            //}
            ViewData["InspeccionDetalles"] = inspeccion; // Pasar detalles a la vista
            ViewData["InspeccionId"] = inspeccionId;
            ViewData["InspeccionAdicionalId"] = new SelectList(_context.Inspeccion, "Id", "Id");
            return PartialView();
        }

        // POST: RespuestaInspeccion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,InspeccionId,Resultado,Observaciones,FechaRespuesta,EsNecesariaInspeccionAdicional,AccionesCorrectivas,AccionesCorrectivasLista,DocumentacionCompleta,RecomendacionesFuturas,RecomendacionesFuturasList,InspeccionAdicionalId,EstadoRespuesta")] RespuestaInspeccion respuestaInspeccion)
        {
            if (ModelState.IsValid)
            {
                respuestaInspeccion.EstadoRespuestaInspeccion = EstadoRespuestaInspeccion.Respondida;
                respuestaInspeccion.FechaRespuesta = DateTime.Now;
                respuestaInspeccion.Id = Guid.NewGuid();

                // Obtener la inspección relacionada
                var inspeccion = await _context.Inspecciones
                    .FirstOrDefaultAsync(i => i.Id == respuestaInspeccion.InspeccionId);

                if (inspeccion != null)
                {
                    // Establecer la relación entre Inspeccion y RespuestaInspeccion
                    inspeccion.RespuestaId = respuestaInspeccion.Id; // Asignar el Id de la respuesta a la inspección
                    inspeccion.Estado = EstadoInspeccion.PendientesDeRevision;


                    // Crear una nueva novedad
                    var novedadInspeccion = new NovedadInspeccion
                    {
                        Id = Guid.NewGuid(),
                        InspeccionId = inspeccion.Id,
                        FechaCreacion = DateTime.Now,
                        UsuarioId = _userManager.GetUserId(User), 
                        Comentario = "Inspección respondida:",
                        Estado = EstadoInspeccion.PendientesDeRevision
                    };

                    // Añadir la respuesta al contexto
                    _context.Add(respuestaInspeccion);

                    // Añadir la novedad al contexto
                    _context.Add(novedadInspeccion);

                    // Actualizar la inspección en la base de datos
                    _context.Update(inspeccion);

                    // Guardar los cambios en la base de datos
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "¡La inspección se ha respondido exitosamente!";
                    return Json(new { success = true });
                }

                // Si no se encontró la inspección, devolver un error
                TempData["ErrorMessage"] = "No se encontró la inspección.";
                return Json(new { success = false });
            }

            // Si el modelo no es válido, devolver la vista parcial con los datos existentes
            ViewData["InspeccionId"] = new SelectList(_context.Inspecciones, "Id", "Id", respuestaInspeccion.InspeccionId);
            ViewData["InspeccionAdicionalId"] = new SelectList(_context.Inspecciones, "Id", "Id", respuestaInspeccion.InspeccionAdicionalId);
            return PartialView(respuestaInspeccion);
        }


        // GET: RespuestaInspeccion/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var respuestaInspeccion = await _context.RespuestaInspeccion.FindAsync(id);
            if (respuestaInspeccion == null)
            {
                return NotFound();
            }
            ViewData["InspeccionId"] = new SelectList(_context.Inspeccion, "Id", "Id", respuestaInspeccion.InspeccionId);
            ViewData["InspeccionAdicionalId"] = new SelectList(_context.Inspeccion, "Id", "Id", respuestaInspeccion.InspeccionAdicionalId);
            return View(respuestaInspeccion);
        }

        // POST: RespuestaInspeccion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,InspeccionId,Resultado,Observaciones,FechaRespuesta,EsNecesariaInspeccionAdicional,AccionesCorrectivas,AccionesCorrectivasLista,DocumentacionCompleta,RecomendacionesFuturas,RecomendacionesFuturasList,InspeccionAdicionalId,EstadoRespuesta")] RespuestaInspeccion respuestaInspeccion)
        {
            if (id != respuestaInspeccion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(respuestaInspeccion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RespuestaInspeccionExists(respuestaInspeccion.Id))
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
            ViewData["InspeccionId"] = new SelectList(_context.Inspeccion, "Id", "Id", respuestaInspeccion.InspeccionId);
            ViewData["InspeccionAdicionalId"] = new SelectList(_context.Inspeccion, "Id", "Id", respuestaInspeccion.InspeccionAdicionalId);
            return View(respuestaInspeccion);
        }

        // GET: RespuestaInspeccion/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var respuestaInspeccion = await _context.RespuestaInspeccion
                .Include(r => r.Inspeccion)
                .Include(r => r.InspeccionAdicional)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (respuestaInspeccion == null)
            {
                return NotFound();
            }

            return View(respuestaInspeccion);
        }

        // POST: RespuestaInspeccion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var respuestaInspeccion = await _context.RespuestaInspeccion.FindAsync(id);
            if (respuestaInspeccion != null)
            {
                _context.RespuestaInspeccion.Remove(respuestaInspeccion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RespuestaInspeccionExists(Guid id)
        {
            return _context.RespuestaInspeccion.Any(e => e.Id == id);
        }
    }
}
