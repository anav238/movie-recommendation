using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using movie_recommendation.Data;
using movie_recommendation.Entities;

namespace movie_recommendation.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingRepository _repository;
        private readonly IRepository<Movie> _movieRepository;

        public RatingsController(IRatingRepository repository, IRepository<Movie> movieRepository)
        {
            _repository = repository;
            _movieRepository = movieRepository;
        }

        // GET: api/Ratings
        [HttpGet]
        public ActionResult<IEnumerable<Rating>> GetRatings(int page=1, int pageSize = 100)
        {
            return _repository.GetAll(page,pageSize).ToList();
        }

        // GET: api/Ratings/5
        [HttpGet("{movieId}")]
        public ActionResult<IEnumerable<Rating>> GetRatings(int movieId, int page=1, int pageSize=100)
        {
            var rating = _repository.GetRatings(movieId, page, pageSize).ToList();

            if (rating == null)
            {
                return NotFound();
            }

            return rating;
        }

       



        // POST: api/Ratings
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public ActionResult<Rating> Create([FromBody] Rating rating)
        {
            try
            {
                _repository.Create(rating);
            }
            catch (Exception)
            {
                if (_repository.GetRating(rating.userId, rating.movieId) != null)
                  return  Conflict();
                else
                    throw;
            }

            var movie = _movieRepository.GetById(rating.movieId);
            movie.Rating = ((float)rating.rating + movie.Rating) / (movie.NumberOfRatings + 1);
            movie.NumberOfRatings = movie.NumberOfRatings + 1;
            _movieRepository.Update(movie);
            return CreatedAtAction("GetRating", new { userId = rating.userId, movieId = rating.movieId }, rating);
        }

        // GET: api/Ratings/1/2
        [HttpGet("{userId}/{movieId}", Name = "GetRating")]
        public ActionResult<Rating> GetRating(int userId, int movieId)
        {
            var rating = _repository.GetRating(userId, movieId);

            if (rating == null)
            {
                return NotFound();
            }

            return rating;
        }

        // DELETE: api/Ratings/5/5

        [HttpDelete("{userId}/{movieId}")]
        public ActionResult<Rating> DeleteRating(int userId, int movieId)
        {
            var rating = _repository.GetRating(userId, movieId);
            if (rating == null)
            {
                return NotFound();
            }

            _repository.Remove(rating);
            return rating;
        }

        
    }
}
