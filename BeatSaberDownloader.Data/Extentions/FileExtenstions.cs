


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

        public static async Task<string> GetFileTextAsync(this string filePath)
        {
            var result = string.Empty;
            using(var stream = GetFileAccess(filePath, FileMode.Open, FileAccess.Read))
            using(var reader = new StreamReader(stream))
            {
                result = await reader.ReadToEndAsync();
            }
            return result;
        }

        public static async Task WriteContentToFile(this string filePath, string content)
        {
            using (var stream = GetFileAccess(filePath, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(content);
            }
        }
    }
}
