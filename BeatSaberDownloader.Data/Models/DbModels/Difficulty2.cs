using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("Difficulty2", Schema = "BeatSaver")]
    public class Difficulty2
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
