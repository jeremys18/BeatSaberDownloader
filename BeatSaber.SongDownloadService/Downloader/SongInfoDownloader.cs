
using BeatSaber.SongDownloadService.API;
using System;

namespace BeatSaber.SongDownloadService.Downloader
{
    public static class SongInfoDownloader
    {
        public static void Start()
        {
            DownloadSongInfo();
        }

        private static void DownloadSongInfo()
        {
            var firstround = BeatSaverAPI.SearchSongs(DateTime.Parse(BeatSaverConsts.DefaultBeatSaverAfterDate));
        }

        private static void ProcessSongInfo(string songInfo)
        {
            // Logic to process the downloaded song info
            // This is a placeholder for the actual implementation
            // You would typically parse the JSON or XML response and store it in a database or file

            // save to file
            // start db updator
        }
    }
}
