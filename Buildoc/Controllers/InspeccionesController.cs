using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Buildoc.Data;
using Buildoc.Models;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using System.Reflection;
using Buildoc.Models.Inspecciones;
using Buildoc.Services;


namespace Buildoc.Controllers
{
    public class InspeccionesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<Usuario> _userManager;
        private readonly ILogger<InspeccionesController> _logger;
        private readonly IFileService _fileService;

        public InspeccionesController(IFileService fileService, IEmailSender emailSender, ApplicationDbContext context, UserManager<Usuario> userManager, ILogger<InspeccionesController> logger)
        {
            _fileService = fileService;
            _context = context;
            _emailSender = emailSender;
            _userManager = userManager;
            _logger = logger;
        }


        private async Task<IEnumerable<Proyecto>> GetProyectosForCoordinadorAsync()
        {
            var coordinadorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.Proyectos
                                 .Where(p => p.CoordinadorId == coordinadorId && p.Estado == Proyecto.EstadoProyecto.EnCurso)
                                 .ToListAsync();
        }
        //Detalles dle tipo de inspeccion para el create
        public async Task<IActionResult> GetTipoInspeccionDetails(int id)
        {
            var tipoInspeccion = await _context.TipoInspeccion
                .Where(t => t.Id == id)
                .Select(t => new
                {
                    t.Nombre,
                    t.Categoria,
                    t.Descripcion
                })
                .FirstOrDefaultAsync();

            if (tipoInspeccion == null)
            {
                return NotFound();
            }

            return Json(tipoInspeccion);
        }


        // GET: Inspecciones
        public async Task<IActionResult> Index()
        {
            // Obtén el ID del usuario logueado
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtén el rol del usuario logueado (asumiendo que solo tiene un rol)
            var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(usuarioId));
            var rolUsuario = roles.FirstOrDefault();

            List<Inspeccion> inspecciones = new List<Inspeccion>();

            if (rolUsuario == "Administrador")
            {
                // Mostrar todas las inspecciones si es administrador
                inspecciones = await _context.Inspeccion
                    .Include(i => i.Inspector)
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoInspeccion)
                    .ToListAsync();
            }
            else if (rolUsuario == "Coordinador")
            {
                // Obtén los proyectos asociados al coordinador logueado
                var proyectos = await _context.Proyectos
                                              .Where(p => p.CoordinadorId == usuarioId)
                                              .Select(p => p.Id)
                                              .ToListAsync();

                // Filtra las inspecciones basadas en los proyectos asociados al coordinador
                inspecciones = await _context.Inspeccion
                    .Include(i => i.Inspector)
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoInspeccion)
                    .Where(i => proyectos.Contains(i.ProyectoId))
                    .ToListAsync();
            }
            else if (rolUsuario == "Residente")
            {
                // Filtra las inspecciones en las que el usuario es el inspector
                inspecciones = await _context.Inspeccion
                    .Include(i => i.Inspector)
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoInspeccion)
                    .Where(i => i.InspectorId == usuarioId)
                    .ToListAsync();
            }

            // Calcula el número de inspecciones por estado
            var countProgramadas = inspecciones.Count(i => i.Estado == EstadoInspeccion.Programada);
            var countPendienteRevision = inspecciones.Count(i => i.Estado == EstadoInspeccion.PendientesDeRevision);
            var countSinResponder = inspecciones.Count(i => i.Estado == EstadoInspeccion.SinResponder);
            var countAprobadas = inspecciones.Count(i => i.Estado == EstadoInspeccion.Aprobada);
            var countDesaprobadas = inspecciones.Count(i => i.Estado == EstadoInspeccion.Desaprobada);

            // Obtén los municipios asociados a las inspecciones del usuario (sin duplicados)
            var municipiosConInspecciones = inspecciones
                .Select(i => i.Proyecto.Municipio)
                .Distinct()
                .ToList();

            // Obtener los detalles de las inspecciones
            var detallesInspecciones = inspecciones
                .Select(i => new
                {
                    i.Id,
                    i.Estado,
                    Municipio = i.Proyecto.Municipio
                })
                .ToList();

            var fechaLimite = DateTime.Now.AddDays(7);

            ViewBag.InspeccionesPorInspector = inspecciones
                .Where(i => i.Estado == EstadoInspeccion.Programada && i.FechaInspeccion >= DateTime.Now && i.FechaInspeccion <= fechaLimite)
                .GroupBy(i => i.Inspector.NombreCompleto)
                .Select(g => new { inspector = g.Key, cantidad = g.Count() })
                .ToList();


            var inspeccionesPorTipo = inspecciones
        .GroupBy(i => i.TipoInspeccion.Nombre)  // Agrupa por el nombre del tipo de inspección
        .Select(g => new
        {
            tipo = g.Key,  // El nombre del tipo de inspección
            cantidad = g.Count()  // Número de inspecciones por tipo
        })
        .ToList();

            // Pasar los datos al ViewBag para la vista
            ViewBag.InspeccionesPorTipo = inspeccionesPorTipo;

            var now = DateTime.Now;

            // Filtrar y agrupar las inspecciones por el tiempo restante hasta su fecha de vencimiento
            var inspeccionesPorTiempoRestante = inspecciones
            .Where(i => i.FechaInspeccion > now)  // Solo inspecciones futuras
            .Select(i => new
            {
                categoria = i.FechaInspeccion <= now.AddHours(24) ? "Próximas 24 horas" :
                            i.FechaInspeccion <= now.AddDays(3) ? "Próximos 3 días" :
                            i.FechaInspeccion <= now.AddDays(10) ? "Próximos 10 días" :
                            i.FechaInspeccion <= now.AddDays(30) ? "Próximos 30 días" : "Más de 30 días",
                cantidad = 1
            })
            .GroupBy(i => i.categoria)
            .Select(g => new
            {
                categoria = g.Key,
                cantidad = g.Count()
            })
            .ToList();

            ViewBag.InspeccionesPorTiempoRestante = inspeccionesPorTiempoRestante;



            // Calcular inspecciones programadas por proyecto
            var inspeccionesPorProyecto = inspecciones
                .Where(i => i.Estado == EstadoInspeccion.Programada)
                .GroupBy(i => i.Proyecto.Nombre)
                .Select(group => new
                {
                    Proyecto = group.Key,
                    Cantidad = group.Count()
                })
                .ToList();

            // Cálculo del tiempo restante o indicar si la inspección no ha comenzado
            var inspeccionesConTiempo = inspecciones.Select(i => new
            {
                i.Id,
                i.Objetivo,
                i.FechaInspeccion,
                i.Estado,
                TiempoRestante = (i.FechaInspeccion <= DateTime.Now && i.DuracionHoras.HasValue)
                    ? i.FechaInspeccion.AddHours(i.DuracionHoras.Value) - DateTime.Now
                    : (TimeSpan?)null
            }).ToList();

            // Pasa los datos a la vista
            ViewBag.UsuarioId = usuarioId;
            ViewBag.CountProgramadas = countProgramadas;
            ViewBag.CountPendienteRevision = countPendienteRevision;
            ViewBag.CountSinResponder = countSinResponder;
            ViewBag.CountAprobadas = countAprobadas;
            ViewBag.CountDesaprobadas = countDesaprobadas;
            ViewBag.MunicipiosConInspecciones = municipiosConInspecciones;
            ViewBag.DetallesInspecciones = detallesInspecciones;

            ViewBag.InspeccionesConTiempo = inspeccionesConTiempo;
            ViewBag.InspeccionesPorProyecto = inspeccionesPorProyecto; // Agrega este ViewBag

            return View(inspecciones);
        }



        // GET: Inpsecciones Aprobadas
        public async Task<IActionResult> Aprobadas()
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (User.IsInRole("Administrador"))
            {
                // Mostrar todas las inspecciones aprobadas para el Administrador
                var inspeccionesAprobadas = await _context.Inspeccion
                    .Include(i => i.Inspector)
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoInspeccion)
                    .Where(p => p.Estado == EstadoInspeccion.Aprobada)
                    .ToListAsync();

                return View(inspeccionesAprobadas);
            }

            if (User.IsInRole("Coordinador"))
            {
                // Si es Coordinador, filtrar por proyectos en los que es coordinador
                var proyectos = await _context.Proyectos
                                              .Where(p => p.CoordinadorId == usuarioId)
                                              .Select(p => p.Id)
                                              .ToListAsync();

                var inspeccionesAprobadas = await _context.Inspeccion
                    .Include(i => i.Inspector)
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoInspeccion)
                    .Where(p => p.Estado == EstadoInspeccion.Aprobada && proyectos.Contains(p.ProyectoId))
                    .ToListAsync();

                return View(inspeccionesAprobadas);
            }

            if (User.IsInRole("Residente"))
            {
                // Si es Residente, mostrar las inspecciones donde es el Inspector asignado
                var inspeccionesAprobadas = await _context.Inspeccion
                    .Include(i => i.Inspector)
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoInspeccion)
                    .Where(i => i.InspectorId == usuarioId && i.Estado == EstadoInspeccion.Aprobada)
                    .ToListAsync();

                return View(inspeccionesAprobadas);
            }

            return Unauthorized();
        }


        // GET: Inpsecciones Desaprobadas
        public async Task<IActionResult> Desaprobadas()
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (User.IsInRole("Administrador"))
            {
                var inspeccionesDesaprobadas = await _context.Inspeccion
                    .Include(i => i.Inspector)
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoInspeccion)
                    .Where(p => p.Estado == EstadoInspeccion.Desaprobada)
                    .ToListAsync();

                return View(inspeccionesDesaprobadas);
            }

            if (User.IsInRole("Coordinador"))
            {
                var proyectos = await _context.Proyectos
                                              .Where(p => p.CoordinadorId == usuarioId)
                                              .Select(p => p.Id)
                                              .ToListAsync();

                var inspeccionesDesaprobadas = await _context.Inspeccion
                    .Include(i => i.Inspector)
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoInspeccion)
                    .Where(p => p.Estado == EstadoInspeccion.Desaprobada && proyectos.Contains(p.ProyectoId))
                    .ToListAsync();

                return View(inspeccionesDesaprobadas);
            }

            if (User.IsInRole("Residente"))
            {
                var inspeccionesDesaprobadas = await _context.Inspeccion
                    .Include(i => i.Inspector)
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoInspeccion)
                    .Where(i => i.InspectorId == usuarioId && i.Estado == EstadoInspeccion.Desaprobada)
                    .ToListAsync();

                return View(inspeccionesDesaprobadas);
            }

            return Unauthorized();
        }



        // GET: Inpsecciones Pendientes de Revision
        public async Task<IActionResult> PendientesRevision()
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (User.IsInRole("Administrador"))
            {
                var inspeccionesPendientesRevision = await _context.Inspeccion
                    .Include(i => i.Inspector)
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoInspeccion)
                    .Include(i => i.Respuesta)
                    .Where(i => i.Estado == EstadoInspeccion.PendientesDeRevision)
                    .ToListAsync();

                return View(inspeccionesPendientesRevision);
            }

            if (User.IsInRole("Coordinador"))
            {
                var proyectos = await _context.Proyectos
                                              .Where(p => p.CoordinadorId == usuarioId)
                                              .Select(p => p.Id)
                                              .ToListAsync();

                var inspeccionesPendientesRevision = await _context.Inspeccion
                    .Include(i => i.Inspector)
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoInspeccion)
                    .Include(i => i.Respuesta)
                    .Where(i => proyectos.Contains(i.ProyectoId) && i.Estado == EstadoInspeccion.PendientesDeRevision)
                    .ToListAsync();

                return View(inspeccionesPendientesRevision);
            }

            if (User.IsInRole("Residente"))
            {
                var inspeccionesPendientesRevision = await _context.Inspeccion
                    .Include(i => i.Inspector)
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoInspeccion)
                    .Include(i => i.Respuesta)
                    .Where(i => i.InspectorId == usuarioId && i.Estado == EstadoInspeccion.PendientesDeRevision)
                    .ToListAsync();

                return View(inspeccionesPendientesRevision);
            }

            return Unauthorized();
        }



        // GET: Inpsecciones sin responder
        public async Task<IActionResult> SinResponder()
        {
            var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (User.IsInRole("Administrador"))
            {
                var inspeccionesSinResponder = await _context.Inspeccion
                    .Include(i => i.Inspector)
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoInspeccion)
                    .Where(p => p.Estado == EstadoInspeccion.SinResponder)
                    .ToListAsync();

                return View(inspeccionesSinResponder);
            }

            if (User.IsInRole("Coordinador"))
            {
                var proyectos = await _context.Proyectos
                                              .Where(p => p.CoordinadorId == usuarioId)
                                              .Select(p => p.Id)
                                              .ToListAsync();

                var inspeccionesSinResponder = await _context.Inspeccion
                    .Include(i => i.Inspector)
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoInspeccion)
                    .Where(i => proyectos.Contains(i.ProyectoId) && i.Estado == EstadoInspeccion.SinResponder)
                    .ToListAsync();

                return View(inspeccionesSinResponder);
            }

            if (User.IsInRole("Residente"))
            {
                var inspeccionesSinResponder = await _context.Inspeccion
                    .Include(i => i.Inspector)
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoInspeccion)
                    .Where(i => i.InspectorId == usuarioId && i.Estado == EstadoInspeccion.SinResponder)
                    .ToListAsync();

                return View(inspeccionesSinResponder);
            }

            return Unauthorized();
        }


        // GET Inspecciones programadas
        public async Task<IActionResult> Programadas()
        {
            // Obtén el ID del usuario logueado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IQueryable<Inspeccion> query = _context.Inspeccion
                .Include(i => i.Inspector)
                .Include(i => i.Proyecto)
                .Include(i => i.TipoInspeccion)
                .Include(i => i.Respuesta)
                .Where(i => i.Estado == EstadoInspeccion.Programada);

            if (User.IsInRole("Coordinador"))
            {
                // Si el usuario es un Coordinador, filtra las inspecciones basadas en los proyectos asociados al coordinador
                query = query.Where(i => i.Proyecto.CoordinadorId == userId);
            }
            else if (User.IsInRole("Residente"))
            {
                // Si el usuario es un Residente, filtra las inspecciones en las que el usuario es el Inspector
                query = query.Where(i => i.InspectorId == userId);
            }

            var inspeccionesProgramadas = await query.ToListAsync();
            // Cálculo del tiempo restante o indicar si la inspección no ha comenzado
            var inspeccionesConTiempo = inspeccionesProgramadas.Select(i => new
            {
                i.Id,
                i.Objetivo,
                i.FechaInspeccion,
                i.Estado,
                TiempoRestante = (i.FechaInspeccion <= DateTime.Now && i.DuracionHoras.HasValue)
                    ? i.FechaInspeccion.AddHours(i.DuracionHoras.Value) - DateTime.Now
                    : (TimeSpan?)null
            }).ToList();
            ViewBag.UsuarioId = userId;
            ViewBag.InspeccionesConTiempo = inspeccionesConTiempo;
            return View(inspeccionesProgramadas);
        }



        // GET: Inspecciones/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var inspeccion = await _context.Inspeccion
                    .Include(i => i.Inspector)
                    .Include(i => i.Proyecto)
                    .Include(i => i.TipoInspeccion)
                        .ThenInclude(n => n.Archivos)
                    .Include(i => i.Novedades)
                     .ThenInclude(n => n.Usuario)
                        .Include(i => i.Novedades)
                .ThenInclude(n => n.FileModels) // Archivos asociados a las novedades
                    .Include(i => i.Respuesta)
                       .ThenInclude(n => n.FileModels)
                    .Include(i => i.FileModels)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (inspeccion == null)
                {
                    return NotFound();
                }

                // Verifica si inspeccion.Respuesta está correctamente poblado
                if (inspeccion.Respuesta != null)
                {
                    // Verifica las propiedades de Respuesta
                    Console.WriteLine($"Respuesta Id: {inspeccion.Respuesta.Id}");
                }

                return View(inspeccion);
            }
            catch (Exception ex)
            {
                // Registra la excepción
                _logger.LogError(ex, "Error al obtener los detalles de la inspección.");
                return StatusCode(500, "Se produjo un error en el servidor.");
            }
        }

        // Método para obtener la descripción de un enum
        public static string GetEnumDisplayName(Enum value)
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            if (memberInfo.Length > 0)
            {
                var attribute = memberInfo[0].GetCustomAttribute<DisplayAttribute>();
                if (attribute != null)
                {
                    return attribute.Name;
                }
            }
            return value.ToString();
        }
        //Categorias 
        [HttpGet]
        public async Task<IActionResult> GetCategoriasConTipoInspeccion()
        {
            var categoriasConTipos = await _context.TipoInspeccion
                .GroupBy(t => t.Categoria)
                .Where(g => g.Any())
                .Select(g => new SelectListItem
                {
                    Value = g.Key.ToString(),
                    Text = GetEnumDisplayName(g.Key)
                })
                .ToListAsync();

            return Json(categoriasConTipos);
        }

        [HttpGet]
        public JsonResult GetTipoInspeccionesPorCategoria(string categoria)
        {
            // Verificar si la cadena de categoría puede ser convertida a un valor del enum
            if (!Enum.TryParse(categoria, out CategoriaInspeccion categoriaEnum))
            {
                // Si no se puede convertir, retornar una lista vacía
                return Json(new List<object>());
            }

            // Filtrar los tipos de inspección según la categoría
            var tiposInspeccion = _context.TipoInspeccion
                .Where(t => t.Categoria == categoriaEnum)
                .Select(t => new
                {
                    id = t.Id,
                    nombre = t.Nombre
                })
                .ToList();

            return Json(tiposInspeccion);
        }
        // GET: Inspecciones/GetArchivosByTipoInspeccion
        public async Task<IActionResult> GetArchivosByTipoInspeccion(int tipoInspeccionId)
        {
            var archivos = await _context.FileModels
                   .Where(a => a.TipoInspeccionId == tipoInspeccionId)
                   .Select(a => new
                   {
                       a.FileName,
                       a.FilePath
                   })
                   .ToListAsync();

            return Json(archivos);
        }
        [HttpGet]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            // Aquí debes obtener la ruta completa del archivo desde el FilePath
            // Suponiendo que tienes una forma de obtener el FilePath usando el fileName
            var filePath = _fileService.GetFilePath(fileName);

            if (string.IsNullOrEmpty(filePath))
            {
                return NotFound(); // Maneja el caso donde el archivo no exista
            }

            return Redirect(filePath);
        }


        // GET: Inspecciones/Create
        public async Task<IActionResult> Create()
        {

            ViewData["ProyectoId"] = new SelectList(await GetProyectosForCoordinadorAsync(), "Id", "Nombre");
            ViewData["TipoInspeccionId"] = new SelectList(_context.TipoInspeccion, "Id", "Nombre");

            return PartialView();
        }

        // POST: Inspecciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InspeccionCreateViewModel model)
        {
            var inspeccion = model.Inspeccion;


            if (!ModelState.IsValid)
            {
                // Obtener errores de validación
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = "Los datos están incompletos o inválidos. Inténtelo nuevamente", errors });
            }

            if (inspeccion.FechaInspeccion < DateTime.Now.Date)
            {
                // Enviar mensaje de error como JSON
                return Json(new { success = false, message = "La fecha de la inspección no puede ser anterior a la fecha actual." });
            }

            // Obtener el proyecto relacionado con la inspección

            var proyecto = await _context.Proyectos.FindAsync(inspeccion.ProyectoId);
            if (proyecto != null && inspeccion.FechaInspeccion > proyecto.FechaFinalizacion)
            {
                return Json(new { success = false, message = "La fecha de la inspección no puede ser mayor que la fecha de finalización del proyecto." });
            }


            // Verificar si el inspector tiene una inspección programada en la misma fecha y hora
            var inspeccionesExistentes = await _context.Inspeccion
                .Where(i => i.InspectorId == inspeccion.InspectorId && i.Estado == EstadoInspeccion.Programada)
                .ToListAsync();

            foreach (var i in inspeccionesExistentes)
            {
                // Verificar si las fechas se superponen
                if (inspeccion.FechaInspeccion.Date == i.FechaInspeccion.Date)
                {
                    if (inspeccion.EsTodoElDia || i.EsTodoElDia)
                    {
                        return Json(new { success = false, message = "El inspector ya tiene una inspección programada para todo el día en esta fecha." });
                    }

                    // Verificar si las duraciones se superponen
                    var inspeccionFin = inspeccion.FechaInspeccion.AddHours(inspeccion.DuracionHoras ?? 0);
                    var inspeccionExistenteFin = i.FechaInspeccion.AddHours(i.DuracionHoras ?? 0);

                    if (inspeccion.FechaInspeccion < inspeccionExistenteFin && inspeccionFin > i.FechaInspeccion)
                    {
                        return Json(new { success = false, message = "El inspector ya tiene una inspección programada que se superpone con la nueva." });
                    }
                }
            }

            // Si el modelo es válido, procede a crear la inspección
            inspeccion.Id = Guid.NewGuid();
            inspeccion.Estado = EstadoInspeccion.Programada;
            inspeccion.FechaProgramacion = DateTime.Now;
            _context.Add(inspeccion);
            await _context.SaveChangesAsync();



            // Manejo de archivos subidos
            if (model.UploadedFiles != null && model.UploadedFiles.Count > 0)
            {
                foreach (var file in model.UploadedFiles)
                {
                    try
                    {
                        if (file.Length > 0)
                        {
                            // Subir archivo y manejar la excepción si excede el límite de tamaño
                            var fileUrl = await _fileService.Upload(file, "documents");
                            _logger.LogInformation($"Archivo: {file.FileName}, URL: {fileUrl}");
                            var fileModel = new FileModel
                            {
                                Id = Guid.NewGuid(),
                                InspeccionId = inspeccion.Id,
                                FileName = Path.GetFileName(file.FileName),
                                FilePath = fileUrl,
                                ContentType = file.ContentType,
                                FileSize = file.Length
                            };

                            _context.FileModels.Add(fileModel);
                        }
                        else
                        {
                            _logger.LogWarning($"Archivo {file.FileName} tiene longitud cero.");
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Manejar la excepción cuando el archivo excede el tamaño permitido
                        return Json(new { success = false, message = ex.Message });
                    }
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("No se recibieron archivos.");
            }
            // Obtener el inspector y proyecto asignados
            var inspector = await _userManager.FindByIdAsync(inspeccion.InspectorId);


            // Preparar el mensaje de correo electrónico en formato HTML para el inspector
            var subject = "Nueva Inspección Asignada";
            var htmlMessage = $@"
<p>Hola {inspector.Nombres},</p>
<p>Se ha creado una nueva inspección para el proyecto '<strong>{proyecto.Nombre}</strong>'.</p>
<p>Fecha de Inspección: {inspeccion.FechaInspeccion.ToShortDateString()}</p>
<p>Objetivo: {inspeccion.Objetivo}</p>
<p>Descripción: {inspeccion.Descripcion}</p>";

            if (inspeccion.EsTodoElDia)
            {
                htmlMessage += "<p>Duración: Todo el día</p>";
            }
            else if (inspeccion.DuracionHoras.HasValue)
            {
                htmlMessage += $"<p>Duración: {inspeccion.DuracionHoras.Value} horas</p>";
            }

            htmlMessage += @"
    <p>Saludos,</p>
    <p>El equipo de <strong>Buildoc</strong></p>";

            // Enviar el correo electrónico al inspector
            await _emailSender.SendEmailAsync(inspector.Email, subject, htmlMessage);




            // Si el modelo no es válido, retornar la vista parcial con los datos existentes
            ViewData["InspectorId"] = new SelectList(_context.Users, "Id", "NombreCompleto", inspeccion.InspectorId);
            ViewData["ProyectoId"] = new SelectList(await GetProyectosForCoordinadorAsync(), "Id", "Nombre", inspeccion.ProyectoId);
            ViewData["TipoInspeccionId"] = new SelectList(_context.TipoInspeccion, "Id", "Nombre", inspeccion.TipoInspeccionId);

            TempData["SuccessMessage"] = "¡La inspección se ha creado exitosamente!";
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspeccion = await _context.Inspeccion
                .Include(i => i.Proyecto)
                .Include(i => i.Inspector)
                .Include(i => i.TipoInspeccion)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inspeccion == null)
            {
                return NotFound();
            }

            // Obtener los archivos relacionados con la inspección
            var archivosInspeccion = await _context.FileModels
                .Where(f => f.InspeccionId == inspeccion.Id)
                .ToListAsync();

            // Obtener los archivos relacionados con el tipo de inspección
            var archivosTipoInspeccion = await _context.FileModels
                .Where(f => f.TipoInspeccionId == inspeccion.TipoInspeccionId)
                .ToListAsync();

            // Crear el ViewModel
            var viewModel = new InspeccionEditViewModel
            {
                Inspeccion = inspeccion,
                ArchivosInspeccion = archivosInspeccion,
                ArchivosTipoInspeccion = archivosTipoInspeccion
            };

            // Poblar los SelectLists en ViewBag
            ViewBag.EstadoList = Enum.GetValues(typeof(EstadoInspeccion))
                .Cast<EstadoInspeccion>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.GetType()
                              .GetField(e.ToString())
                              .GetCustomAttributes(typeof(DisplayAttribute), false)
                              .SingleOrDefault() is DisplayAttribute displayAttribute ? displayAttribute.Name : e.ToString()
                }).ToList();
            // Obtener las categorías que tienen al menos un tipo de inspección
            var categoriasConTipos = await _context.TipoInspeccion
                .GroupBy(t => t.Categoria)
                .Where(g => g.Any())  // Filtra solo las categorías que tienen tipos de inspección
                .Select(g => new SelectListItem
                {
                    Value = g.Key.ToString(),
                    Text = GetEnumDisplayName(g.Key)
                })
                .ToListAsync();
            ViewData["CategoriasInspeccion"] = categoriasConTipos;
            ViewData["InspectorId"] = new SelectList(_context.Users, "Id", "NombreCompleto", inspeccion.InspectorId);
            ViewData["ProyectoId"] = new SelectList(await GetProyectosForCoordinadorAsync(), "Id", "Nombre", inspeccion.ProyectoId);
            ViewData["TipoInspeccionId"] = new SelectList(_context.TipoInspeccion, "Id", "Nombre", inspeccion.TipoInspeccionId);

            // Pasar el estado de la inspección a la vista
            ViewData["Estado"] = inspeccion.Estado;

            // Devolver la vista parcial con el ViewModel
            return PartialView("Edit", viewModel);
        }


        // POST: Inspecciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, InspeccionEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return Json(new { success = false, message = "Los datos están incompletos o inválidos. Inténtelo nuevamente", errors });
            }

            // Cargar la inspección original desde la base de datos
            var inspeccionOriginal = await _context.Inspeccion.FirstOrDefaultAsync(i => i.Id == id);

            if (inspeccionOriginal == null)
            {
                return NotFound();
            }

            // Validar que el estado no permita la edición si está en estados no editables
            if (inspeccionOriginal.Estado == EstadoInspeccion.PendientesDeRevision || inspeccionOriginal.Estado == EstadoInspeccion.Aprobada)
            {
                return Json(new { success = false, message = "No se puede editar una inspección que está pendiente de revisión o finalizada." });
            }

            // Verificar que el ID enviado en el formulario coincide con el ID de la inspección
            if (id != model.Inspeccion.Id)
            {
                return NotFound();
            }

            // Actualizar solo los campos permitidos sin modificar la `FechaCreacion`
            inspeccionOriginal.FechaInspeccion = model.Inspeccion.FechaInspeccion;
            inspeccionOriginal.DuracionHoras = model.Inspeccion.DuracionHoras;
            inspeccionOriginal.EsTodoElDia = model.Inspeccion.EsTodoElDia;
            inspeccionOriginal.Objetivo = model.Inspeccion.Objetivo;
            inspeccionOriginal.Descripcion = model.Inspeccion.Descripcion;
            inspeccionOriginal.InspectorId = model.Inspeccion.InspectorId;
            inspeccionOriginal.TipoInspeccionId = model.Inspeccion.TipoInspeccionId;
            inspeccionOriginal.ProyectoId = model.Inspeccion.ProyectoId;

            // Mantener el estado original sin cambios
            inspeccionOriginal.Estado = inspeccionOriginal.Estado;

            // Validar que no se superpongan las fechas de inspección del inspector
            var inspeccionesExistentes = await _context.Inspeccion
                .Where(i => i.InspectorId == inspeccionOriginal.InspectorId && i.Estado == EstadoInspeccion.Programada && i.Id != id)
                .ToListAsync();

            foreach (var i in inspeccionesExistentes)
            {
                if (model.Inspeccion.FechaInspeccion.Date == i.FechaInspeccion.Date)
                {
                    if (model.Inspeccion.EsTodoElDia || i.EsTodoElDia)
                    {
                        return Json(new { success = false, message = "El inspector ya tiene una inspección programada para todo el día en esta fecha." });
                    }

                    var inspeccionFin = model.Inspeccion.FechaInspeccion.AddHours(model.Inspeccion.DuracionHoras ?? 0);
                    var inspeccionExistenteFin = i.FechaInspeccion.AddHours(i.DuracionHoras ?? 0);

                    if (model.Inspeccion.FechaInspeccion < inspeccionExistenteFin && inspeccionFin > i.FechaInspeccion)
                    {
                        return Json(new { success = false, message = "El inspector ya tiene una inspección programada que se superpone con la nueva." });
                    }
                }
            }

            // Guardar cambios en la base de datos
            try
            {
                _context.Update(inspeccionOriginal);
                await _context.SaveChangesAsync();

                // Manejo de archivos subidos (solo si se suben nuevos archivos)
                if (model.UploadedFiles != null && model.UploadedFiles.Count > 0)
                {
                    foreach (var file in model.UploadedFiles)
                    {
                        if (file.Length > 0)
                        {
                            var fileUrl = await _fileService.Upload(file, "documents");
                            var fileModel = new FileModel
                            {
                                Id = Guid.NewGuid(),
                                InspeccionId = inspeccionOriginal.Id,
                                FileName = Path.GetFileName(file.FileName),
                                FilePath = fileUrl,
                                ContentType = file.ContentType,
                                FileSize = file.Length
                            };

                            _context.FileModels.Add(fileModel);
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                // Notificaciones por correo si hay cambios significativos
                if (inspeccionOriginal.FechaInspeccion != model.Inspeccion.FechaInspeccion ||
                    inspeccionOriginal.Objetivo != model.Inspeccion.Objetivo ||
                    inspeccionOriginal.Descripcion != model.Inspeccion.Descripcion ||
                    inspeccionOriginal.TipoInspeccionId != model.Inspeccion.TipoInspeccionId ||
                    inspeccionOriginal.ProyectoId != model.Inspeccion.ProyectoId ||
                    inspeccionOriginal.InspectorId != model.Inspeccion.InspectorId ||
                    inspeccionOriginal.DuracionHoras != model.Inspeccion.DuracionHoras ||
                    inspeccionOriginal.EsTodoElDia != model.Inspeccion.EsTodoElDia)
                {
                    var inspectorOriginal = await _userManager.FindByIdAsync(inspeccionOriginal.InspectorId);
                    var inspectorNuevo = await _userManager.FindByIdAsync(model.Inspeccion.InspectorId);

                    if (inspeccionOriginal.InspectorId != model.Inspeccion.InspectorId)
                    {
                        // Notificación al inspector original si fue reasignado
                        var subjectOriginal = "Inspección Reasignada";
                        var htmlMessageOriginal = $@"
<p>Hola {inspectorOriginal.Nombres},</p>
<p>La inspección para el proyecto '<strong>{inspeccionOriginal.Proyecto.Nombre}</strong>' programada para el {model.Inspeccion.FechaInspeccion} ha sido reasignada a otro inspector.</p>
<p>Saludos,</p>
<p>El equipo de <strong>Buildoc</strong></p>";
                        await _emailSender.SendEmailAsync(inspectorOriginal.Email, subjectOriginal, htmlMessageOriginal);

                        // Notificación al nuevo inspector
                        var subjectNuevo = "Nueva Inspección Asignada";
                        var htmlMessageNuevo = $@"
<p>Hola {inspectorNuevo.Nombres},</p>
<p>Se le ha asignado una nueva inspección para el proyecto '<strong>{inspeccionOriginal.Proyecto.Nombre}</strong>'.</p>
<p>Fecha de Inspección: {model.Inspeccion.FechaInspeccion}</p>
<p>Objetivo: {model.Inspeccion.Objetivo}</p>
<p>Descripción: {model.Inspeccion.Descripcion}</p>";
                        if (model.Inspeccion.EsTodoElDia)
                        {
                            htmlMessageNuevo += "<p>Duración: Todo el día</p>";
                        }
                        else if (model.Inspeccion.DuracionHoras.HasValue)
                        {
                            htmlMessageNuevo += $"<p>Duración: {model.Inspeccion.DuracionHoras.Value} horas</p>";
                        }
                        htmlMessageNuevo += @"
<p>Saludos,</p>
<p>El equipo de <strong>Buildoc</strong></p>";
                        await _emailSender.SendEmailAsync(inspectorNuevo.Email, subjectNuevo, htmlMessageNuevo);
                    }
                    else
                    {
                        // Notificación si solo se actualizaron detalles
                        var subject = "Inspección Actualizada";
                        var htmlMessage = $@"
<p>Hola {inspectorNuevo.Nombres},</p>
<p>Se han actualizado los detalles de la inspección para el proyecto '<strong>{inspeccionOriginal.Proyecto.Nombre}</strong>'.</p>
<p>Fecha de Inspección: {model.Inspeccion.FechaInspeccion}</p>
<p>Objetivo: {model.Inspeccion.Objetivo}</p>
<p>Descripción: {model.Inspeccion.Descripcion}</p>";
                        if (model.Inspeccion.EsTodoElDia)
                        {
                            htmlMessage += "<p>Duración: Todo el día</p>";
                        }
                        else if (model.Inspeccion.DuracionHoras.HasValue)
                        {
                            htmlMessage += $"<p>Duración: {model.Inspeccion.DuracionHoras.Value} horas</p>";
                        }
                        htmlMessage += @"
<p>Saludos,</p>
<p>El equipo de <strong>Buildoc</strong></p>";
                        await _emailSender.SendEmailAsync(inspectorNuevo.Email, subject, htmlMessage);
                    }
                }

                TempData["SuccessMessage"] = "¡La inspección se ha editado exitosamente!";
                return Json(new { success = true });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InspeccionExists(inspeccionOriginal.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Si hay un error, recargar las listas de selección y devolver la vista
            ViewData["InspectorId"] = new SelectList(_context.Users, "Id", "NombreCompleto", inspeccionOriginal.InspectorId);
            ViewData["ProyectoId"] = new SelectList(await GetProyectosForCoordinadorAsync(), "Id", "Nombre", inspeccionOriginal.ProyectoId);
            ViewData["TipoInspeccionId"] = new SelectList(_context.TipoInspeccion, "Id", "Nombre", inspeccionOriginal.TipoInspeccionId);
            return PartialView("Edit", inspeccionOriginal);
        }

        // GET: Inspecciones/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspeccion = await _context.Inspeccion
                 .Include(i => i.Inspector) // Incluir el inspector
                .Include(i => i.Inspector)
                .Include(i => i.Proyecto)
                .Include(i => i.TipoInspeccion)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inspeccion == null)
            {
                return NotFound();
            }

            return PartialView("Delete", inspeccion);
        }

        // POST: Inspecciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var inspeccion = await _context.Inspeccion
                                  .Include(i => i.Inspector)
                                  .Include(i => i.Proyecto)  // Incluye el proyecto si es necesario
                                  .FirstOrDefaultAsync(i => i.Id == id);
            if (inspeccion == null)
            {
                return Json(new { success = false, message = "Inspección no encontrada." });
            }

            // Verificar si el estado de la inspección es 'Programada'
            if (inspeccion.Estado != EstadoInspeccion.Programada)
            {
                return Json(new { success = false, message = "Solo se pueden eliminar inspecciones en estado 'Programada'." });
            }

            try
            {
                // Verificar si el inspector está disponible
                if (inspeccion.Inspector == null)
                {
                    return Json(new { success = false, message = "No se encontró información del inspector." });
                }
                _context.Inspeccion.Remove(inspeccion);
                await _context.SaveChangesAsync();

                // Enviar correo al inspector
                var inspector = inspeccion.Inspector;
                var subject = "Inspección Eliminada";
                var htmlMessage = $@"
            <p>Hola {inspector.Nombres},</p>
            <p>La inspección programada para el proyecto '<strong>{inspeccion.Proyecto?.Nombre ?? "Desconocido"}</strong>' el {inspeccion.FechaInspeccion} ha sido eliminada.</p>
            <p>Saludos,</p>
            <p>El equipo de <strong>Buildoc</strong></p>";

                await _emailSender.SendEmailAsync(inspector.Email, subject, htmlMessage);

                TempData["SuccessMessage"] = "¡La inspección se ha eliminado exitosamente!";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar la inspección: " + ex.Message });
            }
        }


        private bool InspeccionExists(Guid id)
        {
            return _context.Inspeccion.Any(e => e.Id == id);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeState(Guid id, EstadoInspeccion newState)
        {
            var inspeccion = await _context.Inspeccion.FindAsync(id);
            if (inspeccion == null)
            {
                return Json(new { success = false, message = "Inspección no encontrada." });
            }

            // Verifica que el nuevo estado sea diferente al actual y que sea un estado permitido
            if (inspeccion.Estado == newState)
            {
                return Json(new { success = false, message = "La inspección ya se encuentra en el estado seleccionado." });
            }

            if (newState != EstadoInspeccion.Aprobada && newState != EstadoInspeccion.Desaprobada)
            {
                return Json(new { success = false, message = "Estado inválido. Solo se permite cambiar a 'Aprobada' o 'Desaprobada'." });
            }

            try
            {
                inspeccion.Estado = newState;
                _context.Update(inspeccion);
                await _context.SaveChangesAsync();
                if (newState == EstadoInspeccion.Aprobada)
                {
                    TempData["SuccessMessage"] = "La inspección ha sido aprobada exitosamente.";
                }
                else if (newState == EstadoInspeccion.Desaprobada)
                {
                    TempData["WarningMessage"] = "La inspección ha sido desaprobada.";
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al cambiar el estado: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetInspectoresByProyecto(Guid proyectoId)
        {
            var residentes = await _context.Proyectos
                .Where(p => p.Id == proyectoId)
                .SelectMany(p => p.Residentes)
                .Select(r => new { r.Id, r.NombreCompleto })
                .ToListAsync();

            return Json(residentes);
        }




    }
}
