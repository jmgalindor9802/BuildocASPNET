using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Buildoc.Data;
using Buildoc.Models;

namespace Buildoc.Services.Incidentes
{
    public class IncidenteEstadoService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEmailSender _emailSender;

        public IncidenteEstadoService(IServiceProvider serviceProvider, IEmailSender emailSender)
        {
            _serviceProvider = serviceProvider;
            _emailSender = emailSender;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // Obtener todos los proyectos finalizados que no estén archivados
                    var proyectosFinalizados = await context.Proyectos
                        .Where(p => p.Estado == Proyecto.EstadoProyecto.Finalizado)
                        .Include(p => p.Incidentes)
                        .Include(p => p.Coordinador) // Incluye el Coordinador del proyecto
                        .ToListAsync();

                    foreach (var proyecto in proyectosFinalizados)
                    {
                        var incidentesVencidos = proyecto.Incidentes
                            .Where(i => i.Estado == EstadoIncidenteEnum.Activo)
                            .ToList();

                        foreach (var incidente in incidentesVencidos)
                        {
                            // Cambiar el estado del incidente a "Vencido"
                            incidente.Estado = EstadoIncidenteEnum.Vencido;
                        }

                        // Si hay incidentes que han sido vencidos, enviar un correo al coordinador del proyecto
                        if (incidentesVencidos.Any() && proyecto.Coordinador != null)
                        {
                            string subject = "Incidentes Vencidos en el Proyecto " + proyecto.Nombre;
                            string htmlMessage = "<p>Estimado " + proyecto.Coordinador.Nombres + ",</p>";
                            htmlMessage += "<p>Los siguientes incidentes en el proyecto <strong>" + proyecto.Nombre + "</strong> han cambiado su estado a <strong>Vencido</strong>:</p>";
                            htmlMessage += "<ul>";

                            foreach (var incidente in incidentesVencidos)
                            {
                                htmlMessage += "<li>" + incidente.Titulo + "</li>";
                            }

                            htmlMessage += "</ul>";
                            htmlMessage += "<p>Por favor revise los incidentes para darles una solucion adecuada</p>";
                            htmlMessage += "<p>Saludos,</p><p>Buildoc Support</p>";

                            await _emailSender.SendEmailAsync(proyecto.Coordinador.Email, subject, htmlMessage);
                        }
                    }

                    // Guardar cambios en la base de datos
                    await context.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromHours(12), stoppingToken); // Ejecutar cada 24 horas
            }
        }
    }
}