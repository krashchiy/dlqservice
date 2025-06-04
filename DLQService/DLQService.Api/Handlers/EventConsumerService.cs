using Aapc.Eventing.Abstractions.Consumer;

namespace DLQService.Api.Handlers
{
    public class EventConsumerService(
        IMessageConsumer consumer,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<EventConsumerService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // Create a temporary scope just to register the handler types
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    // Register JSON handlers
                    var jsonOrderHandler = scope.ServiceProvider.GetRequiredService<OrderEventHandler>();

                    consumer.RegisterJsonHandler(jsonOrderHandler);

                    logger.LogInformation("Successfully registered event handlers");
                }

                logger.LogInformation("Starting message consumer");
                await consumer.StartAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // This is expected when stopping
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to start event consumer");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await consumer.StopAsync(cancellationToken);
                logger.LogInformation("Stopped message consumer");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error stopping message consumer");
                throw;
            }
        }
    }

}
