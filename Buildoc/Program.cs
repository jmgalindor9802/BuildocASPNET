using Buildoc.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Buildoc.Models;
using Buildoc.Services;
using System.Globalization;
using Buildoc.Services.Proyectos;
using Buildoc.Services.Incidentes;
using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);
var cultureInfo = new CultureInfo("es-CO");

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
builder.Services.AddControllersWithViews();
builder.Services.AddScoped(_ =>
{
    return new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobStorage"));
});
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
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
builder.Services.AddScoped<IncidenteEstadoService>();
builder.Services.AddScoped<ProyectoEstadoService>();



// Add EmailSender service
builder.Services.AddScoped<IEmailSender, EmailSender>();


//Contenedor
builder.Services.AddScoped<IAzureStorageService, AzureBlobStorageService>();
builder.Services.AddScoped<IFileService, FileService>();

//Cors
builder.Services.AddCors();

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

app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
});


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();



app.Run();
