using BeatSaberDownloader.Data.Consts;
using BeatSaberDownloader.Data.DBContext;
using BeatSaberDownloader.Data.Extentions;
using BeatSaberDownloader.Data.Models;
using BeatSaberDownloader.Data.Models.DbModels;
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

            _watcher = new FileSystemWatcher(DBUpdateConsts.UpdatesFolder)
            {
                NotifyFilter = NotifyFilters.FileName,
                EnableRaisingEvents = true,
                IncludeSubdirectories = false
            };

            _watcher.Created += (sender, e) => ProcessUpdateAsync(sender, e);
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

        public void ProcessUpdateAsync(object sender, FileSystemEventArgs e)
        {
            try
            {
                _logger.LogInformation($"New file detected. Processing {e.Name}....");

                var updateInfo = JsonConvert.DeserializeObject<UpdateInfo>(File.ReadAllText(e.FullPath)) ?? throw new NullReferenceException("Could not decerialize the update file...");

                using (var db = new BeatSaverContext())
                {
                    UpsertdSong(updateInfo, db);
                    db.SaveChanges();
                }

                // Delete the file after processing
                _logger.LogInformation($"\tFinished processing update. Deleting file {e.Name}...");
                File.Delete(e.FullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing update file {filename}: {Message}", e.Name, ex.Message);
            }
        }

        private void UpsertdSong(UpdateInfo info, BeatSaverContext db)
        {
            if (info.msg is string id)
            {   
                DeleteSong(id, db);
            }
            else
            {
                UpsertSong(info, db);
            }
        }

        private void DeleteSong(string id, BeatSaverContext db)
        {
            var song = db.Songs.FirstOrDefault(x => x.Id == id);
            if (song == null)
            {
                _logger.LogWarning($"\tSong with id {id} not found in the DB. Cannot delete non-existent song...");
                return;
            }

            _logger.LogInformation("\tMarking the song and all versions as deleted in the DB....");
            // Mark song as deleted in DB
            var now = DateTime.UtcNow;
            song.DeletedAt = now;
            foreach (var item in song.Versions)
            {
                item.DeletedAt = now;
            }

            _logger.LogInformation("\tMoving the song files to the deleted folder");

            Directory.GetFiles(DBUpdateConsts.SongsFolder, $"{song.Id}*").ToList().ForEach(f =>
            {
                var filename = Path.GetFileName(f);
                if (File.Exists($"{DBUpdateConsts.DeletedSongsFolder}{filename}"))
                {
                    _logger.LogWarning($"\tFile {filename} has already been deleted. Deleting song file instead...");
                    File.Delete(f);
                    return;
                }
                File.Move(f, $"{DBUpdateConsts.DeletedSongsFolder}{filename}");
            });
        }

        private void DeleteSongFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"\tThe file {filePath} does not exist. Cannot move to deleted folder.");
            }
            else if (File.Exists($"{DBUpdateConsts.DeletedSongsFolder}{filePath.Split("\\").Last()}"))
            {
                _logger.LogWarning($"\tThe file {filePath} is already in the deleted folder. Deleting file from song folder...");
                File.Delete(filePath);
            }
            else
            {
                File.Move(filePath, $"{DBUpdateConsts.DeletedSongsFolder}{filePath.Split("\\").Last()}");
            }
        }

        public void UpsertSong(UpdateInfo info, BeatSaverContext db)
        {
            var mapInfo = JsonConvert.DeserializeObject<MapDetail>(info.msg.ToString()) ?? throw new ArgumentNullException(nameof(info), "\tError deserializing the update info.");
            var song = db.Songs.FirstOrDefault(x => x.Id == mapInfo.id) ?? new Song
            {
                Id = mapInfo.id,
                Name = mapInfo.name ?? string.Empty,
                Uploaded = mapInfo.uploaded,
                UpdatedAt = mapInfo.updatedAt,
                CreatedAt = mapInfo.createdAt,
                LastPublishedAt = mapInfo.lastPublishedAt,
                Automapper = mapInfo.automapper,
                BlQualified = mapInfo.blQualified,
                BlRanked = mapInfo.blRanked,
                Bookmarked = mapInfo.bookmarked,
                DeclaredAiId = (int)mapInfo.declaredAi,
                Qualified = mapInfo.qualified,
                Ranked = mapInfo.ranked,
                Description = mapInfo.description ?? string.Empty,
                Uploader = GetUserByUserDetail(mapInfo.uploader, db)
            };

            // Process the update for the json file
            _logger.LogInformation("\tUpdating the song info in the DB....");

            //Upsert song. This only needs done if the song exists.
            if (song.SongId != 0)
            {
                UpdateSong(mapInfo, song);
            }

            //Upsert MetaData
            UpdateMetadata(mapInfo.metadata, song);

            //Upsert Stats
            UpdateStats(mapInfo.stats, song);

            //Upsert Tags
            UpdateTags(mapInfo.tags, song);

            //Upsert Versions
            UpdateVersions(mapInfo, song, db);
        }

        private void UpdateSong(MapDetail detail, Song song)
        {
            song.Automapper = detail.automapper;
            song.BlQualified = detail.blQualified;
            song.BlRanked = detail.blRanked;
            song.Bookmarked = detail.bookmarked;
            song.CreatedAt = detail.createdAt;
            song.DeclaredAiId = (int)detail.declaredAi;
            song.Description = detail.description ?? string.Empty;
            song.LastPublishedAt = detail.lastPublishedAt;
            song.Name = detail.name ?? string.Empty;
            song.Qualified = detail.qualified;
            song.Ranked = detail.ranked;
            song.Uploaded = detail.uploaded;
            song.UpdatedAt = detail.updatedAt;
        }

        private void UpdateMetadata(MapDetailMetadata detail, Song song)
        {
            song.Metadata ??= new MetaData();
            song.Metadata.BPM = detail.bpm;
            song.Metadata.Duration = detail.duration;
            song.Metadata.LevelAuthorName = detail.levelAuthorName ?? string.Empty;
            song.Metadata.SongAuthorName = detail.songAuthorName;
            song.Metadata.SongName = detail.songName ?? string.Empty;
            song.Metadata.SongSubName = detail.songSubName;
        }

        private void UpdateStats(MapStats detail, Song song)
        {
            song.Stats ??= new Stats();
            song.Stats.Downvotes = detail.downvotes;
            song.Stats.Downloads = detail.downloads;
            song.Stats.Plays = detail.plays;
            song.Stats.Reviews = detail.reviews;
            song.Stats.Score = detail.score;
            song.Stats.ScoreOneDP = detail.scoreOneDP;
            song.Stats.SentimentId = (int)detail.sentiment;
            song.Stats.Upvotes = detail.upvotes;
        }

        private void UpdateTags(string[] tags, Song song)
        {
            var existingTagNames = song.Tags.Select(t => t.Name).ToList();
            var newTags = tags.Except(existingTagNames).Select(t => new Tag { Name = t }).ToList();
            var deletedTags = existingTagNames.Except(tags).ToList();

            ((List<Tag>)song.Tags).AddRange(newTags);
            ((List<Tag>)song.Tags).RemoveAll(t => deletedTags.Contains(t.Name));
        }

        private void UpdateVersions(MapDetail mapInfo, Song song, BeatSaverContext db)
        {
            var currFiles = song.GetValidFileNames(basePath);
            var newFiles = mapInfo.GetValidFileNames(basePath);
            var deletedVersions = song.Versions.ExceptBy(mapInfo.versions.Select(v => v.hash), x => x.Hash).ToList();
            var songsToDownload = new List<DownloadInfo>();
            var nameAuthorUploaderChanged = song.Name != mapInfo.name || mapInfo.metadata.songAuthorName != song.Metadata?.SongAuthorName || mapInfo.uploader.name != song.Uploader?.Name;

            foreach (var version in mapInfo.versions) // These are updated or new versions
            {
                var existingVersion = song.Versions.FirstOrDefault(v => v.Hash == version.hash) ?? new Data.Models.DbModels.Version { Hash = version.hash };
                var urlChanged = existingVersion.Id != 0 && version.downloadURL != existingVersion.DownloadURL && !File.Exists(currFiles[version.hash]);

                existingVersion.CoverURL = version.coverURL;
                existingVersion.CreatedAt = version.createdAt;
                existingVersion.DownloadURL = version.downloadURL;
                existingVersion.Feedback = version.feedback;
                existingVersion.Key = version.key;
                existingVersion.PreviewURL = version.previewURL;
                existingVersion.SageScore = version.sageScore;
                existingVersion.ScheduledAt = version.scheduledAt;
                existingVersion.StateId = (int)version.state;
                existingVersion.TestplayAt = version.testplayAt;

                // Version done. Now process testplays
                UpdateTestPlays(version.testplays ?? [], existingVersion, db);

                // Now process Difficulties
                UpdateDifficulties(version.diffs, existingVersion, db);

                // At this point everything should be updated for the db so process what needs to be done for files
                if (existingVersion.Id == 0)
                {
                    _logger.LogInformation($"\tThe update indicates the version with hash {version.hash} has been added. Adding to DB and marking for download...");
                    song.Versions.Add(existingVersion);
                    songsToDownload.Add(new DownloadInfo
                    {
                        Id = song.Id,
                        Hash = version.hash.Substring(version.hash.Length - 5),
                        Filename = newFiles[version.hash],
                        DownloadURL = version.downloadURL
                    });
                }
                else if (urlChanged)
                {
                    _logger.LogInformation($"\tThe update indicates the version with hash {version.hash} has changed and must be re-downloaded. Marking for re-download...");
                    songsToDownload.Add(new DownloadInfo
                    {
                        Id = song.Id,
                        Hash = version.hash.Substring(version.hash.Length - 5),
                        Filename = newFiles[version.hash],
                        DownloadURL = version.downloadURL
                    });
                }
                else if (nameAuthorUploaderChanged)
                {
                    _logger.LogInformation($"\tThe update indicates the version with hash {version.hash} has a filename change. Renaming the file...");
                    var oldFileName = currFiles[version.hash];
                    var newFileName = newFiles[version.hash];
                    if (!File.Exists(oldFileName))
                    {
                        _logger.LogWarning($"\tThe file {oldFileName} does not exist. Cannot rename to {newFileName}.");
                    }
                    else
                    {
                        File.Move(oldFileName, newFileName);
                    }
                }
            }

            // Delete removed versions
            foreach (var del in deletedVersions)
            {
                _logger.LogInformation($"\tThe update indicates the version with hash {del} has been deleted. Marking DB entry as deleted and moving the file...");
                del.DeletedAt = DateTime.UtcNow;

                DeleteSongFile(currFiles[del.Hash]);
            }

            // Finally, write out the download info for new or updated versions
            foreach (var s in songsToDownload)
            {
                var text = JsonConvert.SerializeObject(s, Formatting.Indented);
                File.WriteAllText($"{DBUpdateConsts.SongsToDownloadFolder}{s.Id}-{s.Hash}.json", text);
            }
        }

        private void UpdateTestPlays(MapTestplay[] testplays, Data.Models.DbModels.Version version, BeatSaverContext db)
        {
            foreach (var tp in testplays)
            {
                var existingTP = version.TestPlays.FirstOrDefault(t => t.CreatedAt == tp.createdAt) ?? new TestPlay { CreatedAt = tp.createdAt };

                existingTP.Feedback = tp.feedback;
                existingTP.FeedbackAt = tp.feedbackAt;
                existingTP.Video = tp.video;
                existingTP.User = GetUserByUserDetail(tp.user, db);

                if (existingTP.Id == 0)
                {
                    version.TestPlays.Add(existingTP);
                }
            }
        }

        private void UpdateDifficulties(MapDifficulty[] difficulties, Data.Models.DbModels.Version version, BeatSaverContext db)
        {
            var deletedDiffs = version.Difficulties.ExceptBy(difficulties.Select(d => ((int)d.difficulty, (int)d.environment)), x => (x.Difficulty2Id, x.EnvironmentId)).ToList();
            foreach (var diff in difficulties)
            {
                var existingDiff = version.Difficulties.FirstOrDefault(d => d.Difficulty2Id == (int)diff.difficulty && d.EnvironmentId == (int)diff.environment) ??
                    new Difficulty
                    {
                        Difficulty2Id = (int)diff.difficulty,
                        EnvironmentId = (int)diff.environment
                    };

                existingDiff.BlStars = diff.blStars;
                existingDiff.Bombs = diff.bombs;
                existingDiff.CharacteristicId = char.IsDigit(diff.characteristic[0])
                   ? (int)Enum.Parse<Data.Enums.Characteristic>($"_{diff.characteristic}")
                   : (int)Enum.Parse<Data.Enums.Characteristic>(diff.characteristic);
                existingDiff.Chroma = diff.chroma;
                existingDiff.Cinema = diff.cinema;
                existingDiff.Events = diff.events;
                existingDiff.Label = diff.label;
                existingDiff.Length = diff.length;
                existingDiff.MaxScore = diff.maxScore;
                existingDiff.ME = diff.me;
                existingDiff.NE = diff.ne;
                existingDiff.NJS = diff.njs;
                existingDiff.Notes = diff.notes;
                existingDiff.NPS = diff.nps;
                existingDiff.Offset = diff.offset;
                existingDiff.Obstacles = diff.obstacles;
                existingDiff.Seconds = diff.seconds;
                existingDiff.Stars = diff.stars;
                existingDiff.Vivify = diff.vivify;

                UpdateParitySummary(diff.paritySummary, existingDiff);

                if (existingDiff.Id == 0)
                {
                    version.Difficulties.Add(existingDiff);
                }
            }

            // Delete removed difficulties
            ((List<Difficulty>)version.Difficulties).RemoveAll(d => deletedDiffs.Contains(d));
        }

        private void UpdateParitySummary(MapParitySummary summary, Difficulty difficulty)
        {
            difficulty.ParitySummary ??= new ParitySummary();
            difficulty.ParitySummary.Errors = summary.errors;
            difficulty.ParitySummary.Resets = summary.resets;
            difficulty.ParitySummary.Warns = summary.warns;
        }

        private User GetUserByUserDetail(UserDetail userDetail, BeatSaverContext db)
        {
            return db.Users.FirstOrDefault(x => x.ExternalId == userDetail.id) ?? new User
            {
                Name = userDetail.name,
                Admin = userDetail.admin,
                Curator = userDetail.curator,
                SeniorCurator = userDetail.seniorCurator,
                PlaylistUrl = userDetail.playlistUrl,
                Avatar = userDetail.avatar,
                ExternalId = userDetail.id,
                UserTypeId = (int)userDetail.type
            };
        }
    }
}
