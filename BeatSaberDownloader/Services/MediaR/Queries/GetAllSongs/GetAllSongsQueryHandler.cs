using BeatSaberDownloader.Data.DBContext;
using BeatSaberDownloader.Data.Extentions;
using BeatSaberDownloader.Data.Models.BareModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSaberDownloader.Server.Services.MediaR.Queries.GetAllSongs
{
    public class GetAllSongsQueryHandler : IRequestHandler<GetAllSongsQuery, Song[]?>
    {
        private readonly ILogger<GetAllSongsQueryHandler> _logger;

        public GetAllSongsQueryHandler(ILogger<GetAllSongsQueryHandler> logger)
        {
            _logger = logger;
        }

        public async Task<Song[]?> Handle(GetAllSongsQuery query, CancellationToken cancellationToken)
        {
            var result = new List<Song>();
            try
            {
                // Yay! We finally using the DB. This should speed things up
                var songs = new List<Data.Models.DbModels.Song>();
                using(var db = new BeatSaverContext())
                {
                    songs = await db.Songs
                        .Include(x => x.Metadata)
                        .Include(x => x.Versions)
                        .Include(x => x.Uploader)
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);
                }

                foreach (var song in songs)
                {
                    var fileNames = song.GetValidFileNames(query.SongBasePath);
                    foreach (var ver in song.Versions)
                    {
                        result.Add(new Song
                        {
                            Id = song.Id,
                            Name = song.Name,
                            BeatSaverDownloadUrl = ver.DownloadURL,
                            FileName = fileNames[ver.Hash],
                            VersionHash = ver.Hash
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Oh no! Couldn't get all songs....");
                return null;
            }
            return [.. result];
        }
    }
}
