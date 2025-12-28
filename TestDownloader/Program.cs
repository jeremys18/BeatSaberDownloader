
using BeatSaberDownloader.Data.Enums;
using BeatSaberDownloader.Data.Models;
using BeatSaberDownloader.Data.Models.DbModels;
using Newtonsoft.Json;
using TestDownloader;
using Version = BeatSaberDownloader.Data.Models.DbModels.Version;

//await SongInfoDownloader.StartAsync();

internal class Program
{
    private static async Task Main(string[] args)
    {
        //await Downlaoder.StartAsync(); // Downloads song info and saves to songs.json
        //DeleteSongs();
        //await DownloadSongs();

        PopulateDB();
    }

    private static void DeleteSongs()
    {
        // This grabs the songs from the list AND the files in the folder
        // If a file exists in the folder but not in the list, it moves it to DeletedSongs
        // If a file exists in the list but not in the folder, it logs it as missing
        var basePath = @"g:\BeatSaber\SongFiles";
        var jsonFilename = @"g:\BeatSaber\songs.json";
        var currentJsonText = File.ReadAllText(jsonFilename);
        var songs = JsonConvert.DeserializeObject<MapDetail[]>(currentJsonText) ?? throw new NullReferenceException("Could not deserialize current song list...");

        var songFiles = Directory.GetFiles(basePath, "*.zip", SearchOption.TopDirectoryOnly).Select(x => Path.GetFileName(x)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var songListFiles = songs.Select(x => x.GetValidFileNames(basePath)).SelectMany(x => x.Values).Select(x => Path.GetFileName(x)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missingFiles = songListFiles.Except(songFiles).ToList();
        var missing2 = songFiles.Except(songListFiles).ToList();

        missing2.ForEach(x =>
        {
            Console.WriteLine(x);
            if (File.Exists(@$"g:\BeatSaber\DeletedSongs\{x}"))
            {
                Console.WriteLine($"Also exists in deleted songs, skipping move...");
                File.Delete(@$"g:\BeatSaber\SongFiles\{x}");
                return;
            }
            File.Move(@$"g:\BeatSaber\SongFiles\{x}", Path.Combine(@"g:\BeatSaber\DeletedSongs", x));
        });
        Console.Read();
    }

    private static async Task DownloadSongs()
    {
        try
        {
            var basePath = @"g:\BeatSaber\SongFiles";
            var jsonFilename = @"g:\BeatSaber\songs.json";
            var currentJsonText = File.ReadAllText(jsonFilename);
            var songs = JsonConvert.DeserializeObject<MapDetail[]>(currentJsonText) ?? throw new NullReferenceException("Could not deserialize current song list...");
            var missingCount = 0;
            var missingSongs = new List<MapDetail>();
            foreach (var song in songs.OrderByDescending(x => x.id))
            {
                var filesNames = song.GetValidFileNames(basePath);
                foreach (var file in filesNames.Keys)
                {
                    try
                    {
                        if (File.Exists(filesNames[file]) && new FileInfo(filesNames[file]).Length > 0)
                        {
                            //Console.WriteLine($"{song.id} - {song.name} already exists. Skipping...");
                            continue;
                        }
                        else if (File.Exists(filesNames[file]))
                        {
                            Console.WriteLine($"{song.id} - {song.name} already exists but has no size. Deleting...");
                            File.Delete(filesNames[file]);
                        }

                        var httpClient = new HttpClient();
                        using var response = await httpClient.GetAsync(song.versions.First(x => x.hash == file).downloadURL, HttpCompletionOption.ResponseContentRead);
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
            //var songData = JsonConvert.SerializeObject(missingSongs, Formatting.Indented);
            //File.WriteAllText(@"g:\BeatSaber\missingsongs.json", songData);
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.ReadLine();
        }
    }

    private static void PopulateDB()
    {
        // Use BeatSaverContext to populate the database with songs from songs.json
        // This is just a placeholder for the actual implementation
        Console.WriteLine("Populating database...");
        using var context = new BeatSaberDownloader.Data.DBContext.BeatSaverContext();
        var jsonFilename = @"C:\Users\grabb\Desktop\New Lappy\songs.json";
        var currentJsonText = File.ReadAllText(jsonFilename);
        var songs = JsonConvert.DeserializeObject<MapDetail[]>(currentJsonText) ?? throw new NullReferenceException("Could not deserialize current song list...");
        foreach (var song in songs)
        {
            var uploader = context.Users.FirstOrDefault(x => x.Id == song.uploader.id) ?? new User
            {
                Id = song.uploader.id,
                Name = song.uploader.name,
                Admin = song.uploader.admin,
                Avatar = song.uploader.avatar,
                Curator = song.uploader.curator,
                PlaylistUrl = song.uploader.playlistUrl,
                SeniorCurator = song.uploader.seniorCurator,
                UserTypeId = (int)song.uploader.type,
            };

            var tags = song.tags?.Select(t => context.Tags.FirstOrDefault(tag => tag.Name == t) ?? new BeatSaberDownloader.Data.Models.DbModels.Tag { Name = t })?.ToList() ?? new List<BeatSaberDownloader.Data.Models.DbModels.Tag>();

            // Map MapDetail to Song entity and add to context
            var songEntity = new Song
            {
                Id = song.id,
                Automapper = song.automapper,
                BlQualified = song.blQualified,
                BlRanked = song.blRanked,
                Bookmarked = song.bookmarked,
                CreatedAt = song.createdAt,
                DeclaredAiId = (int)song.declaredAi,
                Description = song.description,
                LastPublishedAt = song.lastPublishedAt,
                Name = song.name,
                Qualified = song.qualified,
                Ranked = song.ranked,
                UpdatedAt = song.updatedAt,
                Uploaded = song.uploaded,
                UploaderId = uploader.UserId,

                Metadata = new MetaData
                {
                    BPM = song.metadata.bpm,
                    Duration = song.metadata.duration,
                    SongAuthorName = song.metadata.songAuthorName,
                    LevelAuthorName = song.metadata.levelAuthorName,
                    SongName = song.metadata.songName,
                    SongSubName = song.metadata.songSubName,
                },
                Stats = new Stats
                {
                    Downloads = song.stats.downloads,
                    Plays = song.stats.plays,
                    Downvotes = song.stats.downvotes,
                    Reviews = song.stats.reviews,
                    Score = song.stats.score,
                    ScoreOneDP = song.stats.scoreOneDP,
                    Upvotes = song.stats.upvotes,
                    SentimentId = ((int)song.stats.sentiment) == 0 ? 1 : (int)song.stats.sentiment
                },
                Uploader = uploader,
                Tags = tags,
                Versions = song.versions.Select(v => new Version
                {
                    Hash = v.hash,
                    DownloadURL = v.downloadURL,
                    CreatedAt = v.createdAt,
                    StateId = (int)v.state,
                    CoverURL = v.coverURL,
                    Feedback = v.feedback,
                    Key = v.key,
                    PreviewURL = v.previewURL,
                    SageScore = v.sageScore,
                    ScheduledAt = v.scheduledAt,
                    TestplayAt = v.testplayAt,
                    Difficulties = v.diffs.Select(d => new BeatSaberDownloader.Data.Models.DbModels.Difficulty
                    {
                        CharacteristicId = (int)Enum.Parse<BeatSaberDownloader.Data.Enums.Characteristic>(d.characteristic),
                        NJS = d.njs,
                        Notes = d.notes,
                        Obstacles = d.obstacles,
                        Bombs = d.bombs,
                        Stars = d.stars,
                        BlStars = d.blStars,
                        Length = d.length,
                        NPS = d.nps,
                        MaxScore = d.maxScore,
                        Label = d.label,
                        Difficulty2Id = (int)d.difficulty,
                        EnvironmentId = (int)d.environment,
                        Chroma = d.chroma,
                        Cinema = d.cinema,
                        ME = d.me,
                        NE = d.ne,
                        Vivify = d.vivify,
                        Events = d.events,
                        Offset = d.offset,
                        Seconds = d.seconds,
                        ParitySummary = new ParitySummary
                        {
                            Errors = d.paritySummary.errors,
                            Resets = d.paritySummary.resets,
                            Warns = d.paritySummary.warns
                        }

                    }).ToList(),
                    TestPlays = v.testplays?.Select(tp => new TestPlay
                    {
                        CreatedAt = tp.createdAt,
                        Feedback = tp.feedback,
                        FeedbackAt = tp.feedbackAt,
                        User = context.Users.FirstOrDefault(u => u.Id == tp.user.id) ?? new User
                        {
                            Id = tp.user.id,
                            Name = tp.user.name,
                            Admin = tp.user.admin,
                            Avatar = tp.user.avatar,
                            Curator = tp.user.curator,
                            PlaylistUrl = tp.user.playlistUrl,
                            SeniorCurator = tp.user.seniorCurator,
                            UserTypeId = (int)tp.user.type
                        },
                        Video = tp.video,
                    }).ToList() ?? new List<TestPlay>()
                }).ToList()
            };

            context.Songs.Add(songEntity);
            context.SaveChanges();
        }
        
        Console.WriteLine("Database populated.");
    }
}