using BeatSaberDownloader.Data.Consts;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Serialization;
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
                        var requiresTemp = !IsValidJson(message);

                        // This is VERY annoying but.... A message seems to have a max size and if it exceeds that size it gets split into multiple messages
                        // The first part of every message has the id so if json is invalid and it has an id then save to temp
                        // If it doesnt have an id but is invalid json then this is the second part of a message so append to temp and process as normal
                        if (requiresTemp && !string.IsNullOrWhiteSpace(id))
                        {
                            _logger.LogWarning("\tReceived partial JSON message: for {id}. Saving to temp....", id);
                        }
                        else if(requiresTemp && File.Exists(Path.Combine(BeatSaverConsts.BeatSaverDataDirectory, BeatSaverConsts.TempSongFile)))
                        {
                            _logger.LogWarning("\tReceived partial JSON message with no id. Appending to temp and processing...");
                            var temp = File.ReadAllText(Path.Combine(BeatSaverConsts.BeatSaverDataDirectory, BeatSaverConsts.TempSongFile));
                            message = temp + message;
                            File.Delete(Path.Combine(BeatSaverConsts.BeatSaverDataDirectory, BeatSaverConsts.TempSongFile));
                        }

                        // If its the first part of a message and it requires temp then just save to temp, otherwise save to normal file
                        var fileName = requiresTemp && !string.IsNullOrWhiteSpace(id) ? Path.Combine(BeatSaverConsts.BeatSaverDataDirectory, BeatSaverConsts.TempSongFile) : GetFileName(id);
                        File.WriteAllText(fileName, message); // Save the message to a file
                        _logger.LogInformation("\tSaved file {fileName}", fileName);
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
            var fileName = @$"G:\BeatSaber\Updates\{id}.json";
            while (File.Exists(fileName))
            {
                fileName = @$"G:\BeatSaber\Updates\{id}_{fileNum++}.json";
            }
            return fileName;
        }

        static bool IsValidJson(string strInput)
        {
            try
            {
                JsonConvert.DeserializeObject(strInput);
                return true;
            }
            catch // not valid
            {
                return false;
            }
        }
    }
}
