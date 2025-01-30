
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using ExchangeWebSocketClient.Models;
using Microsoft.Extensions.Configuration;

// Configure logging
var logger = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
}).CreateLogger<Program>();

// Load configuration from appsettings.json
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string webSocketUri = config["WebSocketServer:Uri"] ?? throw new ArgumentNullException("WebSocket URI is not configured.");

logger.LogInformation($"Listening {webSocketUri}...");

var wsClient = new ClientWebSocket();
try
{
    // Connect to WebSocket server
    await wsClient.ConnectAsync(new Uri(webSocketUri), CancellationToken.None);
    logger.LogInformation($"Client connected to the server WebSocket at {webSocketUri}");

    // Start receiving messages
    await ReceiveMessages(wsClient, logger);
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while connecting or receiving messages.");
}
finally
{
    if (wsClient.State == WebSocketState.Open)
    {
        await wsClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
    }
    wsClient.Dispose();
}

/// <summary>
/// Receives messages from the WebSocket server.
/// </summary>
static async Task ReceiveMessages(ClientWebSocket webSocket, ILogger logger)
{
    logger.LogInformation("Start receiving messages...");

    var buffer = new byte[1024];
    while (webSocket.State == WebSocketState.Open)
    {
        try
        {
            // Receive a message from WebSocket
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                logger.LogInformation("WebSocket connection closed.");
            }
            else
            {
                // Decode and deserialize the message
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var priceUpdate = JsonSerializer.Deserialize<PriceUpdate>(message);

                if (priceUpdate != null)
                {
                    logger.LogInformation("Price update: {Instrument} - {Price}", priceUpdate.Instrument, priceUpdate.Price);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while receiving messages.");
            break;
        }
    }
}
