
using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("Tag", Schema = "BeatSaver")]
    public class Tag
    {
        public int TagId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
