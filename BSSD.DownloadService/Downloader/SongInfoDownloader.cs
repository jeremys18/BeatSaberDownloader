using BeatSaberDownloader.Data.Consts;
using BeatSaberDownloader.Data.Models;
using BeatSaberDownloader.Data.Models.Search;
using BSSD.DownloadService.API;
using Newtonsoft.Json;

namespace BSSD.DownloadService.Downloader
{
    public static class SongInfoDownloader
    {
        public static async Task StartAsync()
        {
            await DownloadSongInfoAsync();
        }

        private static async Task DownloadSongInfoAsync()
        {
            DateTime startDate = DateTime.Parse(BeatSaverConsts.DefaultBeatSaverAfterDate);
            var now = DateTime.Now;
            var songs = new List<MapDetail>();

            while (startDate < now.AddDays(-1))
            {
                var songString = await BeatSaverAPI.SearchSongsAsync(startDate);
                if (!string.IsNullOrEmpty(songString))
                {
                    var newSongs = JsonConvert.DeserializeObject<SearchResponse>(songString) ?? null;
                    if (newSongs == null) continue;
                    songs.AddRange(newSongs.docs);
                    var maxDate = newSongs.docs.Select(x => x.uploaded).Max(); // Cant use created date as they dont use that date for the search
                    Console.WriteLine($"Found {newSongs.docs.Length} songs after {startDate:yyyy-MM-dd}. Next start date will be {maxDate:yyyy-MM-dd}");
                    startDate = maxDate;
                }
                else
                {
                    // If no songs are found log it
                    Console.WriteLine("No songs found for the given date range.");
                }
            }

            ProcessSongInfo([.. songs]);
        }

        private static void ProcessSongInfo(MapDetail[] songs)
        {
            // Convert the JSON response to a list of SearchResponse objects
            // Logic to process the downloaded song info
            // This is a placeholder for the actual implementation
            // You would typically parse the JSON or XML response and store it in a database or file

            // save to file
            var text = JsonConvert.SerializeObject(songs, Formatting.Indented);
            var filePath = Path.Combine(@"g:\BeatSaber", "songs.json");
            Directory.CreateDirectory(@"g:\BeatSaber"); // Ensure the directory exists
            File.WriteAllText(filePath, text);
            Console.WriteLine($"Song info downloaded. Found {songs.Length} songs.");
            // start db updator

        }
    }
}
