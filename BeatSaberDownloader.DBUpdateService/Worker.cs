using BeatSaberDownloader.Data.Extentions;
using BeatSaberDownloader.Data.Models;
using BeatSaberDownloader.Data.Models.UpdateSrvc;
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
            _watcher = new FileSystemWatcher(@"G:\BeatSaber\Updates")
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
            var curVersion = nameParts.Length > 1 ? int.Parse(nameParts[1].Replace(".json", string.Empty)) : 1;
            var nextVer = ++curVersion;

            return $"{nameParts[0]}_{nextVer}";
        }

        public void ProcessUpdate(object sender, FileSystemEventArgs e)
        {
            try
            {
                var jsonFilename = @"G:\BeatSaber\songs.json";
                _logger.LogInformation($"New file detected. Processing {e.Name}....");
                // Ensure there is not already a file with a name _{filenum} that is greater than current file (a newer file). If there is then disregard this update and delete file (update will be in later file)
                if (File.Exists(GetNextFileVersion(e.FullPath)))
                {
                    _logger.LogInformation($"\tA newer file exists for {e.Name}. Deleting file. Will process update in newer file instead...");
                    File.Delete(e.FullPath);
                    return;
                }

                var updateInfo = JsonConvert.DeserializeObject<UpdateInfo>(File.ReadAllText(e.FullPath)) ?? throw new NullReferenceException("Could not decerialize the update file...");
                var currentJsonText = File.ReadAllText(jsonFilename);
                var songs = JsonConvert.DeserializeObject<MapDetail[]>(currentJsonText) ?? throw new NullReferenceException("Could not deserialize current song list...");

                if (updateInfo.msg is string)
                {
                    var song = songs.First(x => x.id == (string)updateInfo.msg);
                    songs = songs.Where(x => x.id != (string)updateInfo.msg).ToArray();
                    DeleteSong(song);
                }
                else
                {
                    UpdateSong(updateInfo, songs);
                    
                }

                // Save new state of json
                File.WriteAllText(jsonFilename, JsonConvert.SerializeObject(songs, Formatting.Indented));
                _logger.LogInformation($"\tUpdated {jsonFilename}....");

                // Delete the file after processing
                _logger.LogInformation($"\tFinished processing update. Deleting file {e.Name}...");
                File.Delete(e.FullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing update file {filename}: {Message}", e.Name, ex.Message);
            }
        }

        public void UpdateSong(UpdateInfo info, List<MapDetail> songs)
        {
            var mapInfo = JsonConvert.DeserializeObject<MapDetail>(info.msg.ToString());
            var song = songs.FirstOrDefault(x => x.id == mapInfo.id);
            var basePath = @"G:\BeatSaber\SongFiles";
            var songsToDownload = new List<DownloadInfo>();

            // Process the update for the json file
            _logger.LogInformation("\tUpdating the song info in the json file....");


            // Update the song info
            if (song == null)
            {
                _logger.LogWarning($"\tSong with id {mapInfo.id} not found in songs.json. Adding to json...");
                songs.Add(mapInfo);
                // Todo: Add the song to the DB
                var files = mapInfo.GetValidFileNames(basePath);
                songsToDownload.AddRange(files.Select(f => new DownloadInfo
                {
                    Id = mapInfo.id,
                    Filename = f.Value,
                    DownloadURL = mapInfo.versions.First(x => x.hash == f.Key).downloadURL
                }));
            }
            else
            {
                // check song name, author, versions
                var versionHashes = song.versions.Select(v => v.hash);
                var deletedVers = versionHashes.Except(mapInfo.versions.Select(v => v.hash)).ToList();
                var newVers = mapInfo.versions.Select(v => v.hash).Except(versionHashes).ToList();
                var existingVers = versionHashes.IntersectBy(mapInfo.versions.Select(v => v.hash), v => v).ToList();
                var hasNameChanged = mapInfo.name != song.name;
                var hasAuthorChanged = mapInfo.metadata.songAuthorName != song.metadata.songAuthorName;
                var hasUploaderNameChanged = mapInfo.uploader.name != song.uploader.name;
                var currFiles = song.GetValidFileNames(basePath);
                var newFiles = mapInfo.GetValidFileNames(basePath);

                foreach (var del in deletedVers)
                {
                    _logger.LogInformation($"\tThe update indicates the version with hash {del} has been deleted. Marking DB entry as deleted and moving the file...");
                    // TODO: Mark the version as deleted in the DB
                    // TODO: Move the file to the deleted folder
                    var currFile = currFiles[del];
                    File.Move(currFile, $@"G:\BeatSaber\Deleted\{currFile.Split("\\").Last()}");
                }
                foreach (var ver in newVers)
                {
                    _logger.LogInformation($"\tThe update indicates the version with hash {ver} has been added. Adding to DB and marking for download...");
                    // TODO: Add the version to the DB
                    // TODO: Mark the version for download
                    songsToDownload.Add(new DownloadInfo
                    {
                        Id = mapInfo.id,
                        Filename = newFiles[ver],
                        DownloadURL = mapInfo.versions.First(v => v.hash == ver).downloadURL
                    });
                }
                if (hasNameChanged || hasAuthorChanged || hasUploaderNameChanged) 
                {
                    _logger.LogInformation($"\tThe update indicates the name or the author or the uploader has changed. This changes the filename. Updating the DB entry and renaming the files...");
                    // TODO: Update the DB
                    foreach (var ver in existingVers)
                    {
                        songsToDownload.Add(new DownloadInfo
                        {
                            Filename = newFiles[ver],
                            DownloadURL = mapInfo.versions.First(v => v.hash == ver).downloadURL
                        });
                        var oldFileName = currFiles[ver];
                        var newFileName = newFiles[ver];
                        File.Move(oldFileName, newFileName);
                    }
                }
                var index = songs.IndexOf(song);
                songs[index] = mapInfo;
            }
            

            foreach (var s in songsToDownload)
            {
                var text = JsonConvert.SerializeObject(s, Formatting.Indented);
                File.WriteAllText($@"G:\BeatSaber\Songs awaiting download\{s.Id}.json", text);
            }
        }

        public void DeleteSong(MapDetail song)
        {
            _logger.LogInformation("\tDeleting the song from the DB....");
            _logger.LogInformation("\tMoving the song files to the deleted folder");
            Directory.GetFiles(@"G:\BeatSaber\SongFiles", $"{song.id}*").ToList().ForEach(f =>
            {
                var filename = Path.GetFileName(f);
                File.Move(f, $@"G:\BeatSaber\DeletedSongs\{filename}");
            });
        }
    }
}
