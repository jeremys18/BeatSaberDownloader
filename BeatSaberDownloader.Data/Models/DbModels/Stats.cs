
using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("Stats", Schema = "BeatSaver")]
    public class Stats
    {
        public int Id { get; set; }
        public int Downloads { get; set; }
        public int Downvotes { get; set; }
        public int Plays { get; set; }
        public int Reviews { get; set; }
        public decimal Score { get; set; }
        public decimal ScoreOneDP { get; set; }
        public int SentimentId { get; set; }
        public int Upvotes { get; set; }

        public virtual Sentiment Sentiment { get; set; }
    }
}
