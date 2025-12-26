

using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("DeclaredAI", Schema = "BeatSaver")]
    public class DeclaredAI
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
