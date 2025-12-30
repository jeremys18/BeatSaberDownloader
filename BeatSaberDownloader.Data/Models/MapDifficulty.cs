
using BeatSaberDownloader.Data.Enums;

namespace BeatSaberDownloader.Data.Models
{
    public class MapDifficulty
    {
        public decimal blStars { get; set; }
        public int bombs { get; set; }
        //public Characteristic characteristic { get; set; } change back to enum once we know all the current values and how to prase them as the docs dont match the real values sent back to us. This sucks
        public string characteristic { get; set; } // Temporary workaround for the issue mentioned above
        public bool chroma { get; set; }
        public bool cinema { get; set; }
        public Difficulty difficulty { get; set; }
        public Enums.Environment environment {get;set;}
        public int events { get; set; }
        public string label { get; set; }
        public decimal length { get; set; }
        public int maxScore { get; set; }
        public bool me { get; set; }
        public bool ne { get; set; }
        public float njs { get; set; }
        public int notes { get; set; }
        public decimal nps { get; set; }
        public int obstacles { get; set; }
        public float offset { get; set; }
        public MapParitySummary paritySummary { get; set; }
        public decimal seconds { get; set; }
        public decimal stars { get; set; }
        public bool vivify { get; set; }
    }
}
