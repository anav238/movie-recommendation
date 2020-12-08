using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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
        public string timestamp { get; set; }
    }
}
