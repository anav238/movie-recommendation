using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace movie_recommendation.Entities
{
    public class Friendship
    {
        [Key]
        [Column(Order=1)]
        public int UserId_1 { get; set; }

        [Key]
        [Column(Order=2)]
        public int UserId_2 { get; set; }
    }
}
