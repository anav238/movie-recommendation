using movie_recommendation.Entities;
using System.Collections.Generic;

namespace movie_recommendation.Data
{
    public interface IRatingRepository
    {
        void Create(Rating rating);
        IEnumerable<Rating> GetAll(int page, int pageSize);
        IEnumerable<Rating> GetRatings(int movieId, int page, int pageSize);
        Rating GetRating(int userId, int movieId);
        void Remove(Rating rating);
        void Update(Rating rating);
    }
}