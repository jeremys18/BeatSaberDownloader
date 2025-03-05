
using BeatSaberDownloader.Data.Enums;
using BeatSaberDownloader.Data.Interfaces;

namespace BeatSaberDownloader.Data.Models.Playlists
{
    public class Playlist
    {
        public IPlaylistConfig config { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime curatedAt { get; set; }
        public UserDetail curator { get; set; }
        public DateTime deletedAt { get; set; }
        public string description { get; set; }
        public string downloadURL { get; set; }
        public string name { get; set; }
        public UserDetail owner { get; set; }
        public int playlistId { get; set; }
        public string playlistImage { get; set; }
        public string playlistImage512 { get; set; }
        public DateTime songsChangedAt { get; set; }
        public PlaylistStats stats {get;set;}
        public PlaylistType type { get; set; }
        public DateTime updatedAt { get; set; }
    }
}
