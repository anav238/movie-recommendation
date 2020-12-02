﻿using Microsoft.EntityFrameworkCore;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Friendship>()
                .HasKey(friendship => new { friendship.UserId_1, friendship.UserId_2 });
            base.OnModelCreating(modelBuilder);
        }
    }
}
