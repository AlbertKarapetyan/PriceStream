using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PS.Domain.Interfaces;
using PS.Infrastructure.Handlers;
using PS.Infrastructure.Services;

namespace PS.Application.DIConfiguration
{
    public static class DIServicesRegistration
    {
        public static void AddServices(this IServiceCollection services, ILogger logger)
        {
            logger.LogInformation($"Registering services");

            services.AddScoped<IInstrumentService, InstrumentService>();
            services.AddSingleton<IExchangeWebSocketService, BinanceWebSocketService>();

            logger.LogInformation($"Registering Web Socket Handler");

            services.AddSingleton<WebSocketHandler>();


        }
    }
}
