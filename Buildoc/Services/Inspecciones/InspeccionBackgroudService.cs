namespace Buildoc.Services.Inspecciones
{
    public class InspeccionBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<InspeccionBackgroundService> _logger;

        public InspeccionBackgroundService(IServiceProvider serviceProvider, ILogger<InspeccionBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("El servicio en segundo plano de Inspección está en ejecución.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Iniciando la actualización de inspecciones no respondidas.");

                using (var scope = _serviceProvider.CreateScope())
                {
                    var inspeccionService = scope.ServiceProvider.GetRequiredService<IInspeccionService>();

                    try
                    {
                        await inspeccionService.UpdateInspeccionesNoRespondidasAsync();
                        _logger.LogInformation("La actualización de inspecciones no respondidas se completó correctamente.");

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ocurrió un error al actualizar los estados de las inspecciones.");
                    }
                }
                _logger.LogInformation("Esperando un minuto antes de revisar nuevamente.");
                // Esperar 1 minuto antes de volver a revisar
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

            }
            _logger.LogInformation("El servicio en segundo plano de Inspección se ha detenido.");

        }
    }

}
