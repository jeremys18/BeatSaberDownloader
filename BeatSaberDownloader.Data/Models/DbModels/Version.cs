using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("Version", Schema = "BeatSaver")]
    public class Version
    {
        public int Id { get; set; }
        public string CoverURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DownloadURL { get; set; }
        public string? Feedback { get; set; }
        public string Hash { get; set; }
        public string? Key { get; set; }
        public string PreviewURL { get; set; }
        public short SageScore { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public int StateId { get; set; }
        public DateTime? TestplayAt { get; set; }
        public int SongId { get; set; }

        public virtual State State { get; set; }
        public virtual ICollection<TestPlay> TestPlays { get; set; } = new List<TestPlay>();
        public virtual ICollection<Difficulty> Difficulties { get; set; } = new List<Difficulty>();
        public virtual Song? Song { get; set; }
    }
}
