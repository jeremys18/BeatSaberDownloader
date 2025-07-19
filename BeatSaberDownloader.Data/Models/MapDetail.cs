

using BeatSaberDownloader.Data.Enums;

namespace BeatSaberDownloader.Data.Models
{
    public class MapDetail
    {
        // Current data sends back these properties
        public string id { get; set; }
        public bool automapper { get; set; }
        public bool blQualified { get; set; }
        public bool blRanked { get; set; }
        public bool bookmarked { get; set; }
        public DateTime createdAt { get; set; }
        public DeclaredAI declaredAi { get; set; }
        public string description { get; set; }
        public DateTime lastPublishedAt { get; set; }
        public MapDetailMetadata metadata { get; set; }
        public string name { get; set; }
        public bool qualified { get; set; }
        public bool ranked { get; set; }
        public MapStats stats { get; set; }
        //[JsonConverter(typeof(TagsConverter))]
        //public Tag[] tags { get; set; } // Change this back later once we know the real tags since the tags in swagger dont match
        public string[] tags { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime uploaded { get; set; }
        public UserDetail uploader { get; set; }
        public MapVersion[] versions { get; set; }
        
        
        

        // These are not in the data but are in the swagger documentation

        /*public UserDetail[] collaborators { get; set; }
        public DateTime curatedAt { get; set; }
        public UserDetail curator { get; set; }
        public DateTime deletedAt { get; set; } 
        public bool nsfw { get; set; }*/
        
        
        
        
    }
}
