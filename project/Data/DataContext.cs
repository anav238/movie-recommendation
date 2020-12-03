using Microsoft.EntityFrameworkCore;
using movie_recommendation.Entities;

namespace movie_recommendation.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Friendship>()
                .HasKey(friendship => new { friendship.UserId_1, friendship.UserId_2 });
            modelBuilder.Entity<Rating>()
                .HasKey(rating => new { rating.userId, rating.movieId });
            modelBuilder.Entity<Tag>()
                .HasKey(tag => new { tag.userId, tag.movieId });
            base.OnModelCreating(modelBuilder);
        }
    }
}
