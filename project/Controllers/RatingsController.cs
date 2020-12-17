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

        public RatingsController(IRatingRepository repository)
        {
            _repository = repository;
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

        // GET: api/Ratings/1/2
        [HttpGet("{userId}/{movieId}")]
        public ActionResult<Rating> GetRating(int userId, int movieId)
        {
            var rating = _repository.GetRating(userId, movieId);

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
            
           
            return CreatedAtAction("GetRating", new { id_1 = rating.userId, id_2 = rating.movieId }, rating);
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
