using movie_recommendation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace movie_recommendation.Data
{
    public interface IMovieRepository : IRepository<Movie>
    {
        IEnumerable<Rating> GetMovieRatings(int movieId, int page, int pageSize);
        IEnumerable<Tag> GetMovieTags(int movieId, int page, int pageSize);

        IEnumerable<Movie> GetMoviesByGenre(string genre, int page, int pageSize);

        object GetMovieRating(int movieId);

    }
}
