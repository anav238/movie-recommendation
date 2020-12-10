using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace movie_recommendation.Entities
{
    public class Rating
    {
        [Key]
        [Column(Order = 1)]
        public int userId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int movieId { get; set; }

        [Column(Order = 3)]
        public double rating { get; set; }

        [Column(Order = 4)]
        public DateTime timestamp { get; set; }
    }
}
