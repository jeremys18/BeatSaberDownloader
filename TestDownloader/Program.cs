using BeatSaberDownloader.Data.Consts;
using BeatSaberDownloader.Data.DBContext;
using BeatSaberDownloader.Data.Models;
using BeatSaberDownloader.Data.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TestDownloader;

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
        // This is messy and a lot of code but its also the fastest way to insert the data.
        // The normal way in EF takes 4+ HOURS. This messy code takes 3 mins or less. So messy code it is!

        Console.WriteLine("Populating database...");
        using var context = new BeatSaverContext();
        var jsonFilename = Path.Combine(BeatSaverConsts.BeatSaverDataDirectory, "songs.json");
        var currentJsonText = File.ReadAllText(jsonFilename);
        var songs = JsonConvert.DeserializeObject<MapDetail[]>(currentJsonText) ?? throw new NullReferenceException("Could not deserialize current song list...");
        var uploaders = songs.Select(s => s.uploader).DistinctBy(u => u.id).Select(x => new User
        {
            ExternalId = x.id,
            Name = x.name,
            Admin = x.admin,
            Avatar = x.avatar,
            Curator = x.curator,
            PlaylistUrl = x.playlistUrl,
            SeniorCurator = x.seniorCurator,
            UserTypeId = (int)x.type,
        }).ToList();
        var newTags = songs.SelectMany(s => s?.tags ?? []).Distinct().Select(t => new BeatSaberDownloader.Data.Models.DbModels.Tag { Name = t }).ToList();
        var md = songs.Select(x => new MetaData
        {
            BPM = x.metadata.bpm,
            Duration = x.metadata.duration,
            SongAuthorName = x.metadata.songAuthorName,
            LevelAuthorName = x.metadata.levelAuthorName,
            SongName = x.metadata.songName,
            SongSubName = x.metadata.songSubName == string.Empty ? null : x.metadata.songSubName,
        }).ToList();
        var stats = songs.Select(x => new Stats
        {
            Downloads = x.stats.downloads,
            Plays = x.stats.plays,
            Downvotes = x.stats.downvotes,
            Upvotes = x.stats.upvotes,
            Reviews = x.stats.reviews,
            Score = x.stats.score,
            ScoreOneDP = x.stats.scoreOneDP,
            SentimentId = ((int)x.stats.sentiment) == 0 ? 1 : (int)x.stats.sentiment
        }).ToList();

        context.Users.AddRange(uploaders);
        context.Tags.AddRange(newTags);
        context.MetaDatas.AddRange(md);
        context.Stats.AddRange(stats);
        context.SaveChanges();

        var dbSongs = new List<Song>();
        for (int i = 0; i < songs.Count(); i++)
        {
            var song = songs[i];
            dbSongs.Add(new Song
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
                MetadataId = md[i].Id,
                StatsId = stats[i].Id,
                UploaderId = uploaders.First(x => x.ExternalId == song.uploader.id).Id
            });
        }

        context.Songs.AddRange(dbSongs);
        context.SaveChanges();

        // build lookups for faster mapping
        var tagLookup = newTags.ToDictionary(t => t.Name, t => t.Id);
        var songLookup = dbSongs.ToDictionary(s => s.Id, s => s.SongId);

        var songTags = songs
            .Where(s => s.tags != null)
            .SelectMany(s => s.tags, (s, tag) => new SongTag
            {
                SongId = songLookup[s.id],
                TagId = tagLookup[tag]
            })
            .ToList();



        context.SongTags.AddRange(songTags);
        context.SaveChanges();

        var versions = songs.SelectMany((song, index) => song.versions.Select(v => new BeatSaberDownloader.Data.Models.DbModels.Version
        {
            SongId = dbSongs[index].SongId,
            Hash = v.hash,
            DownloadURL = v.downloadURL,
            CreatedAt = v.createdAt,
            StateId = (int)v.state,
            CoverURL = v.coverURL,
            Feedback = v.feedback,
            Key = v.key,
            PreviewURL = v.previewURL,
            SageScore = v.sageScore,
            ScheduledAt = v.scheduledAt.Year < 2015 ? null : v.scheduledAt,
            TestplayAt = v.testplayAt.Year < 2015 ? null : v.testplayAt,
        })).ToList();
        context.Versions.AddRange(versions);
        context.SaveChanges();

        // Create ParitySummary entries with explicit keys so we can reliably link them to Difficulties
        var parityEntries = new List<(int SongIndex, int VersionIndex, int DiffIndex, ParitySummary Entry)>();
        for (int si = 0; si < songs.Length; si++)
        {
            var song = songs[si];
            for (int vi = 0; vi < song.versions.Length; vi++)
            {
                var v = song.versions[vi];
                for (int di = 0; di < v.diffs.Length; di++)
                {
                    var d = v.diffs[di];
                    var ps = new ParitySummary
                    {
                        Errors = d.paritySummary?.errors ?? 0,
                        Resets = d.paritySummary?.resets ?? 0,
                        Warns = d.paritySummary?.warns ?? 0
                    };
                    parityEntries.Add((si, vi, di, ps));
                }
            }
        }

        context.ParitySummaries.AddRange(parityEntries.Select(p => p.Entry));
        context.SaveChanges();

        // build a lookup from song/version/diff indices to saved ParitySummary Id
        var parityLookup = parityEntries.ToDictionary(p => (p.SongIndex, p.VersionIndex, p.DiffIndex), p => p.Entry.Id);

        // Create Difficulty list from songs -> versions -> diffs, link ParitySummaryId and VersionId, then save
        var difficulties = new List<Difficulty>();
        var testPlays = new List<TestPlay>();
        var versionGlobalIndex = 0;

        for (int s = 0; s < songs.Length; s++)
        {
            var song = songs[s];
            for (int vIndex = 0; vIndex < song.versions.Length; vIndex++)
            {
                var v = song.versions[vIndex];
                // get corresponding saved Version entity
                var versionEntity = versions[versionGlobalIndex];

                // map testplays for this version (if any)
                if (v.testplays != null)
                {
                    foreach (var tp in v.testplays)
                    {
                        // find uploader/user id for the testplay user
                        var user = uploaders.FirstOrDefault(u => u.ExternalId == tp.user.id);
                        if (user == null)
                            continue; // skip if we don't have the user saved

                        testPlays.Add(new TestPlay
                        {
                            CreatedAt = tp.createdAt,
                            Feedback = tp.feedback,
                            FeedbackAt = tp.feedbackAt,
                            UserId = user.Id,
                            Video = tp.video,
                            VersionId = versionEntity.Id
                        });
                    }
                }

                for (int dIndex = 0; dIndex < v.diffs.Length; dIndex++)
                {
                    var d = v.diffs[dIndex];

                    var characteristicId = char.IsDigit(d.characteristic[0])
                        ? (int)Enum.Parse<BeatSaberDownloader.Data.Enums.Characteristic>($"_{d.characteristic}")
                        : (int)Enum.Parse<BeatSaberDownloader.Data.Enums.Characteristic>(d.characteristic);

                    // lookup parity summary id by indices
                    parityLookup.TryGetValue((s, vIndex, dIndex), out var parityId);

                    difficulties.Add(new Difficulty
                    {
                        BlStars = d.blStars,
                        Bombs = d.bombs,
                        Chroma = d.chroma,
                        Cinema = d.cinema,
                        CharacteristicId = characteristicId,
                        Difficulty2Id = (int)d.difficulty,
                        EnvironmentId = (int)d.environment,
                        Events = d.events,
                        Label = d.label,
                        Length = d.length,
                        MaxScore = d.maxScore,
                        ME = d.me,
                        NE = d.ne,
                        NJS = d.njs,
                        Notes = d.notes,
                        NPS = d.nps,
                        Obstacles = d.obstacles,
                        Offset = d.offset,
                        ParitySummaryId = parityId,
                        Seconds = d.seconds,
                        Stars = d.stars,
                        Vivify = d.vivify,
                        VersionId = versionEntity.Id
                    });
                }

                versionGlobalIndex++;
            }
        }

        context.Difficulties.AddRange(difficulties);
        context.TestPlays.AddRange(testPlays);
        context.SaveChanges();

        Console.WriteLine("Database populated.");
    }
}