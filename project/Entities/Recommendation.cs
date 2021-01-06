using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace movie_recommendation.Entities
{
    public class Recommendation
    {
        [Key]
        [Column(Order = 1)]
        public int userId;

        [Key]
        [Column(Order = 2)]
        public int movieId;
    }
}
