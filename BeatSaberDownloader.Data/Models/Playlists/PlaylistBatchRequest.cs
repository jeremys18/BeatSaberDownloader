
namespace BeatSaberDownloader.Data.Models.Playlists
{
    public class PlaylistBatchRequest
    {
        public string[] hashes { get; set; }
        public bool ignoreUnknown { get; set; }
        public bool inPlaylist { get; set; }
        public string[] keys { get; set; }
    }
}
