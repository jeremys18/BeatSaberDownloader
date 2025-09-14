using Newtonsoft.Json;

namespace BeatSaberDownloader.DownloadService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private FileSystemWatcher? _watcher;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _watcher = new FileSystemWatcher(@"C:\BeatSaver\Songs awaiting download")
            {
                NotifyFilter = NotifyFilters.FileName,
                EnableRaisingEvents = true,
                IncludeSubdirectories = false
            };
            _watcher.Created += (sender, e) =>
            {
                // Download the file
                var fileText = File.ReadAllText(e.FullPath);
                var info = JsonConvert.DeserializeObject(fileText);

                _logger.LogInformation("Downloaded new file: {filename}", e.FullPath);

                // Delete the file
                File.Delete(e.FullPath);
                _logger.LogInformation("Deleted file: {filename}", e.FullPath);
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service starting at: {time}", DateTimeOffset.Now);

            try
            {
                // Keep the service alive until cancellation is requested
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in service loop: {Message}", ex.Message);
            }
            finally
            {
                _watcher?.Dispose();
            }
        }
    }
}
