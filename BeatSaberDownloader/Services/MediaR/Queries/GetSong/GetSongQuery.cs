using MediatR;

namespace BeatSaberDownloader.Server.Services.MediaR.Queries.GetSong
{
    public class GetSongQuery : IRequest<byte[]?>
    {
        public string SongId { get; set; } = string.Empty;
        public string VersionHash { get; set; } = string.Empty;
    }
}
