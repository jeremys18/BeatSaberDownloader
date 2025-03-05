
namespace BeatSaberDownloader.Data.Models.Playlists
{
    public class PlaylistStats
    {
        public float avgScore { get; set; }
        public int downVotes { get; set; }
        public long mapperCount { get; set; }
        public double maxNps { get; set; }
        public double maxNpsTwoDP { get; set; }
        public double minNps  { get;set;}
        public double minNpsTwoDP { get; set; }
        public float scoreOneDP { get; set; }
        public int totalDuration { get; set; }
        public int totalMaps { get; set; }
        public int upVotes { get; set; }
    }
}
