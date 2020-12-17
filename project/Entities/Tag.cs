﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace movie_recommendation.Entities
{
    public class Tag
    {
        [Key]
        [Column(Order = 1)]
        public int id { get; set; }

        [Column(Order = 2)]
        public int userId { get; set; }

        [Column(Order = 3)]
        public int movieId { get; set; }

        [Column(Order = 4)]
        public String tag { get; set; }

        [Column(Order = 5)]
        public DateTime timestamp { get; set; }

        public Tag()
        {
            this.timestamp = DateTime.Now;
        }
    }
}
