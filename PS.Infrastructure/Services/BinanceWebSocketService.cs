using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PS.Domain.Interfaces;
using PS.Domain.Models;
using PS.Infrastructure.Handlers;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace PS.Infrastructure.Services
{
    /// <summary>
    /// BinanceWebSocketService handles WebSocket connections to Binance, subscribing to price updates 
    /// and broadcasting received data to clients.
    /// </summary>
    public class BinanceWebSocketService : IExchangeWebSocketService, IDisposable
    {
        private readonly ILogger<BinanceWebSocketService> _logger;
        private readonly IConfiguration _configuration;
        private readonly WebSocketHandler _webSocketHandler;
        private readonly ClientWebSocket _webSocket = new();

        /// <summary>
        /// Initializes a new instance of BinanceWebSocketService.
        /// </summary>
        /// <param name="webSocketHandler">Handles broadcasting WebSocket messages.</param>
        /// <param name="logger">Logger instance for logging errors and events.</param>
        /// <param name="configuration">Configuration settings for WebSocket connection and subscriptions.</param>
        public BinanceWebSocketService(
            WebSocketHandler webSocketHandler,
            ILogger<BinanceWebSocketService> logger,
            IConfiguration configuration
            )
        {
            _logger = logger;
            _configuration = configuration;
            _webSocketHandler = webSocketHandler;
        }

        /// <summary>
        /// Connects to Binance WebSocket and subscribes to configured price updates.
        /// </summary>
        public async Task ConnectAndSubscribe()
        {
            try
            {
                var exchangeWebSocketUri = _configuration["ExchangeWebSocketUri"];

                if (string.IsNullOrEmpty(exchangeWebSocketUri))
                {
                    _logger.LogError("ExchangeWebSocketUri is missing in configuration.");
                }
                else
                {
                    _logger.LogInformation($"WebSocket URI: {exchangeWebSocketUri}");
                }

                // Establish WebSocket connection
                await _webSocket.ConnectAsync(new Uri(exchangeWebSocketUri!), CancellationToken.None);

                // Retrieve subscription parameters from configuration
                var subscriptionParams = _configuration.GetSection("WebSocketSubscriptions").Get<string[]>() ?? Array.Empty<string>();

                var subscribeMessage = new
                {
                    method = "SUBSCRIBE",
                    @params = subscriptionParams,
                    id = 1
                };

                // Send subscription request
                await _webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(subscribeMessage))), WebSocketMessageType.Text, true, CancellationToken.None);

                // Start receiving messages asynchronously
                _ = ReceiveMessages();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while connecting and subscribing to the WebSocket.");
                throw;
            }
        }

        /// <summary>
        /// Listens for incoming messages from the WebSocket and processes price updates.
        /// </summary>
        private async Task ReceiveMessages()
        {
            var buffer = new byte[1024];

            while (_webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    }
                    else
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        // Deserialize the received message into a PriceUpdate object
                        var priceUpdate = JsonSerializer.Deserialize<PriceUpdate>(message);

                        if (priceUpdate != null)
                        {
                            try
                            {
                                if (priceUpdate.Instrument != null)
                                {
                                    // Update price cache with received data
                                    PriceCache.UpdatePrice(priceUpdate.Instrument, Convert.ToDecimal(priceUpdate.Price));

                                    // Broadcast price update to connected clients
                                    _webSocketHandler.Broadcast(JsonSerializer.Serialize(new { Instrument = priceUpdate.Instrument, Price = priceUpdate.Price }));

                                    _logger.LogInformation("Price update received: {Instrument} - {Price}", priceUpdate.Instrument, priceUpdate.Price);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"An error occurred while updating price {priceUpdate.Instrument}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while receiving messages from the WebSocket.");
                    break; // Exit the loop if an error occurs
                }
            }
        }

        /// <summary>
        /// Closes the WebSocket connection.
        /// </summary>
        public async Task Disconnect()
        {
            try
            {
                if (_webSocket.State == WebSocketState.Open || _webSocket.State == WebSocketState.Connecting)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", CancellationToken.None);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while disconnecting from the WebSocket.");
            }
            finally
            {
                Dispose();
            }
        }

        /// <summary>
        /// Disposes the WebSocket instance and releases resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                _webSocket.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while disposing the WebSocket.");
            }
        }
    }
}
