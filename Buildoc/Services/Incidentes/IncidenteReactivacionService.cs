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
    public class IncidenteReactivacionService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public IncidenteReactivacionService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // Obtener todos los incidentes con CierrePorServidor = true
                    var incidentesConCierre = await context.Incidentes
                        .Where(i => i.CierrePorServidor == true && i.Estado == EstadoIncidenteEnum.Cerrado)
                        .Include(i => i.Proyecto)
                        .ToListAsync();

                    // Filtrar solo los incidentes asociados a proyectos con estado EnCurso
                    var incidentesReactivar = incidentesConCierre
                        .Where(i => i.Proyecto != null && i.Proyecto.Estado == Proyecto.EstadoProyecto.EnCurso)
                        .ToList();

                    foreach (var incidente in incidentesReactivar)
                    {
                        // Reactivar el incidente cambiando el estado a Activo y CierrePorServidor a false
                        incidente.Estado = EstadoIncidenteEnum.Activo;
                        incidente.CierrePorServidor = false;
                    }

                    // Guardar cambios en la base de datos
                    await context.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromHours(12), stoppingToken); // Ejecutar cada 12 horas
            }
        }
    }
}
