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
        public DbSet<SongTag> SongTags { get; set; } = null!;
        public DbSet<State> States { get; set; } = null!;
        public DbSet<Stats> Stats { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<TestPlay> TestPlays { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserType> UserTypes { get; set; } = null!;
        public DbSet<Version> Versions { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure the database connection string here
            optionsBuilder.UseSqlServer("Server=.;Database=BeatSaver;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Song>(entity =>
            {
                entity.HasKey(e => e.SongId);
                entity.ToTable("Song", "BeatSaver");
            });

            // Map Tag primary key column to TagId
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Tag", "BeatSaver");
            });

            modelBuilder.Entity<SongTag>().HasKey(st => st.Id);
            modelBuilder.Entity<Song>()
                .HasMany(s => s.Tags)
                .WithMany(t => t.Songs)
                .UsingEntity<SongTag>(
                    j => j.HasOne(st => st.Tag).WithMany().HasForeignKey(st => st.TagId),
                    j => j.HasOne(st => st.Song).WithMany().HasForeignKey(st => st.SongId));

            base.OnModelCreating(modelBuilder);
        }
    }
}
