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
namespace Buildoc.Controllers
{
    public class UsuariosController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsuariosController(ApplicationDbContext context, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
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

            return View(usuariosConRoles);
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

            return View(viewModel);
        }


        // GET: Usuarios/Create
        public IActionResult Create()
        {
            var model = new UsuarioViewModel();
            return View(model);
        }
        //POST modal
        [HttpPost]
        public JsonResult Insert(UsuarioViewModel model)
        {
            if (ModelState.IsValid)
            {
                return null;
            }
            else
            {
                return Json("Fallo registro del modal");
            }
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
                    Profesion = model.Profesion,
                    Telefono = model.Telefono,
                    
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //Asinar el rol
                    await _userManager.AddToRoleAsync(user, model.Role);
                    // Aquí puedes redirigir a la acción deseada después de crear el usuario
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Si llegamos aquí, significa que hubo un error en el modelo, devolvemos la vista con errores
            return View(model);
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
                Eps = usuario.Eps,
                Arl = usuario.Arl,
                Profesion = usuario.Profesion,
                Role = roles.FirstOrDefault(), // Suponiendo que el usuario tiene un solo rol

            };

            return View(viewModel);
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

                        return RedirectToAction(nameof(Index));
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
            return View(viewModel);
        }



        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
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
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(usuario); // Devolver la vista con el usuario si hay errores
                }
            }

            return NotFound(); // Usuario no encontrado
        }

        private bool UsuarioExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}