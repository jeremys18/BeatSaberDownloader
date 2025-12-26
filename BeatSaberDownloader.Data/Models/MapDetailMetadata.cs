

using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models
{
    [Table("MetaData", Schema = "BeatSaver")]
    public class MapDetailMetadata
    {
        public float bpm { get; set; }
        public int duration { get; set; }
        public string levelAuthorName { get; set; }
        public string songAuthorName { get; set; }
        public string songName { get; set; }
        public string songSubName { get; set; }
    }
}
