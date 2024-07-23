﻿using System;
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
    public class ProyectosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ProyectosController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                .Where(p => p.CoordinadorId == usuarioLogueado.Id)
                .ToListAsync();

            // Contar los proyectos con estado "Activo"
            var countActivos = await _context.Proyectos.CountAsync(p => p.Estado == "Activo" && p.CoordinadorId == usuarioLogueado.Id);

            // Contar los proyectos con estado "Finalizado"
            var countFinalizados = await _context.Proyectos.CountAsync(p => p.Estado == "Finalizado" && p.CoordinadorId == usuarioLogueado.Id);

            // Pasar los datos a la vista
            ViewBag.CountActivos = countActivos;
            ViewBag.CountFinalizados = countFinalizados;
           return View(proyectos); // Retornar solo los proyectos donde el coordinador es el usuario logueado
        
           // return View(await _context.Proyectos.ToListAsync());
        }

        // GET: Proyectos/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proyecto = await _context.Proyectos
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
                proyecto.Estado = "Activo";

                // Obtener el ID del usuario actual
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                proyecto.CoordinadorId = userId;

                // Obtener y asignar los residentes seleccionados
                proyecto.Residentes = await _userManager.Users
                    .Where(u => ResidentesIds.Contains(u.Id))
                    .ToListAsync();

                _context.Add(proyecto);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "¡El proyecto se ha creado exitosamente!";
                return Json(new { success = true });
            }
            return PartialView("Create", proyecto);
        }

        // GET: Proyectos/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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
            if (id != proyecto.Id)
            {
                return NotFound();
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
                _context.Proyectos.Remove(proyecto);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "¡El proyecto se ha eliminado exitosamente!";
            return Json(new { success = true });
        }

        private bool ProyectoExists(Guid id)
        {
            return _context.Proyectos.Any(e => e.Id == id);
        }
    }
}
