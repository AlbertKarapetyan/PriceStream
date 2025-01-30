using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PS.Infrastructure.Handlers;


namespace PS.Application.Extensions
{
    public static class SignalRExtensions
    {
        public static WebApplication MapMyHub(this WebApplication app)
        {
            // Map the SignalR hub to the "/pricehub" endpoint
            app.Map("/pricehub", async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await app.Services.GetRequiredService<WebSocketHandler>().AddClient(webSocket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            });
            return app;
        }
    }
}
