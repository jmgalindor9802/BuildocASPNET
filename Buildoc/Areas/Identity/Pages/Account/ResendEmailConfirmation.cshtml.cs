using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using Buildoc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Mail;
using System.Net;

namespace Buildoc.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {

        private readonly UserManager<Usuario> _userManager;
        private readonly IEmailSender _emailSender;

        public ResendEmailConfirmationModel(UserManager<Usuario> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }
        private string miEmail = "buildoc2@gmail.com";
        private string miContrasena = "enux ynxq flpn xoza";
        private string miAlias = "BuilDoc";
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var usuario = await _userManager.FindByEmailAsync(Input.Email);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Correo de verificación enviado. Por favor, revisa tu correo electrónico.");
                return Page();
            }

            var usuarioId = await _userManager.GetUserIdAsync(usuario);
            var codigo = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);
            codigo = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(codigo));
            var urlDeRetorno = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = usuarioId, code = codigo },
                protocol: Request.Scheme);
            await EnviarCorreoAsync(
                Input.Email,
                "Confirma tu correo electrónico",
                $"Por favor, confirma tu cuenta <a href='{HtmlEncoder.Default.Encode(urlDeRetorno)}'  style='display:inline-block;padding:10px 20px;color:#fff;background-color:#007bff;text-decoration:none;border-radius:5px;'>haciendo clic aquí</a>.");

            ModelState.AddModelError(string.Empty, "Correo de verificación enviado. Por favor, revisa tu correo electrónico.");
            return Page();
        }

        public async Task<bool> EnviarCorreoAsync(string email, string asunto, string enlaceDeConfirmacion)
        {
            try
            {
                MailMessage mensaje = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();
                mensaje.From = new MailAddress(miEmail, miAlias, System.Text.Encoding.UTF8);
                mensaje.To.Add(email);
                mensaje.Subject = asunto;
                mensaje.IsBodyHtml = true;
                mensaje.Body = enlaceDeConfirmacion;

                smtpClient.Port = 587; // Cambiar el puerto a 587
                smtpClient.Host = "smtp.gmail.com"; // Cambiar el host a smtp.gmail.com
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(miEmail, miContrasena);
                smtpClient.EnableSsl = true; // Asegurarse de que SSL esté habilitado
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                // Eliminar la validación del certificado del servidor, ya que puede causar problemas de seguridad
                // ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return true; };

                await smtpClient.SendMailAsync(mensaje);
                return true; // Aseguramos que se devuelve true cuando el correo se envía correctamente
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error para un análisis posterior
                return false;
            }
        }
    }
}
