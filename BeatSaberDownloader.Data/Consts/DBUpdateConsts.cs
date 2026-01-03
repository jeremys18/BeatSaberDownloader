
namespace BeatSaberDownloader.Data.Consts
{
    public static class DBUpdateConsts
    {
        public readonly static string LogFile = @"g:\BeatSaber\Logs\DBUpdateService.log"; // The log file path
        public readonly static string UpdatesFolder = @"g:\BeatSaber\Updates\"; // The folder to monitor for update files
        public readonly static string SongsFolder = @"g:\BeatSaber\SongFiles"; // The folder where the song files are
        public readonly static string DeletedSongsFolder = @"g:\BeatSaber\DeletedSongs\"; // The folder where deleted songs are moved to
        public readonly static string SongsToDownloadFolder = @"g:\BeatSaber\Songs awaiting download\"; // The folder containing files with songs to download
    }
}
