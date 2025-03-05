

using BeatSaberDownloader.Data.Enums;

namespace BeatSaberDownloader.Data.Models
{
    public class MapDetail
    {
        public string id { get; set; }
        public bool automapper { get; set; }
        public bool blQualified { get; set; }
        public bool blRanked { get; set; }
        public bool bookmarked { get; set; }
        public UserDetail[] collaborators { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime curatedAt { get; set; }
        public UserDetail curator { get; set; }
        public DeclaredAI declaredAi { get; set; }
        public DateTime deletedAt { get; set; }
        public string description { get; set; }
        public DateTime lastPublishedAt { get; set; }
        public MapDetailMetadata metadata { get; set; }
        public string name { get; set; }
        public bool nsfw { get; set; }
        public bool qualified { get; set; }
        public bool ranked { get; set; }
        public MapStats stats { get; set; }
        public Tag[] tags { get; set; } 
        public DateTime updatedAt { get; set; }
        public DateTime uploaded { get;set; }
        public MapVersion[] versions { get; set; }
    }
}
