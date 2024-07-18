using Buildoc.Data;
using Buildoc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
namespace Buildoc.Controllers
{
    public class UsuariosController : Controller
    {


        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _host;
        public UsuariosController(ApplicationDbContext context, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment host)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _host = host;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index(string roleFilter)
        {
            var todosUsuarios = await _userManager.Users.ToListAsync(); // Obtén todos los usuarios

            var usuariosTotales = todosUsuarios.Count;
            var usuariosActivos = todosUsuarios.Count(u => u.Estado);
            var usuariosDesactivos = todosUsuarios.Count(u => !u.Estado);

            ViewBag.UsuariosTotales = usuariosTotales;
            ViewBag.UsuariosActivos = usuariosActivos;
            ViewBag.UsuariosDesactivos = usuariosDesactivos;

            if (!string.IsNullOrEmpty(roleFilter))
            {
                return await FiltrarRol(roleFilter);
            }

            var usuarios = await _userManager.Users
                                            .Where(u => u.Estado) // Filtra usuarios con Estado true
                                            .ToListAsync();

            var usuariosConRoles = new List<IndexUsuarioViewModel>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                usuariosConRoles.Add(new IndexUsuarioViewModel
                {
                    Id = usuario.Id,
                    Email = usuario.Email,
                    Nombres = usuario.Nombres,
                    Direccion = usuario.Direccion,
                    Estado = usuario.Estado,
                    Cedula = usuario.Cedula,
                    Role = roles.FirstOrDefault() // Suponemos que el usuario tiene un solo rol
                });
            }

            return View(usuariosConRoles);
        }

        // Método para filtrar por roles los usuarios
        public async Task<IActionResult> FiltrarRol(string roleFilter)
        {
            // Obtener el rol especificado
            var role = await _roleManager.FindByNameAsync(roleFilter);
            if (role == null)
            {
                // Manejar el caso en que el rol no exista
                return NotFound();
            }

            // Obtener los usuarios asociados a este rol
            var roleUsers = await _userManager.GetUsersInRoleAsync(role.Name);

            // Crear la lista de usuarios con el rol filtrado
            var usuariosConRol = (from u in roleUsers
                                  select new IndexUsuarioViewModel
                                  {
                                      Id = u.Id,
                                      Email = u.Email,
                                      Nombres = u.Nombres,
                                      Direccion = u.Direccion,
                                      Estado = u.Estado,
                                      Cedula = u.Cedula,
                                      Role = roleFilter // Asignar el rol filtrado
                                  }).ToList();

            return View("Index", usuariosConRol); // Devolver la vista con los usuarios filtrados
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(usuario);
            var viewModel = new UsuarioDetailsViewModel
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Cedula = usuario.Cedula,
                FechaNacimiento = usuario.FechaNacimiento,
                Direccion = usuario.Direccion,
                Municipio = usuario.Municipio,
                Eps = usuario.Eps,
                Arl = usuario.Arl,
                Profesion = usuario.Profesion,
                Role = roles.FirstOrDefault() // Suponiendo que el usuario tiene un solo rol
            };
            return PartialView("Details", viewModel);
        }


        // GET: Usuarios/Create
        public IActionResult Create()
        {
            var model = new UsuarioViewModel();
            return PartialView(model);
        }
        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Usuario
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nombres = model.Nombres,
                    Apellidos = model.Apellidos,
                    Direccion = model.Direccion,
                    Cedula = model.Cedula,
                    Estado = true,
                    Arl = model.Arl,
                    Eps = model.Eps,
                    FechaNacimiento = model.FechaNacimiento,
                    Municipio = model.Municipio,
                    Departamento = model.Departamento,
                    Profesion = model.Profesion,
                    Telefono = model.Telefono,
                    
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //Asinar el rol
                    await _userManager.AddToRoleAsync(user, model.Role);
                    // Aquí puedes redirigir a la acción deseada después de crear el usuario
                    TempData["SuccessMessage"] = "¡El usuario se ha creado exitosamente!";
                    return Json(new { success = true });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Si llegamos aquí, significa que hubo un error en el modelo, devolvemos la vista con errores
            
            return PartialView("Create", model);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var usuario = await _context.Users.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Obtener los roles actuales del usuario
            var roles = await _userManager.GetRolesAsync(usuario);

            // Obtener todos los roles disponibles
            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            var viewModel = new EditUsuarioViewModel
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Cedula = usuario.Cedula,
                Telefono = usuario.Telefono,
                FechaNacimiento = usuario.FechaNacimiento,
                Direccion = usuario.Direccion,
                Municipio = usuario.Municipio,
                Departamento = usuario.Departamento,
                Eps = usuario.Eps,
                Arl = usuario.Arl,
                Profesion = usuario.Profesion,
                Role = roles.FirstOrDefault(), // Suponiendo que el usuario tiene un solo rol

            };
            return PartialView("Edit", viewModel);
        }


        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUsuarioViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioToUpdate = await _userManager.FindByIdAsync(id);
                    if (usuarioToUpdate == null)
                    {
                        return NotFound();
                    }
                    usuarioToUpdate.Email = viewModel.Email;
                    usuarioToUpdate.Nombres = viewModel.Nombres;
                    usuarioToUpdate.Apellidos = viewModel.Apellidos;
                    usuarioToUpdate.Cedula = viewModel.Cedula;
                    usuarioToUpdate.Telefono = viewModel.Telefono;
                    usuarioToUpdate.FechaNacimiento = viewModel.FechaNacimiento;
                    usuarioToUpdate.Direccion = viewModel.Direccion;
                    usuarioToUpdate.Municipio = viewModel.Municipio;
                    usuarioToUpdate.Departamento = viewModel.Departamento;
                    usuarioToUpdate.Eps = viewModel.Eps;
                    usuarioToUpdate.Arl = viewModel.Arl;
                    usuarioToUpdate.Profesion = viewModel.Profesion;

                    var result = await _userManager.UpdateAsync(usuarioToUpdate);

                    if (result.Succeeded)
                    {
                        // Actualizar el rol del usuario
                        var currentRoles = await _userManager.GetRolesAsync(usuarioToUpdate);
                        await _userManager.RemoveFromRolesAsync(usuarioToUpdate, currentRoles);
                        await _userManager.AddToRoleAsync(usuarioToUpdate, viewModel.Role);

                        TempData["SuccessMessage"] = "¡El usuario se ha editado exitosamente!";
                        return Json(new { success = true });
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(viewModel.Id))
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
                    // Registrar la excepción
                    ModelState.AddModelError(string.Empty, $"Unexpected error: {ex.Message}");
                }
            }
            else
            {
                // Registrar los errores del ModelState
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }
            }

            // Si llegamos aquí, significa que ModelState no es válido, devolvemos la vista con errores
            return PartialView("Edit", viewModel);
        }



        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(usuario);
            var viewModel = new UsuarioDetailsViewModel
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Cedula = usuario.Cedula,
                FechaNacimiento = usuario.FechaNacimiento,
                Direccion = usuario.Direccion,
                Municipio = usuario.Municipio,
                Departamento = usuario.Departamento,
                Eps = usuario.Eps,
                Arl = usuario.Arl,
                Profesion = usuario.Profesion,
                Role = roles.FirstOrDefault() // Suponemos que el usuario tiene un solo rol
            };
            return PartialView("Delete", viewModel);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario != null)
            {
                usuario.Estado = false; // Cambiar el estado a "Deshabilitado"
                var result = await _userManager.UpdateAsync(usuario);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "¡El usuario se ha desactivado exitosamente!";
                    return Json(new { success = true });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return PartialView(usuario); // Devolver la vista con el usuario si hay errores
                }
            }

            return NotFound(); // Usuario no encontrado
        }

        public async Task<IActionResult> UsuarioDesactivado()
        {
            var usuarios = await _userManager.Users
                                            .Where(u => !u.Estado) // Filtra usuarios con Estado true
                                            .ToListAsync();

            var usuariosConRoles = new List<IndexUsuarioViewModel>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                usuariosConRoles.Add(new IndexUsuarioViewModel
                {
                    Id = usuario.Id,
                    Email = usuario.Email,
                    Nombres = usuario.Nombres,
                    Direccion = usuario.Direccion,
                    Estado = usuario.Estado,
                    Cedula = usuario.Cedula,
                    Role = roles.FirstOrDefault() // Suponemos que el usuario tiene un solo rol
                });
            }

            return View(usuariosConRoles);
        }
        //Cargar los  datos para la vista, Reactivar los usuarios desactivados
        public async Task<IActionResult> ReactivarUsuario(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(usuario);
            var viewModel = new UsuarioDetailsViewModel
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Cedula = usuario.Cedula,
                FechaNacimiento = usuario.FechaNacimiento,
                Direccion = usuario.Direccion,
                Municipio = usuario.Municipio,
                Departamento = usuario.Departamento,
                Eps = usuario.Eps,
                Arl = usuario.Arl,
                Profesion = usuario.Profesion,
                Role = roles.FirstOrDefault() // Suponemos que el usuario tiene un solo rol
            };
            return PartialView("ReactivarUsuario", viewModel);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("ReactivarUsuario")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReactivarUsuarioConfirmed(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);

            if (usuario != null)
            {
                usuario.Estado = true; // Cambiar el estado a "Deshabilitado"
                var result = await _userManager.UpdateAsync(usuario);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(UsuarioDesactivado));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return PartialView(usuario); // Devolver la vista con el usuario si hay errores
                }
            }

            return NotFound(); // Usuario no encontrado
        }

        public async Task<IActionResult> DescargarPDF()
        {
            var usuarios = await _userManager.Users
                                            .Where(u => u.Estado) // Filtra usuarios con Estado true
                                            .ToListAsync();

            var usuariosConRoles = new List<IndexUsuarioViewModel>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                usuariosConRoles.Add(new IndexUsuarioViewModel
                {
                    Id = usuario.Id,
                    Email = usuario.Email,
                    Nombres = usuario.Nombres,
                    Direccion = usuario.Direccion,
                    Estado = usuario.Estado,
                    Cedula = usuario.Cedula,
                    Role = roles.FirstOrDefault() // Suponemos que el usuario tiene un solo rol
                });
            }

            var data = Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(30);
                    page.Header().ShowOnce().Row(row =>
                    {
                        var rutaImagen = Path.Combine(_host.WebRootPath, "logo_buildoc_color.png");
                        byte[] imageData = System.IO.File.ReadAllBytes(rutaImagen);
                        row.ConstantItem(150).Image(imageData);

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().AlignCenter().Text("Codigo Estudiante SAC").Bold().FontSize(14);
                            col.Item().AlignCenter().Text("Jr. Las mercedes N378 - Lima").FontSize(9);
                            col.Item().AlignCenter().Text("987 987 123 / 02 213232").FontSize(9);
                            col.Item().AlignCenter().Text("codigo@example.com").FontSize(9);
                        });

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Border(1).BorderColor("#257272")
                            .AlignCenter().Text("RUC 21312312312");

                            col.Item().Background("#257272").Border(1)
                            .BorderColor("#257272").AlignCenter()
                            .Text("Boleta de venta").FontColor("#fff");

                            col.Item().Border(1).BorderColor("#257272").
                            AlignCenter().Text("B0001 - 234");
                        });
                    });

                    page.Content().PaddingVertical(10).Column(col1 =>
                    {
                        col1.Item().Column(col2 =>
                        {
                            col2.Item().Text("Datos del cliente").Underline().Bold();
                            col2.Item().Text(txt =>
                            {
                                txt.Span("Nombre: ").SemiBold().FontSize(10);
                                txt.Span("Mario mendoza").FontSize(10);
                            });
                            col2.Item().Text(txt =>
                            {
                                txt.Span("DNI: ").SemiBold().FontSize(10);
                                txt.Span("978978979").FontSize(10);
                            });
                            col2.Item().Text(txt =>
                            {
                                txt.Span("Direccion: ").SemiBold().FontSize(10);
                                txt.Span("av. miraflores 123").FontSize(10);
                            });
                        });

                        col1.Item().LineHorizontal(0.5f);

                        col1.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            tabla.Header(header =>
                            {
                                header.Cell().Background("#257272")
                                .Padding(2).Text("Nombre").FontColor("#fff");

                                header.Cell().Background("#257272")
                               .Padding(2).Text("Email").FontColor("#fff");

                                header.Cell().Background("#257272")
                               .Padding(2).Text("Cedula").FontColor("#fff");

                                header.Cell().Background("#257272")
                               .Padding(2).Text("Rol").FontColor("#fff");
                            });

                            foreach (var usuario in usuariosConRoles)
                            {
                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9")
                                .Padding(2).Text(usuario.Nombres).FontSize(10);

                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9")
                                .Padding(2).Text(usuario.Email).FontSize(10);

                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9")
                                .Padding(2).Text(usuario.Cedula.ToString()).FontSize(10);

                                tabla.Cell().BorderBottom(0.5f).BorderColor("#D9D9D9")
                                .Padding(2).Text(usuario.Role).FontSize(10);
                            }
                        });

                        col1.Item().AlignRight().Text("Total: " + usuariosConRoles.Count).FontSize(12);
                        if (usuariosConRoles.Count == 0)
                        {
                            col1.Item().Background(Colors.Grey.Lighten3).Padding(10)
                            .Column(column =>
                            {
                                column.Item().Text("No hay usuarios activos").FontSize(14);
                                column.Spacing(5);
                            });
                        }
                        col1.Spacing(10);
                    });

                    page.Footer()
                    .AlignRight()
                    .Text(txt =>
                    {
                        txt.Span("Pagina ").FontSize(10);
                        txt.CurrentPageNumber().FontSize(10);
                        txt.Span(" de ").FontSize(10);
                        txt.TotalPages().FontSize(10);
                    });
                });
            }).GeneratePdf();

            Stream stream = new MemoryStream(data);
            return File(stream, "application/pdf", "usuarios.pdf");
        }
        private bool UsuarioExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}