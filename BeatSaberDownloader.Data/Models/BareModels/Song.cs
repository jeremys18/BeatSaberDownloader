

namespace BeatSaberDownloader.Data.Models.BareModels
{
    public class Song
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        // This URl is for their server, not ours. 
        // If this URL fails we can default to our server url as a backup but first we need to know their URL
        public string BeatSaverDownloadUrl { get; set; } = string.Empty;

        public string VersionHash { get; set; } = string.Empty;

        public override string ToString()
        {
            return Name;
        }
    }
}
