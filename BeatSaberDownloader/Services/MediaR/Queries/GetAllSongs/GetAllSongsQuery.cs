using BeatSaberDownloader.Data.Models;
using MediatR;

namespace BeatSaberDownloader.Server.Services.MediaR.Queries.GetAllSongs
{
    public class GetAllSongsQuery : IRequest<MapDetail[]?>
    {
        public string SongBasePath { get; set; } = string.Empty;
    }
}
