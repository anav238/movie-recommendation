using movie_recommendation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace movie_recommendation.Data
{
    public class RatingRepository : IRatingRepository
    {
        private readonly DataContext _context;

        public RatingRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Rating> GetAll()
        {
            return _context.Ratings.ToList();
        }

        public IEnumerable<Rating> GetRatings(int movieId)
        {
            return _context.Ratings.ToList()
                .Where(rating => rating.movieId == movieId);
        }

        public Rating GetRating(int userId, int movieId)
        {
            return _context.Ratings.Find(userId, movieId);
        }

        public void Create(Rating rating)
        {
            _context.Add(rating);
            _context.SaveChanges();
        }

        public void Remove(Rating rating)
        {
            _context.Remove(rating);
            _context.SaveChanges();
        }

        public void Update(Rating rating)
        {
            _context.Update(rating);
            _context.SaveChanges();
        }
    }
}
