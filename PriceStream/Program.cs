using PS.Application;
using PS.Application.DIConfiguration;
using PS.Application.Extensions;
using PS.Application.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for ELK logging using custom configuration class
LoggingConfiguration.ConfigureSerilog(builder);

// Configure logging
var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

// Add services to the container.
builder.Services.AddServices(loggerFactory.CreateLogger("DIServicesRegistration"));

// Add Mediatr to the container.
builder.Services.AddMediatr(loggerFactory.CreateLogger("MediatRConfiguration"));

builder.Services.AddControllers();

builder.Services.AddHealthChecks();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SignalR
builder.Services.AddSignalR();

// Define CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("BasicCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()  // Allow all origins
              .AllowAnyHeader()   // Allow all headers
              .WithMethods("GET") // Allow only GET methods
              .AllowCredentials();
    });
});

// Register the WebSocket background service
builder.Services.AddHostedService<ExchangeWebSocketBackgroundService>();

Console.WriteLine("Starting...");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Add the health check endpoint
app.MapHealthChecks("/health");

// Enable WebSockets
app.UseWebSockets();

app.MapMyHub(); // Map SignalR hub to /ws

app.MapControllers();

app.Run();
