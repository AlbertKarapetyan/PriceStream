using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PS.Domain.Interfaces;

namespace PS.Application
{
    public class ExchangeWebSocketBackgroundService : IHostedService
    {
        private readonly IExchangeWebSocketService _webSocketService;
        private readonly ILogger<ExchangeWebSocketBackgroundService> _logger;

        public ExchangeWebSocketBackgroundService(
            IExchangeWebSocketService webSocketService,
            ILogger<ExchangeWebSocketBackgroundService> logger)
        {
            _webSocketService = webSocketService;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting WebSocket Service...");
            await _webSocketService.ConnectAndSubscribe();
            _logger.LogInformation("WebSocket Service started.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping WebSocket Service...");
            return _webSocketService.Disconnect();
        }
    }
}
