
using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("Metadata", Schema = "BeatSaver")]
    public class MetaData
    {
        public int Id { get; set; }
        public decimal BPM { get; set; }
        public int Duration { get; set; }
        public string LevelAuthorName { get; set; }
        public string? SongAuthorName { get; set; }
        public string SongName { get; set; }
        public string? SongSubName { get; set; }
    }
}
