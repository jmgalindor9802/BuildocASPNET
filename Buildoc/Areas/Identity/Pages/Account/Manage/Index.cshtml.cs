using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Buildoc.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Buildoc.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        public IndexModel(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Campo requerido")]
            [MaxLength(255, ErrorMessage = "La longitud maxima es de 255 caracteres")]
            [Display(Name = "Nombres")]
            public string Nombres { get; set; }

            [Required(ErrorMessage = "Campo requerido")]
            [MaxLength(255, ErrorMessage = "La longitud maxima es de 255 caracteres")]
            [Display(Name = "Apellidos")]
            public string Apellidos { get; set; }

            [Display(Name = "Correo Electrónico")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Campo requerido")]
            [RegularExpression(@"^\d{10}$", ErrorMessage = "El número de teléfono debe tener 10 dígitos.")]
            [Display(Name = "Teléfono")]
            public long Telefono { get; set; }

            [Required(ErrorMessage = "Campo requerido")]
            [Display(Name = "Cédula")]
            public long Cedula { get; set; }

            [Display(Name = "Departamento")]
            public string Departamento { get; set; }

            [Display(Name = "Municipio")]
            public string Municipio { get; set; }

            [Required(ErrorMessage = "Campo requerido")]
            [Display(Name = "Dirección")]
            public string Direccion { get; set; }

            [Display(Name = "EPS")]
            public string Eps { get; set; }

            [Display(Name = "ARL")]
            public string Arl { get; set; }

            [Display(Name = "Profesión")]
            public string Profesion { get; set; }

            [Required(ErrorMessage = "Campo requerido")]
            [Display(Name = "Fecha de Nacimiento")]
            public DateOnly FechaNacimiento { get; set; }
        }

        private async Task LoadAsync(Usuario user)
        {
            var userName = await _userManager.GetUserNameAsync(user);

            Username = userName;

            Input = new InputModel
            {
                Nombres = user.Nombres,
                Apellidos = user.Apellidos,
                Email = user.Email,
                Telefono = user.Telefono,
                Cedula = user.Cedula,
                Departamento = user.Departamento,
                Municipio = user.Municipio,
                Direccion = user.Direccion,
                Eps = user.Eps,
                Arl = user.Arl,
                Profesion = user.Profesion,
                FechaNacimiento = user.FechaNacimiento ?? DateOnly.FromDateTime(DateTime.Now),
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!string.IsNullOrWhiteSpace(Input.Nombres))
            {
                // Validar que no contenga números
                if (System.Text.RegularExpressions.Regex.IsMatch(Input.Nombres, @"\d"))
                {
                    ModelState.AddModelError("Input.Nombres", "El nombre no debe contener números.");
                }

                // Validar que no tenga espacios al inicio o al final
                if (Input.Nombres.StartsWith(" ") || Input.Nombres.EndsWith(" "))
                {
                    ModelState.AddModelError("Input.Nombres", "El nombre no puede tener espacios al inicio o al final.");
                }
            }
            else if (!string.IsNullOrWhiteSpace(Input.Apellidos))
            {
                // Validar que no contenga números
                if (System.Text.RegularExpressions.Regex.IsMatch(Input.Apellidos, @"\d"))
                {
                    ModelState.AddModelError("Input.Apellidos", "El apellido no debe contener números.");
                }

                // Validar que no tenga espacios al inicio o al final
                if (Input.Apellidos.StartsWith(" ") || Input.Apellidos.EndsWith(" "))
                {
                    ModelState.AddModelError("Input.Apellidos", "El apellido no puede tener espacios al inicio o al final.");
                }
            }

            // Validar que la fecha de nacimiento indique al menos 18 años, con precisión en años, meses y días
            DateTime today = DateTime.Today;
            DateTime birthDate = Input.FechaNacimiento.ToDateTime(TimeOnly.MinValue);

            // Calcular la edad en años, meses y días
            int ageYears = today.Year - birthDate.Year;
            int ageMonths = today.Month - birthDate.Month;
            int ageDays = today.Day - birthDate.Day;

            // Ajuste si el mes o día actual es menor que el mes o día de nacimiento
            if (ageMonths < 0 || (ageMonths == 0 && ageDays < 0))
            {
                ageYears--; // Si no ha cumplido años este año, reducir la edad en 1
            }

            if (ageYears < 18)
            {
                ModelState.AddModelError("Input.FechaNacimiento", "Debes tener al menos 18 años.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // Actualiza los campos del usuario
            user.Nombres = Input.Nombres;
            user.Apellidos = Input.Apellidos;
            user.Telefono = Input.Telefono;
            user.Cedula = Input.Cedula;
            user.Departamento = Input.Departamento;
            user.Municipio = Input.Municipio;
            user.Direccion = Input.Direccion;
            user.Eps = Input.Eps;
            user.Arl = Input.Arl;
            user.Profesion = Input.Profesion;
            user.FechaNacimiento = Input.FechaNacimiento;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                StatusMessage = "Error al actualizar el perfil";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Perfil actualizado correctamente";
            return RedirectToPage();
        }
    }
}
