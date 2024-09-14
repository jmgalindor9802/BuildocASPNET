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
                using (var scope = _serviceProvider.CreateScope())
                {
                    var inspeccionService = scope.ServiceProvider.GetRequiredService<IInspeccionService>();

                    try
                    {
                        await inspeccionService.UpdateInspeccionesNoRespondidasAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ocurrió un error al actualizar los estados de las inspecciones.");
                    }
                }

                // Esperar 24 horas antes de volver a revisar
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }

}
