
namespace BeatSaberDownloader.Data.Models
{
    public class MapTestplay
    {
        public DateTime createdAt { get; set; }
        public string feedback { get; set; }
        public DateTime feedbackAt { get; set; }
        public UserDetail user { get; set; }
        public string video { get; set; }
    }
}
