using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("ParitySummary", Schema = "BeatSaver")]
    public class ParitySummary
    {
        public int Errors { get; set; }
        public int Resets { get; set; }
        public int Warns { get; set; }
    }
}
