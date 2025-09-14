using Newtonsoft.Json;

namespace BeatSaberDownloader.DBUpdateService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private FileSystemWatcher? _watcher;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _watcher = new FileSystemWatcher(@"C:\BeatSaver\Updates")
            {
                NotifyFilter = NotifyFilters.FileName,
                EnableRaisingEvents = true,
                IncludeSubdirectories = false
            };

            _watcher.Created += (sender, e) => ProcessUpdate(sender, e);
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

        public string GetNextFileVersion(string path)
        {
            var nameParts = path.Split('_');
            var curVersion = nameParts.Length > 1 ? int.Parse(nameParts[1]) : 1;
            var nextVer = ++curVersion;

            return $"{nameParts[0]}_{nextVer}";
        }

        public void ProcessUpdate(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation($"New file detected. Processing {e.Name}....");
            // Ensure there is not already a file with a name _{filenum} that is greater than current file (a newer file). If there is then disregard this update and delete file (update will be in later file)
            if (File.Exists(GetNextFileVersion(e.FullPath)))
            {
                _logger.LogInformation($"\tA newer file exists for {e.Name}. Deleting file. Will process update in newer file instead...");
                File.Delete(e.FullPath);
                return;
            }


            // Process the update for the DB
            _logger.LogInformation("\tUpdating the song info in the DB....");
            // TODO: Update DB

            // Process the update for the json file
            _logger.LogInformation("\tUpdating the song info in the json file....");
            var jsonFilename = @"c:\BeatSaber\songs.json";
            var currentJsonText = File.ReadAllText(jsonFilename);
            var songs = JsonConvert.DeserializeObject(currentJsonText);

            // TODO: get song
            var song = ""; // Grab the current song by id 

            // If the update is to the version or filename or author then redownload file (aka, they added a version or renamed the file) otherwise no need to redownload
            // TODO: Compare the updated info
            if (false)
            {
                _logger.LogInformation("\tThe update requires the song be downloaded. Writting info to {id}.json under Songs awaiting download folder...");
                File.WriteAllText(@"c:\BeatSaber\Songs awaiting download\{id}.json", "{Name: \"Test\", url: \"test url\"}");
            }

            // Delete the file after processing
            _logger.LogInformation($"\tFinished processing update. Deleting file {e.Name}...");
            //File.Delete(e.FullPath);
        }
    }
}
