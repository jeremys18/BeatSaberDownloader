using BeatSaberDownloader.Data.Extentions;
using BeatSaberDownloader.Data.Models;
using MediatR;
using Newtonsoft.Json;

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
                var songFileText = await @"G:\BeatSaber\Songs.json".GetFileTextAsync();
                var songList = JsonConvert.DeserializeObject<MapDetail[]>(songFileText);
                var songInfo = songList.FirstOrDefault(x => x.id == query.SongId);
                var fileNames = songInfo.GetValidFileNames(@"G:BeatSaber\SongFiles");
                songFileText = null;
                songList = null;
                songInfo = null;

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
