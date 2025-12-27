
using BeatSaberDownloader.Data.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using Version = BeatSaberDownloader.Data.Models.DbModels.Version;
using Environment = BeatSaberDownloader.Data.Models.DbModels.Environment;

namespace BeatSaberDownloader.Data.DBContext 
{
    public class BeatSaverContext : DbContext
    {
        public DbSet<Characteristic> Characteristics { get; set; } = null!;
        public DbSet<DeclaredAI> DeclaredAIs { get; set; } = null!;
        public DbSet<Difficulty> Difficulties { get; set; } = null!;
        public DbSet<Difficulty2> Difficulties2 { get; set; } = null!;
        public DbSet<Environment> Environments { get; set; } = null!;
        public DbSet<MetaData> MetaDatas { get; set; } = null!;
        public DbSet<ParitySummary> ParitySummaries { get; set; } = null!;
        public DbSet<Sentiment> Sentiments { get; set; } = null!;
        public DbSet<Song> Songs { get; set; } = null!;
        public DbSet<State> States { get; set; } = null!;
        public DbSet<Stats> Stats { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<TestPlay> TestPlays { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserType> UserTypes { get; set; } = null!;
        public DbSet<Version> Versions { get; set; } = null!;
    }
}
