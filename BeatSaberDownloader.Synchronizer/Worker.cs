using BeatSaberDownloader.Data.Consts;
using BeatSaberDownloader.Data.DBContext;
using BeatSaberDownloader.Data.Models;
using BeatSaberDownloader.Data.Models.Search;
using BeatSaberDownloader.Data.Models.UpdateSrvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BeatSaberDownloader.Synchronizer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        // Use a simple API: pass the method and a time string like "08:00" (daily) or a cron string.
        // Example: await CronRunner.ScheduleAsync(Sync, "08:00", stoppingToken);
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Schedule Sync to run daily at 08:00 local time with a single call.
            return CronRunner.ScheduleAsync(Sync, "08:00", stoppingToken);
        }

        private async Task Sync(CancellationToken cancellationToken)
        {
            // Implement sync logic here.
            _logger.LogInformation("Sync running at: {time}", DateTimeOffset.Now);
            DateTime startDate = DateTime.Parse(BeatSaverConsts.DefaultBeatSaverAfterDate);
            var now = DateTime.Now;
            var songs = new List<MapDetail>();

            while (startDate <= now)
            {
                var songString = await BeatSaverAPI.SearchSongsAsync(startDate);
                if (!string.IsNullOrEmpty(songString))
                {
                    var newSongs = JsonConvert.DeserializeObject<SearchResponse>(songString) ?? null;
                    if (newSongs == null)
                    {
                        startDate = now.AddDays(1);
                        continue;
                    }
                    var distinctSongs = newSongs.docs.ExceptBy(songs.Select(x => x.id), x => x.id).ToList();
                    songs.AddRange(distinctSongs);
                    var maxDate = newSongs.docs.Select(x => x.uploaded).Max(); // Cant use created date as they dont use that date for the search
                    Console.WriteLine($"Found {newSongs.docs.Length} songs after {startDate:yyyy-MM-dd}. Next start date will be {maxDate:yyyy-MM-dd}");
                    startDate = maxDate!.Value;
                }
                else
                {
                    // If no songs are found log it
                    _logger.LogInformation("No songs found for the given date range.");
                }
            }

            ProcessSongInfo([.. songs]);
        }

        private void ProcessSongInfo(MapDetail[] songs)
        {
            // save to file first
            var text = JsonConvert.SerializeObject(songs, Formatting.Indented);
            var filePath = Path.Combine(BeatSaverConsts.BeatSaverDataDirectory, "songs.json");
            Directory.CreateDirectory(BeatSaverConsts.BeatSaverDataDirectory); // Ensure the directory exists
            File.WriteAllText(filePath, text);
            _logger.LogInformation($"Song info downloaded. Found {songs.Length} songs on server. Syncing with DB.");

            // Now sync it with the DB
            var dbSongs = new BeatSaverContext()
                .Songs
                .Include(x => x.Metadata)
                .Include(x => x.Stats)
                .Include(x => x.Tags)
                .Include(x => x.Versions)
                    .ThenInclude(v => v.Difficulties)
                        .ThenInclude(d => d.ParitySummary)
                .Include(x => x.Uploader).ToList();

            // Since we already have a service that Upserts songs, we can just call that service by creating a file for it and letting the other service handle the logic
            // instead of implementing the logic again. This means we simply need to know if the song exists and has been updated, if it has been deleted, or if its a new song.
            // If none of those, the song is current and we move on. This is why we only run this once a day (its got to loop and check every song).
            var deletedSongs = dbSongs.ExceptBy(songs.Select(s => s.id), x => x.Id).ToList();
            var updatedSongs = dbSongs.IntersectBy(songs.Select(s => s.id), x => x.Id).Where(x => IsUpdated(songs.First(y => y.id == x.Id), x)).ToList();
            var newSongs = songs.ExceptBy(dbSongs.Select(s => s.Id), x => x.id).ToList();

            foreach (var s in deletedSongs)
            {
                var json = JsonConvert.SerializeObject(new UpdateInfo
                {
                    Type = "MAP_DELETE",
                    msg = s.Id
                }, Formatting.Indented);
                File.WriteAllText(GetFileName(s.Id), json);
            }

            foreach (var s in updatedSongs)
            {
                var json = JsonConvert.SerializeObject(new UpdateInfo
                {
                    Type = "MAP_UPDATE",
                    msg = JsonConvert.SerializeObject(songs.First(x => x.id == s.Id), Formatting.Indented)
                }, Formatting.Indented);

                File.WriteAllText(GetFileName(s.Id), json);
            }

            foreach(var s in newSongs)
            {
                var json = JsonConvert.SerializeObject(new UpdateInfo
                {
                    Type = "MAP_UPDATE",
                    msg = JsonConvert.SerializeObject(s, Formatting.Indented)
                }, Formatting.Indented);

                File.WriteAllText(GetFileName(s.id), json);
            }
            
            _logger.LogInformation($"DB Sync complete. {newSongs.Count} new songs, {updatedSongs.Count} updated songs, deleted songs {deletedSongs.Count}.");
        
        }

        private bool IsUpdated(MapDetail song, Data.Models.DbModels.Song dbSong)
        {
            // check if author, etc updated
            if(song.uploader.name != dbSong!.Uploader.Name || song.description != dbSong.Description || song.name != dbSong.Name)
                return true;
            // check if any versions added/deleted
            else if (song.versions.Length != dbSong.Versions.Count)
                return true;

            // check if any urls have changed
            else if(song.versions.Any(v => v.downloadURL != dbSong.Versions.First(dbv => dbv.Hash == v.hash).DownloadURL))
                return true;

            // Nothing important has changed so just return false
            return false;
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
    }
}
