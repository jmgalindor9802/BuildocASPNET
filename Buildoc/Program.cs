using Buildoc.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Buildoc.Models;
using Buildoc.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<Usuario>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

// A�adir SignInManager y UserManager
builder.Services.AddScoped<SignInManager<Usuario>>();
builder.Services.AddScoped<UserManager<Usuario>>();


var app = builder.Build();

// Inicializaci?n de datos
using (var scope = app.Services.CreateScope())
{
var services = scope.ServiceProvider;
try
{
var userManager = services.GetRequiredService<UserManager<Usuario>>();
var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

await DatabaseInitializer.SeedDataAsync(userManager, roleManager);
}
catch (Exception ex)
{
// Manejar cualquier error de inicializaci?n aqu?
var logger = services.GetRequiredService<ILogger<Program>>();
logger.LogError(ex, "An error occurred during database initialization.");
}
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())


{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();



app.Run();
