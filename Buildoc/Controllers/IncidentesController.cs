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
        public async Task<IActionResult> Index(Guid? proyectoId)
        {
            // Obtener el usuario logueado
            var usuarioLogueado = await _userManager.GetUserAsync(User);
            var IdUsuario = usuarioLogueado.Id;

            // Obtén el rol del usuario logueado
            var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(usuarioLogueado.Id));
            var rolUsuario = roles.FirstOrDefault();
            if (usuarioLogueado == null)
            {
                return Unauthorized(); // Si no se puede obtener el usuario logueado, retorna no autorizado
            }
            List<Incidente> todosIncidentes = new List<Incidente>(); // Declarar la variable fuera del if-else
                                                                     // Lista de proyectos para el select
            List<Proyecto> proyectosDisponibles = new List<Proyecto>();
            if (rolUsuario == "Coordinador")
            {
                // Obtener los proyectos donde el usuario logueado es el coordinador para pasarlo al select del filtro
                proyectosDisponibles = await _context.Proyectos
                    .Where(p => p.CoordinadorId == usuarioLogueado.Id)
                    .ToListAsync();
                // Obtener los proyectos donde el usuario logueado es el coordinador
                var proyectosDondeEsCoordinador = await _context.Proyectos
                    .Where(p => p.CoordinadorId == usuarioLogueado.Id)
                    .Select(p => p.Id)
                    .ToListAsync();
                // Obtener todos los incidentes asociados a esos proyectos
                todosIncidentes = await _context.Incidentes
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoIncidente)
                    .Include(i => i.Usuario)
                    .Where(i => proyectosDondeEsCoordinador.Contains(i.ProyectoId))
                    .ToListAsync();
            }
            else if (rolUsuario == "Residente")
            {
                // Obtener los proyectos donde esta asignado el residente
                proyectosDisponibles = await _context.Proyectos
                    .Where(p => p.Residentes.Any(r => r.Id == IdUsuario))
                    .ToListAsync();

                // Obtener los incidentes y lesionados reportados por el usuario logueado
                todosIncidentes = await _context.Incidentes
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoIncidente)
                    .Include(i => i.Usuario)
                    .Include(i => i.IncidenteLesionados)
                    .Where(i => i.UsuarioId == usuarioLogueado.Id)
                    .ToListAsync();
            }
            else
            {
                // Manejar otros roles o el caso en que el rol no sea "Coordinador" ni "Residente"
                todosIncidentes = new List<Incidente>(); // O maneja esto de acuerdo a tus necesidades
            }

            // Obtener todos los tipos de incidentes para el select
            var tiposIncidentes = await _context.TipoIncidentes.ToListAsync();

            // Obtener la cantidad total de lesionados asociados a esos incidentes
            var totalLesionados = await _context.IncidenteLesionados
                .Where(il => todosIncidentes.Select(i => i.Id).Contains(il.IncidenteId))
                .CountAsync();

            // Filtrar incidentes activos 
            var incidentesActivos = todosIncidentes
                .Where(i => i.Estado == EstadoIncidenteEnum.Activo)
                .ToList();

            // Filtrar incidentes solucionados 
            var incidentesArchivados = todosIncidentes
                .Where(i => i.Estado == EstadoIncidenteEnum.Solucionado)
                .ToList();
            // Filtrar incidentes cerrados 
            var incidentesCerrados = todosIncidentes
                .Where(i => i.Estado == EstadoIncidenteEnum.Cerrado)
                .ToList();
            // Filtrar incidentes vencidos 
            var incidentesVencidos = todosIncidentes
                .Where(i => i.Estado == EstadoIncidenteEnum.Vencido)
                .ToList();

            // Contadores
            var totalesIncidentes = todosIncidentes.Count();
            var activosIncidentes = incidentesActivos.Count();
            var archivadosIncidentes = incidentesArchivados.Count();
            var cerradosIncidentes = incidentesCerrados.Count();
            var vencidosIncidentes = incidentesVencidos.Count();

            // Pasar contadores a la vista
            ViewBag.TotalesIncidentes = totalesIncidentes;
            ViewBag.ActivosIncidentes = activosIncidentes;
            ViewBag.ArchivadosIncidentes = archivadosIncidentes;
            ViewBag.CerradosIncidentes = cerradosIncidentes;
            ViewBag.VencidosIncidentes = vencidosIncidentes;
            ViewBag.TotalLesionados = totalLesionados;


            // Retornar solo los incidentes activos para la vista Index
            return View(todosIncidentes);
        }

        public async Task<IActionResult> IncidenteActivos()
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

            List<Incidente> todosIncidentes = new List<Incidente>(); // Declarar la variable fuera del if-else
            if (rolUsuario == "Coordinador")
            {
                // Obtener los proyectos donde el usuario logueado es el coordinador
                var proyectosDondeEsCoordinador = await _context.Proyectos
                    .Where(p => p.CoordinadorId == usuarioLogueado.Id)
                    .Select(p => p.Id)
                    .ToListAsync();
                // Obtener todos los incidentes asociados a esos proyectos
                todosIncidentes = await _context.Incidentes
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoIncidente)
                    .Include(i => i.Usuario)
                    .Where(i => proyectosDondeEsCoordinador.Contains(i.ProyectoId))
                    .ToListAsync();
            }
            else if (rolUsuario == "Residente")
            {
                // Obtener los incidentes y lesionados reportados por el usuario logueado
                todosIncidentes = await _context.Incidentes
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoIncidente)
                    .Include(i => i.Usuario)
                    .Include(i => i.IncidenteLesionados)
                    .Where(i => i.UsuarioId == usuarioLogueado.Id)
                    .ToListAsync();
            }
            else
            {
                // Manejar otros roles o el caso en que el rol no sea "Coordinador" ni "Residente"
                todosIncidentes = new List<Incidente>(); // O maneja esto de acuerdo a tus necesidades
            }

            // Filtrar incidentes archivados (estado false)
            var incidentesArchivados = todosIncidentes
                .Where(i => i.Estado == EstadoIncidenteEnum.Activo)
                .ToList();

            // Retornar solo los incidentes activos para la vista Index
            return View(incidentesArchivados);
        }

        public async Task<IActionResult> IncidenteCerrados()
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

            List<Incidente> todosIncidentes = new List<Incidente>(); // Declarar la variable fuera del if-else
            if (rolUsuario == "Coordinador")
            {
                // Obtener los proyectos donde el usuario logueado es el coordinador
                var proyectosDondeEsCoordinador = await _context.Proyectos
                    .Where(p => p.CoordinadorId == usuarioLogueado.Id)
                    .Select(p => p.Id)
                    .ToListAsync();
                // Obtener todos los incidentes asociados a esos proyectos
                todosIncidentes = await _context.Incidentes
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoIncidente)
                    .Include(i => i.Usuario)
                    .Where(i => proyectosDondeEsCoordinador.Contains(i.ProyectoId))
                    .ToListAsync();
            }
            else if (rolUsuario == "Residente")
            {
                // Obtener los incidentes y lesionados reportados por el usuario logueado
                todosIncidentes = await _context.Incidentes
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoIncidente)
                    .Include(i => i.Usuario)
                    .Include(i => i.IncidenteLesionados)
                    .Where(i => i.UsuarioId == usuarioLogueado.Id)
                    .ToListAsync();
            }
            else
            {
                // Manejar otros roles o el caso en que el rol no sea "Coordinador" ni "Residente"
                todosIncidentes = new List<Incidente>(); // O maneja esto de acuerdo a tus necesidades
            }

            // Filtrar incidentes archivados (estado false)
            var incidentesArchivados = todosIncidentes
                .Where(i => i.Estado == EstadoIncidenteEnum.Cerrado)
                .ToList();

            // Retornar solo los incidentes activos para la vista Index
            return View(incidentesArchivados);
        }

        public async Task<IActionResult> IncidenteVencidos()
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
                .Where(i => i.Estado == EstadoIncidenteEnum.Vencido)
                .ToList();

            // Retornar solo los incidentes activos para la vista Index
            return View(incidentesArchivados);
        }
        public async Task<IActionResult> IncidenteArchivados()
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

            List<Incidente> todosIncidentes = new List<Incidente>(); // Declarar la variable fuera del if-else
            if (rolUsuario == "Coordinador")
            {
                // Obtener los proyectos donde el usuario logueado es el coordinador
                var proyectosDondeEsCoordinador = await _context.Proyectos
                    .Where(p => p.CoordinadorId == usuarioLogueado.Id)
                    .Select(p => p.Id)
                    .ToListAsync();
                // Obtener todos los incidentes asociados a esos proyectos
                todosIncidentes = await _context.Incidentes
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoIncidente)
                    .Include(i => i.Usuario)
                    .Where(i => proyectosDondeEsCoordinador.Contains(i.ProyectoId))
                    .ToListAsync();
            }
            else if (rolUsuario == "Residente")
            {
                // Obtener los incidentes y lesionados reportados por el usuario logueado
                todosIncidentes = await _context.Incidentes
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoIncidente)
                    .Include(i => i.Usuario)
                    .Include(i => i.IncidenteLesionados)
                    .Where(i => i.UsuarioId == usuarioLogueado.Id)
                    .ToListAsync();
            }
            else
            {
                // Manejar otros roles o el caso en que el rol no sea "Coordinador" ni "Residente"
                todosIncidentes = new List<Incidente>(); // O maneja esto de acuerdo a tus necesidades
            }

            // Filtrar incidentes archivados (estado false)
            var incidentesArchivados = todosIncidentes
                .Where(i => i.Estado == EstadoIncidenteEnum.Solucionado)
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
                .Include(i => i.IncidenteLesionados)
                    .ThenInclude(il => il.Lesionado) // Incluye la entidad Lesionado
                .Include(i => i.NovedadesIncidentes)
                    .ThenInclude(s => s.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (incidente == null)
            {
                return NotFound();
            }

            return PartialView(incidente);
        }


        // GET: Incidentes/Create
        public async Task<IActionResult> Create()
        {
            // Obtener el ID del usuario actual
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Obtén el rol del usuario logueado
            var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId));
            var rolUsuario = roles.FirstOrDefault();
            List<Proyecto> proyectos = new List<Proyecto>();

            if (rolUsuario == "Coordinador")
            {
                // Obtener los proyectos creados por el coordinador logeado y que estén en estado "EnCurso"
                proyectos = await _context.Proyectos
                    .Where(p => p.CoordinadorId == userId && p.Estado == Proyecto.EstadoProyecto.EnCurso)
                    .ToListAsync();
            }
            else if (rolUsuario == "Residente")
            {
                // Obtener los proyectos en los que el residente logueado está asignado y que estén en estado "EnCurso"
                proyectos = await _context.Proyectos
                    .Where(p => p.Residentes.Any(r => r.Id == userId) && p.Estado == Proyecto.EstadoProyecto.EnCurso)
                    .ToListAsync();
            }
            else
            {
                proyectos = new List<Proyecto>();
            }

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
        [HttpGet]
        public async Task<IActionResult> GetLesionadoByCedula(long cedula)
        {
            var lesionado = await _context.Lesionados.FirstOrDefaultAsync(l => l.Cedula == cedula);
            if (lesionado == null)
            {
                return Json(new { success = false, message = "Lesionado no encontrado" });
            }

            return Json(new { success = true, data = lesionado });
        }

        // POST: Incidentes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IncidenteViewModel model, bool switchAfectados, string CategoriaTipoIncidente)
        {
            if (!switchAfectados)
            {
                // Elimina las validaciones relacionadas con 'Lesionado' e 'IncidenteLesionado'
                foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Lesionado") || k.StartsWith("IncidenteLesionado")).ToList())
                {
                    ModelState.Remove(key);
                }
                // Elimina los datos relacionados con los afectados si el switch no está activado
                model.Lesionados = null;
                model.IncidenteLesionados = null;
            }
            else
            {
                // Si se activan los afectados, valida los datos
                if (model.Lesionados == null || model.IncidenteLesionados == null)
                {
                    return Json(new { success = false, message = "Los datos del lesionado estan incompletos o mal diligenciados" });
                }
                foreach (var lesionado in model.Lesionados)
                {
                    // Validar que la cédula esté completa (no sea null)
                    if (lesionado.Cedula == null)
                    {
                        return Json(new { success = false, message = "La cédula del lesionado no está completa." });
                    }

                    // Validar que la cédula tenga entre 7 y 10 dígitos
                    if (lesionado.Cedula.Value.ToString().Length < 7 || lesionado.Cedula.Value.ToString().Length > 10)
                    {
                        return Json(new { success = false, message = "La cédula debe tener entre 7 y 10 dígitos." });
                    }

                    // Validar que la cédula no sea un número negativo
                    if (lesionado.Cedula < 0)
                    {
                        return Json(new { success = false, message = "La cédula no puede contener números negativos." });
                    }
                }
            }
            // Validar que la fecha del incidente no sea mayor a la fecha actual
            if (model.Incidente.FechaIncidente > DateOnly.FromDateTime(DateTime.Today))
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
                model.Incidente.Id = Guid.NewGuid();
                model.Incidente.UsuarioId = userId;
                model.Incidente.Estado = EstadoIncidenteEnum.Activo;

                // Agregar el incidente a la base de datos
                _context.Add(model.Incidente);
                await _context.SaveChangesAsync();
                if (switchAfectados)
                {
                    // Si se proporcionaron datos del lesionado
                    if (model.Lesionados != null && model.Lesionados.Count > 0 && model.IncidenteLesionados != null && model.IncidenteLesionados.Count > 0)
                    {
                        for (int i = 0; i < model.Lesionados.Count; i++)
                        {
                            var lesionado = model.Lesionados[i];
                            var incidenteLesionado = model.IncidenteLesionados[i];

                            // Verificar si el lesionado ya existe en la base de datos por cédula
                            var existingLesionado = await _context.Lesionados.FirstOrDefaultAsync(l => l.Cedula == lesionado.Cedula);

                            if (existingLesionado != null)
                            {
                                // Si existe, usar el ID del lesionado existente
                                incidenteLesionado.LesionadoId = existingLesionado.Id;
                            }
                            else
                            {
                                // Si no existe, crear un nuevo lesionado
                                lesionado.Id = Guid.NewGuid(); // Asigna un nuevo ID al lesionado
                                _context.Add(lesionado);
                                await _context.SaveChangesAsync();

                                incidenteLesionado.LesionadoId = lesionado.Id;
                            }

                            incidenteLesionado.IncidenteId = model.Incidente.Id;
                            _context.Add(incidenteLesionado);
                        }
                        await _context.SaveChangesAsync();
                    }
                }

                // Obtener el incidente con su TipoIncidente
                var incidenteConTipo = await _context.Incidentes
                    .Include(i => i.TipoIncidente)
                    .Include(i => i.IncidenteLesionados) // Incluye la relación con IncidenteLesionados
                        .ThenInclude(il => il.Lesionado)  // Incluye también los datos del lesionado
                    .FirstOrDefaultAsync(i => i.Id == model.Incidente.Id);

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

                            // Cuenta la cantidad de lesionados
                            var cantidadAfectados = incidenteConTipo.IncidenteLesionados != null && incidenteConTipo.IncidenteLesionados.Any()
                                ? $"{incidenteConTipo.IncidenteLesionados.Count} lesionado(s) reportado(s)"
                                : null; // Si no hay lesionados, no se mostrará nada

                            var subjectCoordinador = "Reporte de Incidente - Acción Requerida";
                            var htmlMessageCoordinador = $@"
                                <p>Estimado/a {coordinador.Nombres},</p>
                                <p>Se ha registrado un incidente <strong>{incidenteConTipo.Titulo}</strong> en el proyecto <strong>{proyecto.Nombre}</strong>. A continuación, se detallan los datos del incidente:</p>
                                <ul>
                                    <li><strong>Fecha del Incidente:</strong> {incidenteConTipo.FechaIncidente.ToString("dd/MM/yyyy")}</li>
                                    <li><strong>Hora del Incidente:</strong> {horaIncidente}</li>
                                    <li><strong>Categoría:</strong> {incidenteConTipo.TipoIncidente.CategoriaDescripcion}</li>
                                    <li><strong>Título:</strong> {incidenteConTipo.Titulo}</li>
                                    <li><strong>Gravedad:</strong> {incidenteConTipo.TipoIncidente.Gravedad}</li>";

                                    // Solo añadimos la línea de lesionados si hay lesionados reportados
                                    if (!string.IsNullOrEmpty(cantidadAfectados))
                                    {
                                        htmlMessageCoordinador += $"<li><strong>Cantidad de Lesionados:</strong> {cantidadAfectados}</li>";
                                    }

                                 htmlMessageCoordinador += $@"
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

            ViewData["ProyectoId"] = new SelectList(proyectos, "Id", "Nombre", model.Incidente.ProyectoId);
            ViewData["TipoIncidenteId"] = new SelectList(_context.TipoIncidentes, "Id", "Titulo", model.Incidente.TipoIncidenteId);

            // Pasar nuevamente las categorías a la vista en caso de error
            var categorias = Enum.GetValues(typeof(CategoriaEnum))
                                 .Cast<CategoriaEnum>()
                                 .Select(c => new { Id = (int)c, Name = c.GetDescription() })
                                 .ToList();

            ViewData["CategoriaTipoIncidente"] = new SelectList(categorias, "Id", "Name");

            return PartialView(model.Incidente);
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
                //.Include(i => i.Afectados)
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
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Titulo,FechaCreacion,Descripcion,FechaIncidente,Estado,ProyectoId,TipoIncidenteId")] Incidente incidente, /*List<Afectado> afectados*/ bool switchAfectados)
        {
            if (id != incidente.Id)
            {
                return NotFound();
            }
            if (!switchAfectados)
            {
                // Limpiar el objeto afectados si el switch no está marcado
                //afectados = new List<Afectado>();
                //afectados.Clear();
                //ModelState.Remove("Afectados");
                //ModelState.Remove("Afectados[0].Nombre");
                //ModelState.Remove("Afectados[0].Apellido");
                //ModelState.Remove("Afectados[0].CorreoElectronico");
                //ModelState.Remove("Afectados[0].Cedula");
                //ModelState.Remove("Afectados[0].Defuncion");
                //ModelState.Remove("Afectados[0].ActividadRealizada");
                //ModelState.Remove("Afectados[0].AsociadaProyecto");
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
                //.Include(i => i.Afectados)
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
                incidente.Estado = EstadoIncidenteEnum.Activo;

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
