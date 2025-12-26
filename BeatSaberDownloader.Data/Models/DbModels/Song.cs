

using BeatSaberDownloader.Data.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    public class Song
    {
        [Table("Song", Schema = "BeatSaver")]
        public class MapDetail
        {
            [JsonIgnore]
            public int SongId { get; set; }
            // Current data sends back these properties
            public string Id { get; set; }
            public bool Automapper { get; set; }
            public bool BlQualified { get; set; }
            public bool BlRanked { get; set; }
            public bool Bookmarked { get; set; }
            public DateTime CreatedAt { get; set; }
            public DeclaredAI DeclaredAi { get; set; }
            public string Description { get; set; }
            public DateTime LastPublishedAt { get; set; }
            public int MetadataId { get; set; }
            public string Name { get; set; }
            public bool Qualified { get; set; }
            public bool Ranked { get; set; }
            public int StatsId{ get; set; }
            //[JsonConverter(typeof(TagsConverter))]
            //public Tag[] tags { get; set; } // Change this back later once we know the real tags since the tags in swagger dont match
            public string[] Tags { get; set; }
            public DateTime UpdatedAt { get; set; }
            public DateTime Uploaded { get; set; }
            public int UploaderId { get; set; }


            public virtual MapDetailMetadata Metadata { get; set; }
            public virtual MapStats Stats { get; set; }
            public virtual UserDetail Uploader { get; set; }
            public virtual IEnumerable<MapVersion> Versions { get; set; }

            // These are not in the data but are in the swagger documentation

            /*public UserDetail[] collaborators { get; set; }
            public DateTime curatedAt { get; set; }
            public UserDetail curator { get; set; }
            public DateTime deletedAt { get; set; } 
            public bool nsfw { get; set; }*/
        }
    }
}
