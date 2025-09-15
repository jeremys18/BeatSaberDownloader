
using BeatSaberDownloader.Data.Models;
using Newtonsoft.Json;

//await SongInfoDownloader.StartAsync();

internal class Program
{
    private static void Main(string[] args)
    {
        var basePath = @"c:\BeatSaber\SongFiles";
        var jsonFilename = @"c:\BeatSaber\songs.json";
        var currentJsonText = File.ReadAllText(jsonFilename);
        var songs = JsonConvert.DeserializeObject<MapDetail[]>(currentJsonText) ?? throw new NullReferenceException("Could not deserialize current song list...");

        foreach (var song in songs)
        {
            var filesNames = song.GetValidFileNames(basePath);
            Console.WriteLine($"{song.id} - {song.name} - {song.metadata.songAuthorName} - {song.uploader.name}");
        }
    }
}