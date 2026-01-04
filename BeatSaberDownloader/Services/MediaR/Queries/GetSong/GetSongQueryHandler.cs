using BeatSaberDownloader.Data.Consts;
using BeatSaberDownloader.Data.DBContext;
using BeatSaberDownloader.Data.Extentions;
using BeatSaberDownloader.Data.Models.DbModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSaberDownloader.Server.Services.MediaR.Queries.GetSong
{
    public class GetSongQueryHandler : IRequestHandler<GetSongQuery, byte[]?>
    {
        private readonly ILogger<GetSongQueryHandler> _logger;

        public GetSongQueryHandler(ILogger<GetSongQueryHandler> logger)
        {
            _logger = logger;
        }

        public async Task<byte[]?> Handle(GetSongQuery query, CancellationToken cancellationToken)
        {
            byte[]? result;
            try
            {
                Song song;
                using (var db = new BeatSaverContext())
                {
                    // We specifically want to allow getting deleted songs here for archival purposes. If a song is deleted but still in the DB, we should be able to get it.
                    song = db.Songs
                        .AsNoTracking()
                        .Include(x => x.Metadata)
                        .Include(x => x.Versions)
                        .Include(x => x.Uploader) 
                        .First(x => x.Id == query.SongId);
                }

                var fileNames = song.GetValidFileNames(BeatSaverConsts.BeatSaverSongDirectory);

                result = await File.ReadAllBytesAsync(fileNames[query.VersionHash], cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve song {SongId} version {VersionHash}", query.SongId, query.VersionHash);
                result = null;
            }

            return result;
        }
    }
}
