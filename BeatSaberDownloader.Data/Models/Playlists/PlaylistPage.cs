
using BeatSaberDownloader.Data.Models.Map;

namespace BeatSaberDownloader.Data.Models.Playlists
{
    public class PlaylistPage
    {
        public MapDetailWithOrder[] maps { get; set; }
        public Playlist playlist { get; set; }
    }
}
