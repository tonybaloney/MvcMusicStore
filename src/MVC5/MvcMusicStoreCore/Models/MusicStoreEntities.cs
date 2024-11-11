using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace MvcMusicStoreCore.Models
{
    public class MusicStoreEntities(string connectionString) : DbContext
    {
        public DbSet<Album>     Albums { get; set; }
        public DbSet<Genre>     Genres { get; set; }
        public DbSet<Artist>    Artists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseCosmos(connectionString, databaseName: "MvcMusicStore");
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                var testGenre = await context.Set<Genre>().FirstOrDefaultAsync(b => b.Name == "Pop", cancellationToken);
                if (testGenre == null)
                {
                    await SampleData.Seed(context);
                }
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultContainer("Store");
            modelBuilder.Entity<Album>(entity =>
            {
                entity.HasKey(e => e.AlbumId);
                entity.Property(e => e.Title).IsRequired();
                entity.ToContainer("Albums");
            });
            modelBuilder.Entity<Artist>(entity =>
            {
                entity.HasKey(e => e.ArtistId);
                entity.Property(e => e.Name).IsRequired();
            });
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(e => e.Name);
                entity.Property(e => e.Name).IsRequired();
            });
        }
    }
}