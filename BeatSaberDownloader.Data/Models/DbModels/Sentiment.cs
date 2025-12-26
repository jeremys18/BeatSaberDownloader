

using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("Sentiment", Schema = "BeatSaver")]
    public class Sentiment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
