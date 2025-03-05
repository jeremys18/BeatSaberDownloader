
namespace BeatSaberDownloader.Data.Models
{
    public class UserStats
    {
        public float avgBpm { get; set; }
        public float avgDuration { get; set; }
        public float avgScore { get; set; }
        public UserDiffStats diffStats { get; set; }
        public DateTime firstUpload { get; set; }
        public DateTime lastUpload { get; set; }
        public int rankedMaps { get; set; }
        public int totalDownvotes { get; set; }
        public int totalMaps { get; set; }
        public int totalUpvotes { get; set; }
    }
}
