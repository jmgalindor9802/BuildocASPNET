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
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Buildoc.Controllers
{
    public class IncidentesController : Controller
    {
        private readonly ApplicationDbContext _context;
		private readonly UserManager<Usuario> _userManager;
        private readonly IEmailSender _emailSender;

        public IncidentesController(ApplicationDbContext context, UserManager<Usuario> userManager, IEmailSender emailSender)
        {
            _context = context;
			_userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Incidentes
        public async Task<IActionResult> Index()
        {
            // Obtener el usuario logueado
            var usuarioLogueado = await _userManager.GetUserAsync(User);
            if (usuarioLogueado == null)
            {
                return Unauthorized(); // Si no se puede obtener el usuario logueado, retorna no autorizado
            }

            // Obtener los proyectos donde el usuario logueado es el coordinador
            var proyectosDondeEsCoordinador = await _context.Proyectos
                .Where(p => p.CoordinadorId == usuarioLogueado.Id)
                .Select(p => p.Id)
                .ToListAsync();

            // Obtener todos los incidentes asociados a esos proyectos
            var todosIncidentes = await _context.Incidentes
                .Include(i => i.Proyecto)
                .Include(i => i.TipoIncidente)
                .Include(i => i.Usuario)
                .Where(i => proyectosDondeEsCoordinador.Contains(i.ProyectoId))
                .ToListAsync();

            // Filtrar incidentes activos (estado true)
            var incidentesActivos = todosIncidentes
                .Where(i => i.Estado == true)
                .ToList();

            // Filtrar incidentes archivados (estado false)
            var incidentesArchivados = todosIncidentes
                .Where(i => i.Estado == false)
                .ToList();

            // Contadores
            var totalesIncidentes = todosIncidentes.Count();
            var activosIncidentes = incidentesActivos.Count();
            var archivadosIncidentes = incidentesArchivados.Count();

            // Pasar contadores a la vista
            ViewBag.TotalesIncidentes = totalesIncidentes;
            ViewBag.ActivosIncidentes = activosIncidentes;
            ViewBag.ArchivadosIncidentes = archivadosIncidentes;

            // Retornar solo los incidentes activos para la vista Index
            return View(incidentesActivos);
        }
        
        public async Task<IActionResult> IncidenteArchivados()
        {
            // Obtener el usuario logueado
            var usuarioLogueado = await _userManager.GetUserAsync(User);
            if (usuarioLogueado == null)
            {
                return Unauthorized(); // Si no se puede obtener el usuario logueado, retorna no autorizado
            }

            // Obtener los proyectos donde el usuario logueado es el coordinador
            var proyectosDondeEsCoordinador = await _context.Proyectos
                .Where(p => p.CoordinadorId == usuarioLogueado.Id)
                .Select(p => p.Id)
                .ToListAsync();

            // Obtener todos los incidentes asociados a esos proyectos
            var todosIncidentes = await _context.Incidentes
                .Include(i => i.Proyecto)
                .Include(i => i.TipoIncidente)
                .Include(i => i.Usuario)
                .Where(i => proyectosDondeEsCoordinador.Contains(i.ProyectoId))
                .ToListAsync();

            // Filtrar incidentes archivados (estado false)
            var incidentesArchivados = todosIncidentes
                .Where(i => i.Estado == false)
                .ToList();

            // Retornar solo los incidentes activos para la vista Index
            return View(incidentesArchivados);
        }

        [Authorize(Roles = "Coordinador,Residente")]
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
                .Include(i => i.Afectados)  // Incluir los afectados
                .FirstOrDefaultAsync(m => m.Id == id);

            if (incidente == null)
            {
                return NotFound();
            }

            return PartialView(incidente);
        }


        // GET: Incidentes/Create
        public IActionResult Create()
        {
            // Obtener el ID del usuario actual
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener los proyectos creados por el coordinador logeado y que estén en estado "EnCurso"
            var proyectos = _context.Proyectos
                .Where(p => p.CoordinadorId == userId && p.Estado == Proyecto.EstadoProyecto.EnCurso)
                .ToList();
            ViewData["ProyectoId"] = new SelectList(proyectos, "Id", "Nombre");
            ViewData["TipoIncidenteId"] = new SelectList(_context.TipoIncidentes, "Id", "Titulo");
            // Obtener todas las categorías del enum con sus descripciones
            var categoriasTotales = Enum.GetValues(typeof(Buildoc.Models.CategoriaEnum))
                .Cast<Buildoc.Models.CategoriaEnum>()
                .Select(c => new SelectListItem { Value = ((int)c).ToString(), Text = c.GetDescription() })
                .ToList();

            // Obtener las categorías que tienen un título registrado en la tabla TipoIncidentes
            var categoriasValidas = _context.TipoIncidentes
                .Where(t => !string.IsNullOrEmpty(t.Titulo))
                .Select(t => t.Categoria)
                .Distinct()
                .ToList();

            // Filtrar las categorías totales para obtener solo las válidas
            var categoriasConDescripcion = categoriasTotales
                .Where(c => categoriasValidas.Contains((Buildoc.Models.CategoriaEnum)int.Parse(c.Value)))
                .ToList();

            ViewBag.CategoriaTipoIncidente = categoriasConDescripcion;
            return PartialView();
        }

        [HttpGet]
        public async Task<IActionResult> GetTiposDeIncidentePorCategoria(int categoriaId)
        {
            var categoria = (CategoriaEnum)categoriaId;
            var tipos = await _context.TipoIncidentes
                .Where(t => t.Categoria == categoria && !string.IsNullOrEmpty(t.Titulo))
                .Select(t => new { id = t.Id, nombre = t.Titulo })
                .ToListAsync();
            return Json(tipos);
        }

        // Método para obtener la descripción del tipo de incidente
        [HttpGet]
        public async Task<IActionResult> GetTipoIncidenteDetalles(Guid tipoId)
        {
            var tipo = await _context.TipoIncidentes
                .Where(t => t.Id == tipoId)
                .Select(t => new
                {
                    categoria = t.Categoria.GetDescription(),
                    titulo = t.Titulo,
                    gravedad = t.Gravedad,
                    descripcion = t.Descripcion
                })
                .FirstOrDefaultAsync();
            return Json(tipo);
        }
        // POST: Incidentes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Incidente incidente, List<Afectado> afectados, bool switchAfectados, string CategoriaTipoIncidente)
        {
            if (!switchAfectados)
            {
                // Si no se activa el switch de afectados, limpiamos la lista de afectados
                afectados = new List<Afectado>();
                incidente.Afectados.Clear();
            }
            else
            {
                // Validar las cédulas de los afectados
                foreach (var afectado in afectados)
                {
                    if (afectado.Cedula.HasValue)
                    {
                        string cedulaString = afectado.Cedula.Value.ToString();
                        if (cedulaString.Length != 7 && cedulaString.Length != 10)
                        {
                            return Json(new { success = false, message = "La cedula debe tener 7 o 10 digitos" });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError($"Afectados[{afectados.IndexOf(afectado)}].Cedula", "La cédula es obligatoria.");
                    }
                }
                // Si el switch está activado, vinculamos directamente la lista de afectados al incidente
                incidente.Afectados = afectados;
            }
            // Validar que la fecha del incidente no sea mayor a la fecha actual
            if (incidente.FechaIncidente > DateOnly.FromDateTime(DateTime.Today))
            {
                return Json(new { success = false, message = "La fecha del incidente no puede ser superior a la actual" });
            }
            // Declarar la variable userId aquí para que esté disponible en todo el método
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
                incidente.Id = Guid.NewGuid();
                incidente.UsuarioId = userId;
                incidente.Estado = true;

                // Agregar el incidente a la base de datos
                _context.Add(incidente);
                await _context.SaveChangesAsync();

                // Obtener el incidente con su TipoIncidente
                var incidenteConTipo = await _context.Incidentes
                    .Include(i => i.TipoIncidente)
                    .FirstOrDefaultAsync(i => i.Id == incidente.Id);

                if (incidenteConTipo != null && incidenteConTipo.TipoIncidente != null)
                {
                    var tipoIncidenteTitulo = incidenteConTipo.TipoIncidente.Titulo;

                    if (tipoIncidenteTitulo != null && (incidenteConTipo.TipoIncidente.Gravedad.Equals("Medio", StringComparison.OrdinalIgnoreCase) || incidenteConTipo.TipoIncidente.Gravedad.Equals("Alto", StringComparison.OrdinalIgnoreCase)))
                    {
                        // Preparar y enviar el correo solo si la gravedad es "Medio" o "Alto"
                        var proyecto = await _context.Proyectos
                            .Include(p => p.Coordinador)
                            .FirstOrDefaultAsync(p => p.Id == incidenteConTipo.ProyectoId);

                        if (proyecto != null && proyecto.Coordinador != null)
                        {
                            var coordinador = await _userManager.FindByIdAsync(proyecto.CoordinadorId);
                            var horaIncidente = incidenteConTipo.HoraIncidente.HasValue
                                ? incidenteConTipo.HoraIncidente.Value.ToString("HH:mm")
                                : "Hora desconocida";

                            var cantidadAfectados = incidenteConTipo.Afectados != null && incidenteConTipo.Afectados.Any()
                                ? $"{incidenteConTipo.Afectados.Count} afectado(s) reportado(s)"
                                : "No se han reportado afectados";

                            var subjectCoordinador = "Reporte de Incidente - Acción Requerida";
                            var htmlMessageCoordinador = $@"
                                <p>Estimado/a {coordinador.Nombres},</p>
                                <p>Se ha registrado un incidente <strong>{incidenteConTipo.Titulo}</strong> en el proyecto <strong>{proyecto.Nombre}</strong>. A continuación, se detallan los datos del incidente:</p>
                                <ul>
                                    <li><strong>Fecha del Incidente:</strong> {incidenteConTipo.FechaIncidente.ToString("dd/MM/yyyy")}</li>
                                    <li><strong>Hora del Incidente:</strong> {horaIncidente}</li>
                                    <li><strong>Categoría:</strong> {incidenteConTipo.TipoIncidente.CategoriaDescripcion}</li>
                                    <li><strong>Título:</strong> {incidenteConTipo.Titulo}</li>
                                    <li><strong>Gravedad:</strong> {incidenteConTipo.TipoIncidente.Gravedad}</li>
                                    <li><strong>Afectados:</strong> {cantidadAfectados}</li>
                                </ul>
                                <p><strong>Descripción del Incidente:</strong> {incidenteConTipo.Descripcion}</p>
                                <p>Le solicitamos que revise el incidente a la mayor brevedad y tome las medidas necesarias para mitigar cualquier riesgo adicional.</p>
                                <p>Saludos cordiales,</p>
                                <p>El equipo de <strong>Buildoc</strong></p>";

                            await _emailSender.SendEmailAsync(coordinador.Email, subjectCoordinador, htmlMessageCoordinador);
                        }
                    }
                }

                TempData["SuccessMessage"] = "¡El incidente se ha creado exitosamente!";
                return Json(new { success = true });
            }

            // Si llegamos aquí, hubo algún error en el modelo
            // Reutilizamos la variable userId en lugar de declararla nuevamente
            var proyectos = _context.Proyectos
            .Where(p => p.CoordinadorId == userId && p.Estado == Proyecto.EstadoProyecto.EnCurso)
        .ToList();

            ViewData["ProyectoId"] = new SelectList(proyectos, "Id", "Nombre", incidente.ProyectoId);
            ViewData["TipoIncidenteId"] = new SelectList(_context.TipoIncidentes, "Id", "Titulo", incidente.TipoIncidenteId);

            // Pasar nuevamente las categorías a la vista en caso de error
            var categorias = Enum.GetValues(typeof(CategoriaEnum))
                                 .Cast<CategoriaEnum>()
                                 .Select(c => new { Id = (int)c, Name = c.GetDescription() })
                                 .ToList();

            ViewData["CategoriaTipoIncidente"] = new SelectList(categorias, "Id", "Name");

            return PartialView(incidente);
        }

        // GET: Incidentes/Edit/5
        [Authorize(Roles = "Coordinador")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incidente = await _context.Incidentes
                .Include(i => i.Afectados)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (incidente == null)
            {
                return NotFound();
            }
            ViewData["ProyectoId"] = new SelectList(_context.Proyectos, "Id", "Nombre", incidente.ProyectoId);
            ViewData["TipoIncidenteId"] = new SelectList(_context.TipoIncidentes, "Id", "Id", incidente.TipoIncidenteId);
            return PartialView(incidente);
        }

        // POST: Incidentes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Coordinador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Titulo,FechaCreacion,Descripcion,FechaIncidente,Estado,ProyectoId,TipoIncidenteId")] Incidente incidente, List<Afectado> afectados, bool switchAfectados)
        {
            if (id != incidente.Id)
            {
                return NotFound();
            }
            if (!switchAfectados)
            {
                // Limpiar el objeto afectados si el switch no está marcado
                afectados = new List<Afectado>();
                afectados.Clear();
                ModelState.Remove("Afectados");
                ModelState.Remove("Afectados[0].Nombre");
                ModelState.Remove("Afectados[0].Apellido");
                ModelState.Remove("Afectados[0].CorreoElectronico");
                ModelState.Remove("Afectados[0].Cedula");
                ModelState.Remove("Afectados[0].Defuncion");
                ModelState.Remove("Afectados[0].ActividadRealizada");
                ModelState.Remove("Afectados[0].AsociadaProyecto");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener incidente existente para mantener el UsuarioId original
                    var existingIncidente = await _context.Incidentes
                        .FirstOrDefaultAsync(i => i.Id == incidente.Id);
                    if (existingIncidente == null)
                    {
                        return NotFound();
                    }
                    incidente.UsuarioId = incidente.UsuarioId;
                    // Actualizar los valores del tipo de incidente
                    _context.Entry(existingIncidente).CurrentValues.SetValues(incidente);
                    _context.Update(existingIncidente);
                    await _context.SaveChangesAsync();
                    // Solo agregar afectados si el switch está marcado
                    if (switchAfectados)
                    {
                        //await CreateAfectados(afectados, incidente.Id);
                    }
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
                TempData["SuccessMessage"] = "¡El incidente se ha editado exitosamente!";
                return Json(new { success = true });
            }
            ViewData["ProyectoId"] = new SelectList(_context.Proyectos, "Id", "Nombre", incidente.ProyectoId);
            ViewData["TipoIncidenteId"] = new SelectList(_context.TipoIncidentes, "Id", "Titulo", incidente.TipoIncidenteId);
            return PartialView(incidente);
        }

        // GET: Incidentes/Delete/5
        [Authorize(Roles = "Coordinador")]
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
                .Include(i => i.Afectados)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (incidente == null)
            {
                return NotFound();
            }

            return View(incidente);
        }

        // POST: Incidentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Coordinador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var incidente = await _context.Incidentes.FindAsync(id);
            if (incidente != null)
            {
                // Cambiar el estado del incidente a true (archivado)
                incidente.Estado = true;

                // Actualizar el incidente en el contexto
                _context.Update(incidente);

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(IncidenteArchivados));
        }

        private bool IncidenteExists(Guid id)
        {
            return _context.Incidentes.Any(e => e.Id == id);
        }
    }
}
