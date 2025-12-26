using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("State", Schema = "BeatSaver")]
    public class State
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
