
using BeatSaberDownloader.Data.Models.Search;

namespace BeatSaberDownloader.Data.Models.Playlists
{
    public class PlaylistSearchResponse
    {
        public Playlist[] docs { get; set; }
        public SearchInfo info { get; set; }
    }
}
