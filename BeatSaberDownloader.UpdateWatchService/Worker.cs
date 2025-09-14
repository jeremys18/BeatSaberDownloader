using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;

namespace BeatSaberDownloader.UpdateWatchService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service starting at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using SocketsHttpHandler handler = new();
                    using ClientWebSocket ws = new();
                    await ws.ConnectAsync(new Uri("wss://ws.beatsaver.com/maps"), new HttpMessageInvoker(handler), stoppingToken);

                    var buffer = new byte[1024 * 32];
                    while (ws.State != WebSocketState.CloseReceived && !stoppingToken.IsCancellationRequested)
                    {
                        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken).ConfigureAwait(false);
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var type = Regex.Match(message, @"type"":\s*""(\w+)""").Groups[1].Value;
                        var id = type == "MAP_DELETE" ? Regex.Match(message, @"msg"":\s*""(\w+)""").Groups[1].Value : Regex.Match(message, @"id"":\s*""(\w+)""").Groups[1].Value;
                        _logger.LogInformation($"Received message: {type} Id: {id}");

                        File.WriteAllText(GetFileName(id), message); // Save the message to a file
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in service loop: {Message}", ex.Message);
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Wait before retrying
                }
            }
        }

        static string GetFileName(string id)
        {
            var fileNum = 1;
            var fileName = @$"C:\BeatSaber\Updates\{id}.json";
            while (File.Exists(fileName))
            {
                fileName = @$"C:\BeatSaber\Updates\{id}_{fileNum++}.json";
            }
            return fileName;
        }
    }
}
