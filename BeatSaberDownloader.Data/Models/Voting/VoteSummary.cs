

namespace BeatSaberDownloader.Data.Models.Voting
{
    public class VoteSummary
    {
        public int downvotes { get; set; }
        public string hash { get; set; }
        public string key64 { get; set; }
        public int mapId { get; set; }
        public double score { get; set; }
        public int upvotes { get; set; }
    }
}
