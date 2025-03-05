
using BeatSaberDownloader.Data.Enums;

namespace BeatSaberDownloader.Data.Models
{
    public class UserDetail
    {
        public int id { get; set; }
        public bool admin { get; set; }
        public string avatar { get; set; }
        public bool blurnsfw { get; set; }
        public bool curator { get; set; }
        public bool curatorTab { get; set; }
        public string description { get; set; }
        public string email { get; set; }
        public UserFollowData followData { get; set; }
        public string hash { get; set; }
        public string name { get; set; }
        public Patreon patreon { get; set; }
        public string playlistUrl { get; set; }
        public bool seniorCurator { get; set; }
        public UserStats stats { get; set; }
        public DateTime suspendedAt { get; set; }
        public bool testplay { get; set; }
        public UserType type { get; set; }
        public bool uniqueSet { get; set; }
        public int uploadLimit { get; set; }
        public bool verifiedMapper { get; set; }
        public int vivifyLimit { get; set; }
    }
}
