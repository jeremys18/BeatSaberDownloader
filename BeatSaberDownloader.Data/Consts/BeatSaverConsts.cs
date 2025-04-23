namespace BeatSaberDownloader.Data.Consts
{
    public class BeatSaverConsts
    {
        public const string BeatSaverAPIBaseURL = "https://api.beatsaver.com";

        // Maps
        public const string BeatSaverMapSearchURL = "maps/latest?after={after}&automapper=true&pageSize={pagesize}"; // We want this with the time of the last map in the list of the current page. If first page then make it the date beat saber was released
        public const int BeatSaverMapRequestPageSize = 100; // Current max
        public const string DefaultBeatSaverAfterDate = "2018-05-01"; // May 1st 2018. Beat Saber

        // Playlists
        public const string BeatSaverPlaylistsSearchURL = "playlists/latest?after={after}&automapper=true&pageSize={pagesize}";
    }
}
