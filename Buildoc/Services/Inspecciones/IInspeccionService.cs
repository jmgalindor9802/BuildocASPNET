using Buildoc.Data;
using Buildoc.Models;
using Microsoft.EntityFrameworkCore;

namespace Buildoc.Services.Inspecciones
{
    public interface IInspeccionService
    {
        Task UpdateInspeccionesNoRespondidasAsync();
    }

    public class InspeccionService : IInspeccionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InspeccionService> _logger;

        public InspeccionService(ApplicationDbContext context, ILogger<InspeccionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task UpdateInspeccionesNoRespondidasAsync()
        {
            var currentDate = DateTime.Now;

            // Obtener todas las inspecciones programadas sin respuesta
            var inspeccionesNoRespondidas = await _context.Inspeccion
                .Where(i => i.Estado == EstadoInspeccion.Programada
                            && i.RespuestaId == null) // Sin respuesta
                .ToListAsync();

            if (inspeccionesNoRespondidas.Any())
            {
                foreach (var inspeccion in inspeccionesNoRespondidas)
                {
                    // Calcular la fecha y hora en que la inspección termina
                    DateTime fechaFinInspeccion;

                    if (inspeccion.EsTodoElDia)
                    {
                        // Si es inspección de todo el día, la fecha de fin es a medianoche del día siguiente
                        fechaFinInspeccion = inspeccion.FechaInspeccion.Date.AddDays(1);
                    }
                    else if (inspeccion.DuracionHoras.HasValue)
                    {
                        // Si hay una duración específica, sumar la duración a la fecha de inicio
                        fechaFinInspeccion = inspeccion.FechaInspeccion.AddHours(inspeccion.DuracionHoras.Value);
                    }
                    else
                    {
                        // Si no tiene duración, termina cuando comienza
                        fechaFinInspeccion = inspeccion.FechaInspeccion;
                    }

                    // Calcular la fecha límite para cambiar el estado a 'Sin Responder' (48 horas después de la fecha de fin)
                    DateTime fechaLimiteResponder = fechaFinInspeccion.AddHours(48);

                    // Si ya pasaron 48 horas desde la fecha límite, cambiar el estado a 'Sin Responder'
                    if (currentDate > fechaLimiteResponder)
                    {
                        inspeccion.Estado = EstadoInspeccion.SinResponder;
                        _logger.LogInformation($"La inspección con ID {inspeccion.Id} ha sido marcada como 'Sin Responder'.");
                    }
                }

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
            }
        }
    }
}
