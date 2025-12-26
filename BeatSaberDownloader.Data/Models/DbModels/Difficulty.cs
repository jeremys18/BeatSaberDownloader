
using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("Difficulty", Schema = "BeatSaver")]
    public class Difficulty
    {
        public int Id { get; set; }
        public float BlStars { get; set; }
        public int Bombs { get; set; }
        public bool Chroma { get; set; }
        public bool Cinema { get; set; }
        public int CharacteristicId { get; set; }
        public int DifficultyId { get; set; }
        public int EnvironmentId { get; set; }
        public int Events { get; set; }
        public string Label { get; set; }
        public double Length { get; set; }
        public int MaxScore { get; set; }
        public bool ME { get; set; }
        public bool NE { get; set; }
        public float NJS { get; set; }
        public int Notes { get; set; }
        public double NPS { get; set; }
        public int Obstacles { get; set; }
        public float Offset { get; set; }
        public int ParitySummaryId { get; set; }
        public double Seconds { get; set; }
        public float Stars { get; set; }
        public bool Vivify { get; set; }

        public virtual Characteristic Characteristic { get; set; }
        public virtual Difficulty2 Difficulty2 { get; set; }
        public virtual Environment Environment { get; set; }
        public ParitySummary ParitySummary { get; set; }
    }
}
