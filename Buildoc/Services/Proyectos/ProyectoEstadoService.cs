namespace Buildoc.Services.Proyectos { 
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Buildoc.Data;
using Buildoc.Models;

public class ProyectoEstadoService : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;

	public ProyectoEstadoService(IServiceProvider serviceProvider)
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

				var proyectos = await context.Proyectos
					.Where(p => p.Estado == Proyecto.EstadoProyecto.EnCurso)
					.ToListAsync();

				foreach (var proyecto in proyectos)
				{
					if (proyecto.FechaFinalizacion <= DateTime.Now && proyecto.Estado != Proyecto.EstadoProyecto.Finalizado)
					{
						proyecto.Estado = Proyecto.EstadoProyecto.Finalizado;
					}

					//if (proyecto.FechaFinalizacion.AddDays(30) <= DateTime.Now && proyecto.Estado == Proyecto.EstadoProyecto.Finalizado)
					//{
					//	proyecto.Estado = Proyecto.EstadoProyecto.Archivado;
					//}
				}

				await context.SaveChangesAsync();
			}

			await Task.Delay(TimeSpan.FromHours(12), stoppingToken); // Ejecutar cada 24 horas
		}
	}
}
}