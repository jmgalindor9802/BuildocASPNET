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
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
namespace Buildoc.Controllers
{
    public class UsuariosController : Controller
    {


        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _host;
        private readonly IEmailSender _emailSender;
        private readonly IAuthorizationService _authorizationService;
        public UsuariosController(ApplicationDbContext context, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment host, IEmailSender emailSender, IAuthorizationService authorizationService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _host = host;
            _emailSender = emailSender;
            _authorizationService = authorizationService;
        }

        public string ReturnUrl { get; set; }
        private string myEmail = "buildoc2@gmail.com";
        private string myPassword = "enux ynxq flpn xoza";
        private string myAlias = "BuilDoc";


        // GET: Usuarios
        [Authorize(Roles = "Administrador")]
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

                if (!roles.Contains("Administrador"))
                {
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
        [Authorize(Roles = "Administrador")]
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
                Departamento = usuario.Departamento,
                Telefono = usuario.Telefono,
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

        public class PasswordGenerator
        {
            public static string GenerateRandomPassword(int length = 10)
            {
                if (length < 6) length = 6; // Ensure minimum length for complexity requirements

                const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                const string lowercase = "abcdefghijklmnopqrstuvwxyz";
                const string digits = "0123456789";
                const string specialChars = "!@#$%^&*()_+";
                const string allChars = uppercase + lowercase + digits + specialChars;

                var random = new Random();

                // Ensure the password contains at least one of each required character type
                var password = new char[length];
                password[0] = uppercase[random.Next(uppercase.Length)];
                password[1] = lowercase[random.Next(lowercase.Length)];
                password[2] = digits[random.Next(digits.Length)];
                password[3] = specialChars[random.Next(specialChars.Length)];

                // Fill the rest of the password with random characters
                for (int i = 4; i < length; i++)
                {
                    password[i] = allChars[random.Next(allChars.Length)];
                }

                // Shuffle the array to avoid a predictable pattern
                return new string(password.OrderBy(x => random.Next()).ToArray());
            }
        }

        // GET: Usuarios/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            var model = new UsuarioViewModel();
            return PartialView(model);
        }
        // POST: Usuarios/Create
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            // Verificar si ya existe un usuario con el mismo Email
            var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null)
            {
                return Json(new { success = false, message = "Ya existe un usuario con este correo electrónico." });
            }

            // Verificar si ya existe un usuario con el mismo Cedula
            var existingUserByCedula = await _context.Users.FirstOrDefaultAsync(u => u.Cedula == model.Cedula);
            if (existingUserByCedula != null)
            {
                return Json(new { success = false, message = "Ya existe un usuario con este número de cédula." });
            }
            if (ModelState.IsValid)
            {
                // Generar contraseña aleatoria
                var randomPassword = PasswordGenerator.GenerateRandomPassword();
                model.Password = randomPassword;
                model.ConfirmPassword = randomPassword;


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
                    // Generar el token de confirmación de correo electrónico
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Usuarios",
                        new { userId = user.Id, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);
                    // Enviar el correo electrónico de confirmación
                    await SendEmailAsync(model.Email, "Confirmación de correo electrónico",
                        $"<p>Hola {user.Nombres},</p>" +
                        "<p>Gracias por unirte a BuilDoc para el manejo de usuarios, proyectos, inspecciones e incidentes.</p>" +
                        "<p>Tu contraseña temporal es: <strong>" + randomPassword + "</strong></p>" +
                        "<p>Por favor, confirma tu cuenta haciendo clic en el botón a continuación:</p>" +
                        $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}' style='display:inline-block;padding:10px 20px;color:#fff;background-color:#007bff;text-decoration:none;border-radius:5px;'>Confirmar cuenta</a>" +
                        "<p>Después de verificar tu cuenta, se te pedirá que hagas un restablecimiento de contraseña. Por favor, sigue las instrucciones y disfruta.</p>");
                    TempData["SuccessMessage"] = "¡El usuario se ha editado exitosamente!";
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

        public async Task<bool> SendEmailAsync(string email, string subject, string confirmLink)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();
                message.From = new MailAddress(myEmail, myAlias, System.Text.Encoding.UTF8);
                message.To.Add(email);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = confirmLink;

                smtpClient.Port = 587; // Cambiar el puerto a 587
                smtpClient.Host = "smtp.gmail.com"; // Cambiar el host a smtp.gmail.com
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(myEmail, myPassword);
                smtpClient.EnableSsl = true; // Asegurarse de que SSL esté habilitado
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                // Eliminar la validación del certificado del servidor, ya que puede causar problemas de seguridad
                // ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; };

                await smtpClient.SendMailAsync(message);
                return true; // Aseguramos que se devuelve true cuando el correo se envía correctamente
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error para un análisis posterior
                return false;
            }
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"No se pudo cargar el usuario con ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            var model = new ConfirmEmailViewModel
            {
                StatusMessage = result.Succeeded ? "Gracias por confirmar tu correo electrónico." : "Error al confirmar tu correo electrónico.",
                UserEmail = user.Email
            };

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword(string email)
        {
            var model = new ForgotPasswordViewModel { Email = email };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // No revelar que el usuario no existe o no está confirmado
                    return RedirectToPage("/Account/ForgotPasswordConfirmation", new { area = "Identity" });
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                await SendEmailAsync(
                    model.Email,
                    "Restablecer Contraseña",
                    $"Por favor, restablece tu contraseña haciendo <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clic aquí</a>.");

                return RedirectToPage("/Account/ForgotPasswordConfirmation", new { area = "Identity" });
            }

            return View(model);
        }


        // GET: Usuarios/Edit/5
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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
                Telefono = usuario.Telefono,
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
        [Authorize(Roles = "Administrador")]
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

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> UsuarioDesactivado()
        {
            var todosUsuarios = await _userManager.Users.ToListAsync(); // Obtén todos los usuarios

            var usuariosTotales = todosUsuarios.Count;
            var usuariosActivos = todosUsuarios.Count(u => u.Estado);
            var usuariosDesactivos = todosUsuarios.Count(u => !u.Estado);

            ViewBag.UsuariosTotales = usuariosTotales;
            ViewBag.UsuariosActivos = usuariosActivos;
            ViewBag.UsuariosDesactivos = usuariosDesactivos;

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
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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