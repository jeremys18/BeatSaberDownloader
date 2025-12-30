

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("User", Schema = "BeatSaver")]
    public class User
    {
        public int Id { get; set; }

        // External id from BeatSaver API; avoid naming collision with PK
        public int ExternalId { get; set; }

        public bool Admin { get; set; }
        public string Avatar { get; set; } = string.Empty;
        public bool Curator { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PlaylistUrl { get; set; } = string.Empty;
        public bool SeniorCurator { get; set; }
        public int UserTypeId { get; set; }

        public virtual UserType? UserType { get; set; }
        public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
