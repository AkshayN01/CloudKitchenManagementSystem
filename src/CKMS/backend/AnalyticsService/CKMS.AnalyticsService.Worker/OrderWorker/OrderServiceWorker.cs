using CKMS.Interfaces.Queue;

namespace CKMS.AnalyticsService.Worker.OrderWorker
{
    public class OrderServiceWorker : BackgroundService
    {
        private readonly IRabbitMQ _rabbitMq;
        private readonly ILogger<OrderServiceWorker> _logger;

        public OrderServiceWorker(ILogger<OrderServiceWorker> logger, IRabbitMQ rabbitMQ)
        {
            _logger = logger;
            _rabbitMq = rabbitMQ;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task GetOrderDetails()
        {
            //read data from the Queue first
            //only ready those data that have the entity type Order

            //add to DB

            //remove it from the queue
        }
    }
}
