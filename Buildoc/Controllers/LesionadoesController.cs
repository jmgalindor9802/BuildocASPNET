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
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Buildoc.Controllers
{
    public class LesionadoesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public LesionadoesController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Lesionadoes
        public async Task<IActionResult> Index()
        {
            // Obtener el usuario logueado
            var usuarioLogueado = await _userManager.GetUserAsync(User);
            if (usuarioLogueado == null)
            {
                return Unauthorized(); // Si no se puede obtener el usuario logueado, retorna no autorizado
            }
            // Obtén el rol del usuario logueado
            var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(usuarioLogueado.Id));
            var rolUsuario = roles.FirstOrDefault();

            List<Lesionado> todosLesionados = new List<Lesionado>(); // Declarar la variable fuera del if-else
            if (rolUsuario == "Coordinador")
            {
                // Obtener los proyectos donde el usuario logueado es el coordinador
                var proyectosDondeEsCoordinador = await _context.Proyectos
                    .Where(p => p.CoordinadorId == usuarioLogueado.Id)
                    .Select(p => p.Id)
                    .ToListAsync();
                // Obtener los incidentes asociados a esos proyectos
                var incidentes = await _context.Incidentes
                    .Where(i => proyectosDondeEsCoordinador.Contains(i.ProyectoId))
                    .Select(i => i.Id)
                    .ToListAsync();

                // Obtener todos los lesionados asociados a esos incidentes
                todosLesionados = await _context.Lesionados
                    .Where(l => l.IncidenteLesionados.Any(il => incidentes.Contains(il.IncidenteId)))
                    .ToListAsync();
            }
            else if (rolUsuario == "Residente")
            {
                // Obtener los incidentes reportados por el usuario logueado
                var incidentes = await _context.Incidentes
                    .Where(i => i.UsuarioId == usuarioLogueado.Id)
                    .Select(i => i.Id)
                    .ToListAsync();

                // Obtener todos los lesionados asociados a esos incidentes
                todosLesionados = await _context.Lesionados
                    .Where(l => l.IncidenteLesionados.Any(il => incidentes.Contains(il.IncidenteId)))
                    .ToListAsync();
            }
            else
            {
                // Si el rol no es Coordinador ni Residente, maneja otros casos o filtra según sea necesario
                todosLesionados = new List<Lesionado>();
            }
            return View(todosLesionados);
        }


        // GET: Lesionadoes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lesionados = await _context.Lesionados
                .Include(l => l.IncidenteLesionados)
                    .ThenInclude(il=> il.Incidente)
                        .ThenInclude(i => i.Proyecto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lesionados == null)
            {
                return NotFound();
            }

            return PartialView(lesionados);
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
            foreach (var lesionado in model.Lesionados)
            {
                // Validar que la cédula no sea un número negativo
                if (lesionado.Cedula < 0)
                {
                    return Json(new { success = false, message = "La cédula no puede contener números negativos." });
                }
                else if (lesionado.Cedula.Value.ToString().Length < 7 || lesionado.Cedula.Value.ToString().Length > 10)
                {
                    // Validar que la cédula tenga entre 7 y 10 dígitos
                    return Json(new { success = false, message = "La cédula debe tener entre 7 y 10 dígitos." });
                }

                // Verificar si el lesionado ya está registrado como fallecido
                var existingLesionado = await _context.Lesionados
                    .FirstOrDefaultAsync(l => l.Cedula == lesionado.Cedula);

                if (existingLesionado != null && existingLesionado.ConfimacionDefuncion==true)
                {
                    // Si el lesionado está muerto, no permitir el reporte en futuros incidentes
                    return Json(new { success = false, message = $"El lesionado con cédula {lesionado.Cedula} está registrado como fallecido y no puede ser reportado en nuevos incidentes." });
                }
            }
            // Validar cédulas duplicadas
            var cedulas = model.Lesionados.Select(l => l.Cedula).ToList();
            if (cedulas.Count != cedulas.Distinct().Count())
            {
                return Json(new { success = false, message = "El formulario contiene cédulas duplicadas." });
            }
            if (ModelState.IsValid)
            {
                foreach (var lesionado in model.Lesionados)
                {
                    // Verificar si el lesionado ya existe en la base de datos por cédula
                    var existingLesionado = await _context.Lesionados
                        .FirstOrDefaultAsync(l => l.Cedula == lesionado.Cedula);

                    if (existingLesionado != null)
                    {
                        // Si el lesionado ya existe, usar su ID
                        lesionado.Id = existingLesionado.Id;
                    }
                    else
                    {
                        // Si el lesionado no existe, asignar un nuevo ID y agregarlo a la base de datos
                        lesionado.Id = Guid.NewGuid();
                        _context.Lesionados.Add(lesionado);
                        await _context.SaveChangesAsync(); // Guardar el nuevo lesionado
                    }

                    // Crear la relación entre el incidente y el lesionado
                    var incidenteLesionado = new IncidenteLesionado
                    {
                        Id = Guid.NewGuid(),
                        IncidenteId = incidenteId,
                        LesionadoId = lesionado.Id, // Usar el ID del lesionado, sea nuevo o existente
                        Defuncion = model.IncidenteLesionados.FirstOrDefault()?.Defuncion ?? false,
                        ActividadRealizada = model.IncidenteLesionados.FirstOrDefault()?.ActividadRealizada,
                        AsociadaProyecto = model.IncidenteLesionados.FirstOrDefault()?.AsociadaProyecto ?? false,
                        GeneroAfectado = model.IncidenteLesionados.FirstOrDefault()?.GeneroAfectado,
                        Hospitalizado = model.IncidenteLesionados.FirstOrDefault()?.Hospitalizado ?? false,
                        PrimerosAuxilios = model.IncidenteLesionados.FirstOrDefault()?.PrimerosAuxilios ?? false
                    };

                    // **Aquí verificamos si Defuncion es true**
                    if (incidenteLesionado.Defuncion)
                    {
                        // Actualizar el valor de ConfimacionDefuncion para el lesionado
                        existingLesionado = await _context.Lesionados.FirstOrDefaultAsync(l => l.Id == lesionado.Id);

                        if (existingLesionado != null)
                        {
                            existingLesionado.ConfimacionDefuncion = true;
                            _context.Lesionados.Update(existingLesionado); // Actualizar lesionado
                            await _context.SaveChangesAsync(); // Guardar cambios en la base de datos
                        }
                    }

                    // Agregar la relación a la base de datos
                    _context.IncidenteLesionados.Add(incidenteLesionado);
                }

                // Guardar todos los cambios en la base de datos
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "¡El lesionado se ha reportado exitosamente!";
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

            // Cargar IncidenteLesionado
            var incidenteLesionado = await _context.IncidenteLesionados
                                                   .Include(il => il.Lesionado)
                                                   .FirstOrDefaultAsync(il => il.Id == id);

            if (incidenteLesionado == null)
            {
                return NotFound();
            }

            return PartialView(incidenteLesionado);
        }


        // POST: Lesionadoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, IncidenteLesionado incidenteLesionado)
        {
            if (id != incidenteLesionado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Validaciones adicionales
                if (incidenteLesionado.Lesionado == null || incidenteLesionado == null)
                {
                    TempData["ErrorMessage"] = "Los datos proporcionados no son válidos.";
                    return View(incidenteLesionado);
                }
                try
                {
                    // Cargar el Lesionado existente de la base de datos
                    var lesionadoExistente = await _context.Lesionados
                                                           .FirstOrDefaultAsync(l => l.Id == incidenteLesionado.LesionadoId);

                    if (lesionadoExistente == null)
                    {
                        return NotFound("El lesionado no fue encontrado.");
                    }

                    // Actualizar los valores del lesionado existente
                    lesionadoExistente.Nombre = incidenteLesionado.Lesionado.Nombre;
                    lesionadoExistente.Apellido = incidenteLesionado.Lesionado.Apellido;
                    lesionadoExistente.CorreoElectronico = incidenteLesionado.Lesionado.CorreoElectronico;

                    // Actualizar el incidente lesionado
                    _context.Update(incidenteLesionado);


                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LesionadoExists(incidenteLesionado.Lesionado.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    // Aquí capturamos cualquier otro tipo de excepción y mostramos el mensaje.
                    TempData["ErrorMessage"] = "Ocurrió un error al intentar guardar los cambios: " + ex.Message;
                    return View(incidenteLesionado);
                }
                TempData["SuccessMessage"] = "¡El lesionado se ha editado exitosamente!";
                return Json(new { success = true });
            }
            return PartialView(incidenteLesionado);
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
