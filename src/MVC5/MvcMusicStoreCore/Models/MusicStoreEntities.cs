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
            optionsBuilder.UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                var testGenre = await context.Set<Genre>().FirstOrDefaultAsync(b => b.Name == "Pop");
                if (testGenre == null)
                {
                    await SampleData.Seed(context);
                }
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Album>(entity =>
            {
                entity.HasKey(e => e.AlbumId);
                entity.Property(e => e.Title).IsRequired();
                entity.HasOne(d => d.Artist).WithMany(p => p.Albums).HasForeignKey(d => d.AlbumId);
                entity.HasOne(d => d.Genre).WithMany(p => p.Albums).HasForeignKey(d => d.AlbumId);
            });
            modelBuilder.Entity<Artist>(entity =>
            {
                entity.HasKey(e => e.ArtistId);
                entity.Property(e => e.Name).IsRequired();
            });
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(e => e.GenreId);
                entity.Property(e => e.Name).IsRequired();
            });
        }
    }
}