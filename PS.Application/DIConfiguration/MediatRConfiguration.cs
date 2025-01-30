using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace PS.Application.DIConfiguration
{
    public static class MediatRConfiguration
    {
        public static void AddMediatr(this IServiceCollection services, ILogger logger)
        {
            logger.LogInformation("Register Services From Assemblies...");

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
            });

            logger.LogInformation("Added MediatR");
        }
    }
}
