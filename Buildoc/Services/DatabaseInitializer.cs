using Microsoft.AspNetCore.Identity;
using Buildoc.Models;

namespace Buildoc.Services
{
    public class DatabaseInitializer
    {
        public static async Task SeedDataAsync(UserManager<Usuario>? userManager,
            RoleManager<IdentityRole> roleManager)
        {
            if (userManager == null || roleManager == null)
            {
                Console.WriteLine("userManager or roleManager is null => exit");
                return;
            }
            //Revisar si existe el rol de administrador
            var exists = await roleManager.RoleExistsAsync("admin");
            if (!exists)
            {
                Console.WriteLine("Admin role is not defined and will be created");
                await roleManager.CreateAsync(new IdentityRole("Administrador"));
            }

            //Revisar si existe el rol Coordinador
            exists = await roleManager.RoleExistsAsync("Coordinador");
            if (!exists)
            {
                Console.WriteLine("Acudiente role is not defined and will be created");
                await roleManager.CreateAsync(new IdentityRole("Coordinador"));
            }

            //Revisar si existe el rol Trabajador
            exists = await roleManager.RoleExistsAsync("Trabajador");
            if (!exists)
            {
                Console.WriteLine("El rol trabajador ha sido creado");
                await roleManager.CreateAsync(new IdentityRole("Trabajador"));
            }

            var adminUsers = await userManager.GetUsersInRoleAsync("Administrador");
            if (adminUsers.Any())
            {
                //El admin ya esta creado
                Console.WriteLine("El usuario adminsitrador ya existe => exit");
                return;
            }

            //Crear el usuario administrador
            var user = new Usuario()
            {
                UserName = "admin@buildoc.com",
                Email = "admin@buildoc.com",
                Estado=true
            };
            string defaultPassword = "Admin123#";

            var result = await userManager.CreateAsync(user, defaultPassword);
            if (result.Succeeded)
            {
                //modificar la tabla rol
                await userManager.AddToRoleAsync(user, "Administrador");
                Console.WriteLine("Admin user created successfully! Please update the initial password!");
                Console.WriteLine("Email: " + user.Email + " Initial password: " + defaultPassword);
            }
            else
            {
                Console.WriteLine("Unable to create Admin User: " + result.Errors.First().Description);
            }

        }
    }
}

