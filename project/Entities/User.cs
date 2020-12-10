using System;

namespace movie_recommendation.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public DateTime DateCreated { get; set; }

        public User()
        {
            this.DateCreated = DateTime.Now;
        }
    }
}
