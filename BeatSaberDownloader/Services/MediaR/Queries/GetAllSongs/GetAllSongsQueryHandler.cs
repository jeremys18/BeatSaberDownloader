using BeatSaberDownloader.Data.Extentions;
using BeatSaberDownloader.Data.Models;
using BeatSaberDownloader.Data.Models.BareModels;
using MediatR;
using Newtonsoft.Json;

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
                // For now we just use the JSon file for the current list. Will switch to Db later on
                var songs = await @"G:\BeatSaber\Songs.json".GetFileTextAsync();
                var songList = JsonConvert.DeserializeObject<MapDetail[]>(songs) ?? [];

                foreach (var song in songList)
                {
                    var fileNames = song.GetValidFileNames(query.SongBasePath);
                    foreach (var ver in song.versions)
                    {
                        result.Add(new Song
                        {
                            Id = song.id,
                            Name = song.name,
                            BeatSaverDownloadUrl = ver.downloadURL,
                            FileName = fileNames[ver.hash],
                            VersionHash = ver.hash
                        });
                    }
                }

                songList = null;
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
