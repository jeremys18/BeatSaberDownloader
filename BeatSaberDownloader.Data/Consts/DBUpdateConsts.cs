
namespace BeatSaberDownloader.Data.Consts
{
    public static class DBUpdateConsts
    {
        public readonly static string LogFile = @"C:\BeatSaber\Logs\DBUpdateService.log"; // The log file path
        public readonly static string UpdatesFolder = @"C:\BeatSaber\Updates\"; // The folder to monitor for update files
        public readonly static string SongsFolder = @"C:\BeatSaber\SongFiles"; // The folder where the song files are
        public readonly static string DeletedSongsFolder = @"C:\BeatSaber\DeletedSongs\"; // The folder where deleted songs are moved to
        public readonly static string SongsToDownloadFolder = @"C:\BeatSaber\Songs awaiting download\"; // The folder containing files with songs to download
    }
}
