﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using movie_recommendation.Data;

namespace movie_recommendation.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20201216123303_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("movie_recommendation.Entities.Friendship", b =>
                {
                    b.Property<int>("UserId_1")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId_2")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId_1", "UserId_2");

                    b.ToTable("Friendships");
                });

            modelBuilder.Entity("movie_recommendation.Entities.Movie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Genres")
                        .HasColumnType("TEXT");

                    b.Property<int>("ImdbId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<int>("TmdbId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("movie_recommendation.Entities.Rating", b =>
                {
                    b.Property<int>("userId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("movieId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("rating")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("userId", "movieId");

                    b.ToTable("Ratings");
                });

            modelBuilder.Entity("movie_recommendation.Entities.Tag", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("movieId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("tag")
                        .HasColumnType("TEXT");

                    b.Property<string>("timestamp")
                        .HasColumnType("TEXT");

                    b.Property<int>("userId")
                        .HasColumnType("INTEGER");

                    b.HasKey("id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("movie_recommendation.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
