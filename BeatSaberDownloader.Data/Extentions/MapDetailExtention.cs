

using BeatSaberDownloader.Data.Models;

namespace BeatSaberDownloader.Data.Extentions
{
    public static class MapDetailExtention
    {
        public static Dictionary<string, string> GetValidFileNames(this MapDetail map, string basePath)
        {
            var result = new Dictionary<string, string>();
            foreach (var ver in map.versions)
            {
                var version = ver.hash.Substring(ver.hash.Length-5);
                var fileName = $"{map.id} - ({map.name}{version} - {map.metadata.songAuthorName} [{map.uploader.name}]).zip";
                var filePath = $@"{basePath}\{ReplaceInvalidChars(fileName)}";

                if (filePath.Length > 260)
                {
                    fileName = $"{map.id} {version} - {map.name}.zip";
                    filePath = $@"{basePath}\{ReplaceInvalidChars(fileName)}";

                    if ($@"{basePath}\{fileName}".Length > 260)
                    {
                        fileName = $"{map.id} {version} - Song name too long.zip";
                        filePath = $@"{basePath}\{ReplaceInvalidChars(fileName)}";
                    }
                }
                result.Add(ver.hash, filePath);
            }
            return result;
        }

        internal static string ReplaceInvalidChars(string filename)
        {
            return string.Join(" ", filename.Split(Path.GetInvalidFileNameChars()));
        }
    }
}
