
using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSaberDownloader.Data.Models.DbModels
{
    [Table("Difficulty", Schema = "BeatSaver")]
    public class Difficulty
    {
        public int Id { get; set; }
        public float BlStars { get; set; }
        public int Bombs { get; set; }
        //public Characteristic characteristic { get; set; } change back to enum once we know all the current values and how to prase them as the docs dont match the real values sent back to us. This sucks
        public string Characteristic { get; set; } // Temporary workaround for the issue mentioned above
        public bool Chroma { get; set; }
        public bool Cinema { get; set; }
        public Enums.Difficulty Difficulty { get; set; }
        public Enums.Environment environment { get; set; }
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
        public MapParitySummary ParitySummary { get; set; }
        public double Seconds { get; set; }
        public float Stars { get; set; }
        public bool Vivify { get; set; }
    }
}
