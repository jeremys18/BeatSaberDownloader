using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("Tag", Schema = "BeatSaver")]
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
