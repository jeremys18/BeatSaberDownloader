using BeatSaberDownloader.Data.Extentions;
using BeatSaberDownloader.Data.Models;
using MediatR;
using Newtonsoft.Json;

namespace BeatSaberDownloader.Server.Services.MediaR.Queries.GetSong
{
    public class GetSongQueryHandler : IRequestHandler<GetSongQuery, byte[]?>
    {
        public async Task<byte[]?> Handle(GetSongQuery query, CancellationToken cancellationToken)
        {
            byte[]? result;
            try
            {
                var songFileText = await @"G:\BeatSaber\Songs.json".GetFileTextAsync();
                var songList = JsonConvert.DeserializeObject<MapDetail[]>(songFileText);
                var songInfo = songList.FirstOrDefault(x => x.id == query.SongId);
                var fileNames = songInfo.GetValidFileNames(@"G:BeatSaber\SongFiles");

                result = await File.ReadAllBytesAsync(fileNames[query.VersionHash], cancellationToken);
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }
    }
}
