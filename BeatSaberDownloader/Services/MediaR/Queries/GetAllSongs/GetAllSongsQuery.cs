using BeatSaberDownloader.Data.Models.BareModels;
using MediatR;

namespace BeatSaberDownloader.Server.Services.MediaR.Queries.GetAllSongs
{
    public class GetAllSongsQuery : IRequest<Song[]?>
    {
        public string SongBasePath { get; set; } = string.Empty;
    }
}
