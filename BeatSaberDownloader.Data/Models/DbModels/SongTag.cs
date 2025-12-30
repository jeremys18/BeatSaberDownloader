
using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("SongTag", Schema = "BeatSaver")]
    public class SongTag
    {
        public int Id { get; set; }
        public int SongId { get; set; }
        public int TagId { get; set; }

        public virtual Song Song { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
