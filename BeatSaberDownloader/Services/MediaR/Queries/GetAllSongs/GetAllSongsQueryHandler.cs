using BeatSaberDownloader.Data.Models;
using MediatR;
using Newtonsoft.Json;

namespace BeatSaberDownloader.Server.Services.MediaR.Queries.GetAllSongs
{
    public class GetAllSongsQueryHandler : IRequestHandler<GetAllSongsQuery, MapDetail[]?>
    {
        public async Task<MapDetail[]?> Handle(GetAllSongsQuery request, CancellationToken cancellationToken)
        {

            // For now we just use the JSon file for the current list. Will switch to Db later on
            var songs = await GetFileTextAsync();
            var songList = JsonConvert.DeserializeObject<MapDetail[]>(songs);
            return songList;
        }

        private static async Task<string> GetFileTextAsync()
        {
            var keepTrying = true;
            while (keepTrying)
            {
                try
                {
                    var text = await File.ReadAllTextAsync(@"G:\BeatSaber\Songs.json");
                    keepTrying = false;
                    return text;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(1000); // Wait for the file to be availble
                }
            }
            return string.Empty;
        }
    }
}
