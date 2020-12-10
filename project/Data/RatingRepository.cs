using movie_recommendation.Entities;
using System.Collections.Generic;
using System.Linq;

namespace movie_recommendation.Data
{
    public class RatingRepository : IRatingRepository
    {
        private readonly DataContext _context;

        public RatingRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Rating> GetAll(int page, int pageSize)
        {
            return _context.Ratings.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public IEnumerable<Rating> GetRatings(int movieId, int page, int pageSize)
        {
            return _context.Ratings
                .Where(rating => rating.movieId == movieId).Skip((page - 1) * pageSize).Take(pageSize).ToList();
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
