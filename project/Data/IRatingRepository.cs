using movie_recommendation.Entities;
using System.Collections.Generic;

namespace movie_recommendation.Data
{
    public interface IRatingRepository
    {
        IEnumerable<Rating> GetUserRatings(int userId);
        void Create(Rating rating);
        IEnumerable<Rating> GetAll();
        IEnumerable<Rating> GetRatings(int movieId);
        Rating GetRating(int userId, int movieId);
        void Remove(Rating rating);
        void Update(Rating rating);
    }
}