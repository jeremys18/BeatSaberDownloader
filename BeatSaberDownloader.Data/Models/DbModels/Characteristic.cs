using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("Characteristic", Schema = "BeatSaver")]
    public class Characteristic
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
