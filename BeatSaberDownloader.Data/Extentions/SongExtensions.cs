
using BeatSaberDownloader.Data.Models.DbModels;

namespace BeatSaberDownloader.Data.Extentions
{
    public static class SongExtensions
    {
        public static Dictionary<string, string> GetValidFileNames(this Song map, string basePath)
        {
            var result = new Dictionary<string, string>();
            foreach (var ver in map.Versions)
            {
                var version = ver.Hash.Substring(ver.Hash.Length - 5);
                var fileName = $"{map.Id} - ({map.Name} [{version}] - {map.Metadata.SongAuthorName} [{map.Uploader.Name}]).zip";
                var filePath = $@"{basePath}\{ReplaceInvalidChars(fileName)}";

                if (filePath.Length > 260)
                {
                    fileName = $"{map.Id} [{version}] - {map.Name}.zip";
                    filePath = $@"{basePath}\{ReplaceInvalidChars(fileName)}";

                    if ($@"{basePath}\{fileName}".Length > 260)
                    {
                        fileName = $"{map.Id} [{version}] - Song name too long.zip";
                        filePath = $@"{basePath}\{ReplaceInvalidChars(fileName)}";
                    }
                }
                result.Add(ver.Hash, filePath);
            }
            return result;
        }

        private static string ReplaceInvalidChars(string filename)
        {
            return string.Join(" ", filename.Split(Path.GetInvalidFileNameChars()));
        }
    }
}
