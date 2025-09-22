using BeatSaberDownloader.Data.Extentions;
using BeatSaberDownloader.Data.Models;
using BeatSaberDownloader.Data.Models.BareModels;
using MediatR;
using Newtonsoft.Json;

namespace BeatSaberDownloader.Server.Services.MediaR.Queries.GetAllSongs
{
    public class GetAllSongsQueryHandler : IRequestHandler<GetAllSongsQuery, Song[]?>
    {
        public async Task<Song[]?> Handle(GetAllSongsQuery query, CancellationToken cancellationToken)
        {

            // For now we just use the JSon file for the current list. Will switch to Db later on
            var songs = await @"G:\BeatSaber\Songs.json".GetFileTextAsync();
            var songList = JsonConvert.DeserializeObject<MapDetail[]>(songs) ?? [];
            var result = new List<Song>();

            foreach(var song in songList)
            {
                var fileNames = song.GetValidFileNames(query.SongBasePath);
                foreach(var ver in song.versions)
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
            return [.. result];
        }
    }
}
