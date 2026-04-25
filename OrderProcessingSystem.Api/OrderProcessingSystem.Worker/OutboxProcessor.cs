using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Infrastructure.Persistence;

namespace OrderProcessingSystem.Worker
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<OutboxProcessor> _logger;

        public OutboxProcessor(IServiceProvider services, ILogger<OutboxProcessor> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox Processor running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var messages = await db.OutboxMessages
                        .Where(m => m.ProcessedOn == null)
                        .OrderBy(m => m.OccurredOn)
                        .Take(20)
                        .ToListAsync(stoppingToken);

                    foreach (var msg in messages)
                    {
                        _logger.LogInformation("Processing outbox: {Type} {Id}", msg.Type, msg.Id);

                        // TODO: publish to RabbitMQ
                        msg.ProcessedOn = DateTime.UtcNow;
                    }

                    if (messages.Count > 0)
                    {
                        await db.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Processed {Count} outbox messages", messages.Count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing outbox");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
