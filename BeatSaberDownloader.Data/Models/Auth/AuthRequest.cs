namespace BeatSaberDownloader.Data.Models.Auth
{
    public class AuthRequest
    {
        public string gameVersion { get; set; }
        public string oculusId { get; set; }
        public string proof { get; set; }
        public string steamId { get; set; }
    }
}
