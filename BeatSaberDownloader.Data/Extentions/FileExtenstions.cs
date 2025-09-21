

namespace BeatSaberDownloader.Data.Extentions
{
    public static class FileExtenstions
    {
        public static FileStream GetFileAccess(this string filePath, FileMode mode, FileAccess access)
        {
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    var stream = File.Open(filePath, mode , access, FileShare.None);
                    // File is ready
                    return stream;
                }
                catch (IOException)
                {
                    // File is still locked, wait and retry
                    var time = Random.Shared.Next(100, 5000);
                    Thread.Sleep(time);
                }
            }
            throw new IOException($"File {filePath} was not ready after 100 attempts.");
        }
    }
}
