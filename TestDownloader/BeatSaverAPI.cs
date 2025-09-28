
using BeatSaberDownloader.Data.Consts;
namespace TestDownloader
{
    public static class BeatSaverAPI
    {
        /// <summary>
        /// Search for songs using the BeatSaver API.
        /// </summary>
        /// <param name="startDate">The start date for the search. This is the created date of the songs</param>
        public static async Task<string> SearchSongsAsync(DateTime startDate)
        {
            // Logic to search for songs using the BeatSaver API
            // This is a placeholder for the actual implementation
            // You would typically use HttpClient or similar to fetch data from an API

            // Create the URL for the API request
            var url = $"{BeatSaverConsts.BeatSaverAPIBaseURL}/maps/latest?after={startDate:yyyy-MM-ddTHH:mm:ss.FFFZ}&automapper=true&pageSize={BeatSaverConsts.BeatSaverMapRequestPageSize}";
            var result = string.Empty;

            try
            {
                // Make the API call to fetch songs
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);

                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    result = content;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error fetching songs from BeatSaver API: {e.Message}", e);
            }
            return result;
        }
    }
}
