using Serilog;
using Serilog.Sinks.OpenSearch;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace PS.Application.Logging
{
    public static class LoggingConfiguration
    {
        public static void ConfigureSerilog(WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((ctx, cfg) =>
            {
                // Retrieve OpenSearch URL from appsettings.json
                var openSearchUrl = builder.Configuration.GetSection("OpenSearch:Url").Value
                                    ?? "http://localhost:9200";

                cfg.ReadFrom.Configuration(builder.Configuration)
                   .Enrich.FromLogContext();

                // Console logging only in development
                if (ctx.HostingEnvironment.IsDevelopment())
                {
                    cfg.WriteTo.Console();
                }

                // General application logs
                cfg.WriteTo.OpenSearch(new OpenSearchSinkOptions(new Uri(openSearchUrl))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = "logstash-{0:yyyy.MM.dd}",
                    BatchPostingLimit = 1000,  // Number of logs per batch
                    Period = TimeSpan.FromSeconds(5) // Interval for sending logs
                });

                // Instrument-specific logs (filtered separately)
                cfg.WriteTo.Logger(lc => lc
                   .Filter.ByIncludingOnly(logEvent =>
                       logEvent.Properties.ContainsKey("Instrument"))
                   .WriteTo.OpenSearch(new OpenSearchSinkOptions(new Uri(openSearchUrl))
                   {
                       AutoRegisterTemplate = true,
                       IndexFormat = "instrument-{0:yyyy.MM.dd}",
                       BatchPostingLimit = 1000,  // Number of logs per batch
                       Period = TimeSpan.FromSeconds(5) // Interval for sending logs
                   })
               );
            });
        }
    }
}
