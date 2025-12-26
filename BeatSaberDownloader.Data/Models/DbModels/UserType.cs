using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("UserType", Schema = "BeatSaver")]
    public class UserType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
