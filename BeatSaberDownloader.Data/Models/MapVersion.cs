

using BeatSaberDownloader.Data.Enums;

namespace BeatSaberDownloader.Data.Models
{
    public class MapVersion
    {
        public string coverURL { get; set; }
        public DateTime createdAt { get; set; }
        public MapDifficulty[] diffs { get; set; }
        public string downloadURL { get; set; }
        public string feedback { get; set; }
        public string hash { get; set; }
        public string key { get; set; }
        public string previewURL { get; set; }
        public short sageScore { get; set; }
        public DateTime? scheduledAt { get; set; }
        public State state { get; set; }
        public DateTime? testplayAt { get; set; }
        public MapTestplay[] testplays { get; set; }
    }
}
