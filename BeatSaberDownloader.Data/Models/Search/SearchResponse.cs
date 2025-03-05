namespace BeatSaberDownloader.Data.Models.Search
{
    public class SearchResponse
    {
        public MapDetail[] docs { get; set; }
        public SearchInfo info { get; set; }
        public string redirect { get; set; }
    }
}
