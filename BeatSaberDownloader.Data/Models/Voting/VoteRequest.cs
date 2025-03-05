

using BeatSaberDownloader.Data.Models.Auth;

namespace BeatSaberDownloader.Data.Models.Voting
{
    public class VoteRequest
    {
        public AuthRequest auth { get; set; }
        public bool direction { get; set; }
        public string hash { get; set; }
    }
}
