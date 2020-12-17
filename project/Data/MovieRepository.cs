using Microsoft.EntityFrameworkCore;
using movie_recommendation.Entities;
using System.Collections.Generic;
using System.Linq;

namespace movie_recommendation.Data
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly DataContext _context;

        public MovieRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Rating> GetMovieRatings(int movieId, int page, int pageSize)
        {
            return _context.Ratings
                .Where(rating => rating.movieId == movieId).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public object GetMovieRating(int movieId)
        {
            var rating =  _context.Ratings
                .Where(rating => rating.movieId == movieId).Take(1000).Average(rating => rating.rating);

            return new
            {
                movie = GetById(movieId),
                rating = rating

            };
        }

        public IEnumerable<Tag> GetMovieTags(int movieId, int page, int pageSize)
        {
            return _context.Tags
                .Where(tag => tag.movieId == movieId).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public IEnumerable<Movie> GetMoviesByGenre(string genre, int page, int pageSize)
        {
            genre = char.ToUpper(genre[0]) + genre.Substring(1);
            return _context.Movies
                .Where(movie => movie.Genres.Contains(genre)).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public IEnumerable<Movie> GetMoviesByRelease( int page, int pageSize)
        {
            bool condition(string x) => x.LastIndexOf("(") != -1 && ( x.Substring(x.LastIndexOf("("))[1] == '1' || x.Substring(x.LastIndexOf("("))[1] == '2' ) ;

            return _context.Movies.ToList()
                .OrderByDescending(movie => condition(movie.Title)? movie.Title.Substring(movie.Title.LastIndexOf("(")) : null ).Skip((page - 1) * pageSize).Take(pageSize);
        }

    }
}
