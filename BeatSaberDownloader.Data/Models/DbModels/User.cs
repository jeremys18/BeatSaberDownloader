

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("User", Schema = "BeatSaver")]
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public int Id { get; set; }
        public bool Admin { get; set; }
        public string Avatar { get; set; }
        public bool Curator { get; set; }
        public string Name { get; set; }
        public string PlaylistUrl { get; set; }
        public bool SeniorCurator { get; set; }
        public int UserTypeId { get; set; }
        
        public virtual UserType UserType { get; set; }
    }
}
