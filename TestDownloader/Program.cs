
using BeatSaberDownloader.Data.Models;
using Newtonsoft.Json;
using TestDownloader;

//await SongInfoDownloader.StartAsync();

internal class Program
{
    private static async Task Main(string[] args)
    {
        var basePath = @"g:\BeatSaber\SongFiles";
        var jsonFilename = @"g:\BeatSaber\songs.json";
        var currentJsonText = File.ReadAllText(jsonFilename);
        var songs = JsonConvert.DeserializeObject<MapDetail[]>(currentJsonText) ?? throw new NullReferenceException("Could not deserialize current song list...");
        var missingCount = 0;
        var missingSongs = new List<MapDetail>();
        var dups = songs.DistinctBy(x => x.id).ToList();
        File.WriteAllText(@"g:\BeatSaber\songs2.json", JsonConvert.SerializeObject(dups, Formatting.Indented));
        foreach (var song in songs.OrderByDescending(x => x.id))
        {
            var filesNames = song.GetValidFileNames(basePath);
            foreach (var file in filesNames.Keys)
            {
                try
                {
                    if (File.Exists(filesNames[file]) && new FileInfo(filesNames[file]).Length > 0)
                    {
                       // Console.WriteLine($"{song.id} - {song.name} already exists. Skipping...");
                        continue;
                    }
                    else if (File.Exists(filesNames[file]))
                    {
                        Console.WriteLine($"{song.id} - {song.name} already exists but has no size. Deleting...");
                        File.Delete(filesNames[file]);
                    }

                    var httpClient = new HttpClient();
                    using var response = await httpClient.GetAsync(song.versions.First(x => x.hash == file).downloadURL, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    await using var contentStream = await response.Content.ReadAsStreamAsync();
                    await using var fileStream = new FileStream(filesNames[file], FileMode.Create, FileAccess.Write, FileShare.None);

                    await contentStream.CopyToAsync(fileStream);
                    Console.WriteLine($"Downloaded: {song.id} - {song.name}");

                    if (!File.Exists(filesNames[file]))
                    {
                        Console.WriteLine($"{song.id} - {song.name} failed to download...");
                    }
                }
                catch (Exception ex)
                {
                    missingSongs.Add(song);
                    missingCount++;
                    Console.WriteLine($"Error downloading {song.id} - {song.name}: {ex.Message}");
                }
            }
        }

        var songCount = songs.SelectMany(x => x.versions).Count();
        Console.WriteLine($"Total Songs: {songCount}");
        Console.WriteLine($"{missingCount} missing");
        Console.WriteLine($"Should have {songCount - missingCount} songs");
        var songData = JsonConvert.SerializeObject(missingSongs, Formatting.Indented);
        File.WriteAllText(@"g:\BeatSaber\missingsongs.json", songData);
        Console.ReadLine();
    }
}