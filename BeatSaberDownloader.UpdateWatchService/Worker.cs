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
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker starting at: {time}", DateTimeOffset.Now);
                }
                using SocketsHttpHandler handler = new();
                using ClientWebSocket ws = new();
                await ws.ConnectAsync(new Uri("wss://ws.beatsaver.com/maps"), new HttpMessageInvoker(handler), new CancellationToken());

                var buffer = new byte[1024 * 32];
                while (ws.State != WebSocketState.CloseReceived)
                {
                    try
                    {
                        var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false);

                        //Here is the received message as string
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        var type = Regex.Match(message, @"type"":\s*""(\w+)""").Groups[1].Value;
                        var id = type == "MAP_DELETE" ? Regex.Match(message, @"msg"":\s*""(\w+)""").Groups[1].Value : Regex.Match(message, @"id"":\s*""(\w+)""").Groups[1].Value;
                        _logger.LogInformation($"Received message: {type} Id: {id}");

                        File.WriteAllText(GetFileName(id), message); // Save the message to a file
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error in receiving messages: {err}", ex.Message);
                    }
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
