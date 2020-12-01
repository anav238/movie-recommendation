using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace movie_recommendation.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public DateTime DateCreated { get => DateCreated; set => DateCreated = DateTime.Now; } 
    }
}
