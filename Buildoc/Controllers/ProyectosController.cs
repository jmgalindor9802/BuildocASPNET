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
using static Buildoc.Models.Proyecto;
using System.ComponentModel.DataAnnotations;

namespace Buildoc.Controllers
{
    public class ProyectosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly IEmailSender _emailSender;

        public ProyectosController(IEmailSender emailSender, ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }        

    

        // GET: Proyectos
        public async Task<IActionResult> Index()
        {
            // Obtener el usuario logueado
            var usuarioLogueado = await _userManager.GetUserAsync(User);
            if (usuarioLogueado == null)
            {
                return Unauthorized(); // Si no se puede obtener el usuario logueado, retorna no autorizado
            }

            // Filtrar proyectos donde el coordinador es el usuario logueado
            var proyectos = await _context.Proyectos
                .Where(p => p.CoordinadorId == usuarioLogueado.Id && p.Estado != Proyecto.EstadoProyecto.Archivado)
                .ToListAsync();

            // Contar los proyectos con estado "Activo"
            var countActivos = await _context.Proyectos.CountAsync(p => p.Estado == Proyecto.EstadoProyecto.EnCurso && p.CoordinadorId == usuarioLogueado.Id);

            // Contar los proyectos con estado "Archivado"
            var countArchivados = await _context.Proyectos.CountAsync(p => p.Estado == Proyecto.EstadoProyecto.Archivado && p.CoordinadorId == usuarioLogueado.Id);

            // Contar los proyectos con estado "Finalizado"
            var countFinalizados = await _context.Proyectos.CountAsync(p => p.Estado == Proyecto.EstadoProyecto.Finalizado && p.CoordinadorId == usuarioLogueado.Id);

            // Pasar los datos a la vista
            ViewBag.CountActivos = countActivos;
            ViewBag.CountFinalizados = countFinalizados;
            ViewBag.CountArchivados = countArchivados;
            return View(proyectos); // Retornar solo los proyectos donde el coordinador es el usuario logueado
        
           // return View(await _context.Proyectos.ToListAsync());
        }


        // GET: Proyectos/Archivados
        public async Task<IActionResult> Archivados()
        {
            var coordinadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var proyectosArchivados = await _context.Proyectos
                .Where(p => p.Estado == Proyecto.EstadoProyecto.Archivado && p.CoordinadorId == coordinadorId)
                .ToListAsync();

            return View(proyectosArchivados);
        }

        // GET: Proyectos/EnCurso
        public async Task<IActionResult> EnCurso()
        {
            var coordinadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var proyectosEnCurso = await _context.Proyectos
                .Where(p => p.Estado == Proyecto.EstadoProyecto.EnCurso && p.CoordinadorId == coordinadorId)
                .ToListAsync();

            return View(proyectosEnCurso);
        }
        // GET: Proyectos/Finalizados
        public async Task<IActionResult> Finalizados()
        {
            var coordinadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var proyectosFinalizados = await _context.Proyectos
                .Where(p => p.Estado == Proyecto.EstadoProyecto.Finalizado && p.CoordinadorId == coordinadorId)
                .ToListAsync();

            return View(proyectosFinalizados);
        }



        // GET: Proyectos/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proyecto = await _context.Proyectos
                  .Include(p => p.Coordinador)
                   .Include(p => p.Residentes) // Incluir residentes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (proyecto == null)
            {
                return NotFound();
            }

            return PartialView("Details",proyecto);
        }

        public async Task<IActionResult> Create()
        {
            // Obtener todos los usuarios con el rol "Residente"
            var usuarios = await _userManager.GetUsersInRoleAsync("Residente");
            ViewBag.Usuarios = usuarios.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = $"{u.Nombres} {u.Apellidos} ({u.UserName})"
            });


            return PartialView();
        }


        // POST: Proyectos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Departamento,Municipio,Direccion,Cliente, Estado")] Proyecto proyecto, List<string> ResidentesIds)
        {
            // Verificar si ya existe un proyecto con el mismo nombre
            var existingProyecto = await _context.Proyectos.FirstOrDefaultAsync(p => p.Nombre == proyecto.Nombre);

            if (existingProyecto != null)
            {
                return Json(new { success = false, message = "Ya existe un proyecto con este nombre." });
            }

            if (ModelState.IsValid)
            {
                proyecto.Id = Guid.NewGuid();
                proyecto.Estado = Proyecto.EstadoProyecto.EnCurso;

                // Obtener el ID del usuario actual
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                proyecto.CoordinadorId = userId;

                // Obtener y asignar los residentes seleccionados
                var residentes = await _userManager.Users
                    .Where(u => ResidentesIds.Contains(u.Id))
                    .ToListAsync();

                proyecto.Residentes = residentes;

                _context.Add(proyecto);
                await _context.SaveChangesAsync();

                // Preparar el mensaje de correo electrónico en formato HTML para el coordinador
                var coordinador = await _userManager.FindByIdAsync(userId);
                var subjectCoordinador = "Nuevo Proyecto Creado";
                var htmlMessageCoordinador = $@"
            <p>Hola {coordinador.Nombres},</p>
            <p>El proyecto '<strong>{proyecto.Nombre}</strong>' ha sido creado exitosamente.</p>
            <p>Descripción: {proyecto.Descripcion}</p>
            <p>Cliente: {proyecto.Cliente}</p>
            <p>Saludos,</p>
            <p>El equipo de <strong>Buildoc</strong></p>";

                // Enviar el correo electrónico al coordinador
                await _emailSender.SendEmailAsync(coordinador.Email, subjectCoordinador, htmlMessageCoordinador);

                // Preparar y enviar el mensaje de correo electrónico en formato HTML para cada residente
                var subjectResidente = "Nuevo Proyecto Asignado";
                foreach (var residente in residentes)
                {
                    var htmlMessageResidente = $@"
                <p>Hola {residente.Nombres},</p>
                <p>Has sido asignado al proyecto '<strong>{proyecto.Nombre}</strong>'.</p>
                <p>Descripción: {proyecto.Descripcion}</p>
                <p>Cliente: {proyecto.Cliente}</p>
                <p>Saludos,</p>
                <p>El equipo de <strong>Buildoc</strong></p>";

                    // Enviar el correo electrónico a cada residente
                    await _emailSender.SendEmailAsync(residente.Email, subjectResidente, htmlMessageResidente);
                }

                TempData["SuccessMessage"] = "¡El proyecto se ha creado exitosamente!";
                return Json(new { success = true });
            }

            // Asegurarse de volver a llenar ViewBag.Usuarios en caso de que el modelo no sea válido
            var residentesRole = await _userManager.GetUsersInRoleAsync("Residente");
            ViewBag.Usuarios = residentesRole.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = $"{u.Nombres} {u.Apellidos} ({u.UserName})"
            }).ToList();

            return PartialView("Create", proyecto);
        }


        // GET: Proyectos/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {


            if (id == null)
            {
                return NotFound();
            }

            ViewBag.EstadosProyecto = Enum.GetValues(typeof(EstadoProyecto))
                      .Cast<EstadoProyecto>()
                      .Select(e => new SelectListItem
                      {
                          Value = e.ToString(),
                          Text = e.GetType()
                    .GetField(e.ToString())
                    .GetCustomAttributes(typeof(DisplayAttribute), false)
                    .SingleOrDefault() is DisplayAttribute displayAttribute ? displayAttribute.Name : e.ToString()

                      }).ToList();

            var proyecto = await _context.Proyectos
                                         .Include(p => p.Coordinador) // Incluir el Coordinador
                                         .Include(p => p.Residentes)
                                         .FirstOrDefaultAsync(m => m.Id == id);
            if (proyecto == null)
            {
                return NotFound();
            }

            // Obtener la lista de usuarios para la vista
            var usuarios = await _userManager.GetUsersInRoleAsync("Residente");
           
            ViewBag.Usuarios = usuarios.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = $"{u.Nombres} {u.Apellidos} ({u.UserName})"
            });
            return PartialView("Edit",proyecto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Nombre,Descripcion,Departamento,Municipio,Direccion,Cliente,Estado,CoordinadorId")] Proyecto proyecto, List<string> ResidentesIds)
        {

            // Verificar si ya existe un proyecto con el mismo nombre, excluyendo el proyecto actual
            var proyectoConMismoNombre = await _context.Proyectos
                .FirstOrDefaultAsync(p => p.Nombre == proyecto.Nombre && p.Id != proyecto.Id);

            if (proyectoConMismoNombre != null)
            {
                return Json(new { success = false, message = "Ya existe un proyecto con este nombre." });
            }

            if (id != proyecto.Id)
            {
                return NotFound();
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
                    // Obtener el proyecto existente para mantener el CoordinadorId original
                    var existingProyecto = await _context.Proyectos
                        .Include(p => p.Residentes) // Incluir los residentes actuales
                        .FirstOrDefaultAsync(p => p.Id == proyecto.Id);

                    if (existingProyecto == null)
                    {
                        return NotFound();
                    }

                    // Mantener el CoordinadorId original
                    proyecto.CoordinadorId = existingProyecto.CoordinadorId;

                    // Obtener la lista de nuevos residentes
                    var nuevosResidentes = await _userManager.Users
                        .Where(u => ResidentesIds.Contains(u.Id))
                        .ToListAsync();

                    // Obtener los residentes que estaban antes
                    var residentesAntes = existingProyecto.Residentes;

                    // Identificar nuevos residentes que no estaban antes
                    var nuevosResidentesIds = nuevosResidentes.Select(r => r.Id).ToHashSet();
                    var residentesAntesIds = residentesAntes.Select(r => r.Id).ToHashSet();
                    var residentesAñadidosIds = nuevosResidentesIds.Except(residentesAntesIds).ToList();

                    // Actualizar la lista de residentes del proyecto
                    existingProyecto.Residentes.Clear(); // Limpiar los residentes existentes
                    foreach (var residente in nuevosResidentes)
                    {
                        existingProyecto.Residentes.Add(residente);
                    }

                    // Actualizar los valores del proyecto
                    _context.Entry(existingProyecto).CurrentValues.SetValues(proyecto);

                    // Marcar la entidad para su actualización
                    _context.Update(existingProyecto);

                    await _context.SaveChangesAsync();


                    // Enviar correos electrónicos a los nuevos residentes
                    var subject = "Nuevo Proyecto Asignado";
                    foreach (var residente in nuevosResidentes.Where(r => residentesAñadidosIds.Contains(r.Id)))
                    {
                        var htmlMessage = $@"
                    <p>Hola {residente.Nombres},</p>
                    <p>Has sido asignado al proyecto '<strong>{proyecto.Nombre}</strong>'.</p>
                    <p>Descripción: {proyecto.Descripcion}</p>
                    <p>Cliente: {proyecto.Cliente}</p>
                    <p>Saludos,</p>
                    <p>El equipo de <strong>Buildoc</strong></p>";

                        // Enviar el correo electrónico a cada nuevo residente
                        await _emailSender.SendEmailAsync(residente.Email, subject, htmlMessage);
                    }
                    TempData["SuccessMessage"] = "¡El proyecto se ha editado exitosamente!";
                    return Json(new { success = true });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProyectoExists(proyecto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
           
            return PartialView("Edit", proyecto);
        }


        // GET: Proyectos/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proyecto = await _context.Proyectos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (proyecto == null)
            {
                return NotFound();
            }

            return PartialView("Delete",proyecto);
        }

        // POST: Proyectos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto != null)
            {
                // Cambiar el estado a "Archivado" en lugar de eliminar
                proyecto.Estado = Proyecto.EstadoProyecto.Archivado;
                _context.Proyectos.Update(proyecto);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "¡El proyecto se ha archivado exitosamente!"; 
            }


            return Json(new { success = true });
        }

        private bool ProyectoExists(Guid id)
        {
            return _context.Proyectos.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Desarchivar(Guid id)
        {
            var proyecto = await _context.Proyectos.FindAsync(id);
            if (proyecto == null)
            {
                return NotFound();
            }

            proyecto.Estado = Proyecto.EstadoProyecto.EnCurso;
            _context.Update(proyecto);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "¡El proyecto se ha desarchivado exitosamente!";
            return RedirectToAction(nameof(Archivados));
        }

    }
}
