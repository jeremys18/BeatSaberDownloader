
using BeatSaberDownloader.Data.Enums;

namespace BeatSaberDownloader.Data.Models
{
    public class MapStats
    {
        public int downloads { get; set; }
        public int downvotes { get; set; }
        public int plays { get; set; }
        public int reviews { get; set; }
        public decimal score { get; set; }
        public decimal scoreOneDP { get; set; }
        public Sentiment sentiment { get; set; }
        public int upvotes { get; set; }
    }
}
