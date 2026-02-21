
namespace BeatSaberDownloader.Data.Models.DbModels
{
    public class BannedIP
    {
        public int Id { get; set; }
        public string IP { get; set; } = string.Empty;
        public DateTime Updated { get; set; } = DateTime.UtcNow;
    }
}
