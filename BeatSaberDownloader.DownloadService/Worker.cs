using BeatSaberDownloader.Data.Models;
using BeatSaberDownloader.Data.Models.UpdateSrvc;
using BeatSaberDownloader.Data.Extentions;
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
            _watcher = new FileSystemWatcher(@"G:\BeatSaber\Songs awaiting download")
            {
                NotifyFilter = NotifyFilters.FileName,
                EnableRaisingEvents = true,
                IncludeSubdirectories = false
            };
            _watcher.Created += (sender, e) => ProcessUpdateAsync(sender, e);
        }

        protected async Task ProcessUpdateAsync(object sender, FileSystemEventArgs e)
        {
            try
            {
                // Wait for the file to be fully written
                Task.Delay(2000).Wait(); 

                _logger.LogInformation("Processing new file: {filename}", e.FullPath);

                // Get the song info
                var fileText = File.ReadAllText(e.FullPath);
                var info = JsonConvert.DeserializeObject<DownloadInfo>(fileText) ?? throw new Exception("Unable to parse download info file...");

                // download the song
                try
                {
                    // Because Beat Saver constantly changes the urls to download songs from, we need to handle potential failures here.
                    await DownloadSong(info);
                }
                catch (Exception downloadEx)
                {
                    _logger.LogWarning("Initial download attempt failed for {filename}. Attempting to get updated URL from songs.json...", info.Filename);
                    var jsonFilePath = @"G:\BeatSaber\songs.json";
                    var songData = string.Empty;
                    using (var stream = jsonFilePath.GetFileAccess(FileMode.Open, FileAccess.Read))
                    using (var reader = new StreamReader(stream))
                    {
                        songData = await reader.ReadToEndAsync();
                    }
                    try
                    {
                        var songs = JsonConvert.DeserializeObject<MapDetail[]>(songData) ?? throw new Exception("Unable to parse songs.json file...");
                        var curUrl = songs.First(s => s.id == info.Id).versions.First(y => y.hash.EndsWith(info.Hash)).downloadURL ?? throw new Exception("Could not find song in songs.json to get updated download URL...");

                        // Retry with new URL. 
                        await DownloadSong(new DownloadInfo
                        {
                            Id = info.Id,
                            Hash = info.Hash,
                            Filename = info.Filename,
                            DownloadURL = curUrl
                        });
                    }
                    catch (Exception ee)
                    {
                        _logger.LogError("Backup URL failed. {e}", ee.Message);
                    }
                }

                // Delete the file
                File.Delete(e.FullPath);
                _logger.LogInformation("Deleted file: {filename}", e.FullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error processing update file {file}: {Message}", e.FullPath, ex.Message);
            }
        }

        private async Task DownloadSong(DownloadInfo info)
        {
            var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(info.DownloadURL, HttpCompletionOption.ResponseContentRead);
            response.EnsureSuccessStatusCode();

            await using var contentStream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = new FileStream(info.Filename, FileMode.Create, FileAccess.Write, FileShare.None);
            await contentStream.CopyToAsync(fileStream);
            await contentStream.FlushAsync();
            _logger.LogInformation($"Downloaded new file: {info.Filename}");
            await contentStream.DisposeAsync();
            await fileStream.DisposeAsync();
            response.Dispose();
            httpClient.Dispose();
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
