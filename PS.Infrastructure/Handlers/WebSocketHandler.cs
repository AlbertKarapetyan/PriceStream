using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace PS.Infrastructure.Handlers
{
    /// <summary>
    /// Manages WebSocket connections and message broadcasting to multiple clients.
    /// </summary>
    public class WebSocketHandler
    {
        // Stores connected WebSocket clients in a thread-safe collection
        private readonly ConcurrentDictionary<WebSocket, bool> _sockets = new();
        private readonly ILogger<WebSocketHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the WebSocketHandler.
        /// </summary>
        /// <param name="logger">Logger instance for logging connection events.</param>
        public WebSocketHandler(ILogger<WebSocketHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Adds a new WebSocket client and listens for incoming messages.
        /// </summary>
        /// <param name="socket">The WebSocket connection.</param>
        public async Task AddClient(WebSocket socket)
        {
            _logger.LogInformation("Connected new client");

            _sockets.TryAdd(socket, true);
            var buffer = new byte[1024];
            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                        break;
                }

            }
            catch (WebSocketException ex)
            {
                _logger.LogError($"WebSocket error: {ex.Message}");
            }
            finally
            {
                _logger.LogInformation("Client disconnected");
                _sockets.TryRemove(socket, out _);
            }
        }

        /// <summary>
        /// Broadcasts a message to all connected WebSocket clients concurrently.
        /// </summary>
        /// <param name="message">The message to send.</param>
        public async void Broadcast(string message)
        {
            _logger.LogInformation($"Broadcasting message to {_sockets.Count} clients");

            var data = Encoding.UTF8.GetBytes(message);
            
            var tasks = new List<Task>();

            // Send messages concurrently to all connected clients
            foreach (var socket in _sockets.Keys)
            {
                if (socket.State == WebSocketState.Open)
                {
                    tasks.Add(SendMessage(socket, data));
                }
                else
                {
                    _sockets.TryRemove(socket, out _);
                }
            }

            // Sequential sending: ~10ms per message × 1000 clients = 10s total
            // Parallel sending: ~10ms for all clients = 10ms total 
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Sends a message to a specific WebSocket client.
        /// </summary>
        /// <param name="socket">The target WebSocket.</param>
        /// <param name="data">The message data in byte format.</param>
        private async Task SendMessage(WebSocket socket, byte[] data)
        {
            try
            {
                await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (WebSocketException)
            {
                _logger.LogWarning("Error sending message. Removing client...");
                _sockets.TryRemove(socket, out _);
            }
        }
    }
}
