
using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("TestPlay", Schema = "BeatSaver")]
    public class TestPlay
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Feedback { get; set; }
        public DateTime FeedbackAt { get; set; }
        public int UserId { get; set; }
        public string Video { get; set; }
        public int VersionId { get; set; }

        public virtual User User { get; set; }
    }
}
