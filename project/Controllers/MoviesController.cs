using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movie_recommendation.Data;
using movie_recommendation.Entities;

namespace movie_recommendation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IRepository<Movie> _repository;
        private readonly IMovieRepository _movieRepository;

        

        public MoviesController(IRepository<Movie> repository, IMovieRepository movieRepository)
        {
            _repository = repository;
            _movieRepository = movieRepository;
        }


        // GET: api/Movies
        [HttpGet]
        public ActionResult<IEnumerable<Movie>> GetMovies(int page = 1, int pageSize = 100) => _repository.GetAll(page, pageSize).ToList();

        //[Authorize]
        // GET: api/Movies/{id}
        [HttpGet("{id}", Name = "GetById")]
        public ActionResult<Movie> GetById(int id)
        {
            var movie = _repository.GetById(id);

            if (movie == null)
            {
                return NotFound();
            }

            return movie;
        }

        [HttpGet("{movieId}/rating")]
        public ActionResult<object> GetMovieRating(int movieId)
        {
            return _movieRepository.GetMovieRating(movieId);
        }

        [HttpGet("{movieId}/ratings")]
        public ActionResult<IEnumerable<Rating>> GetMovieRatings(int movieId, int page = 1, int pageSize = 100)
        {
            return _movieRepository.GetMovieRatings(movieId, page, pageSize).ToList();
        }

        [HttpGet("{movieId}/tags")]
        public ActionResult<IEnumerable<Tag>> GetMovieTags(int movieId, int page = 1, int pageSize = 100)
        {
            return _movieRepository.GetMovieTags(movieId, page, pageSize).ToList();
        }

        [HttpGet("genre={genre}")]
        public ActionResult<IEnumerable<Movie>> GetMoviesByGenre(string genre, int page = 1, int pageSize = 100)
        {   
            return _movieRepository.GetMoviesByGenre(genre, page, pageSize).ToList();
        }


        // GET: api/Movies
        [HttpPost]
        public IActionResult CreateMovie([FromBody] Movie movie)
        {
            _repository.Create(movie);
            return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie);
        }


        // DELETE: api/Movies/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteMovie(int id)
        {
            var contact = _repository.GetById(id);
            if (contact == null)
            {
                return NotFound();
            }

            _repository.Remove(contact);
            return NoContent();
        }


        // PUT: api/Movies/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateMovie(int id, [FromBody] Movie movie)
        {

            if (id != movie.Id)
            {
                return BadRequest();
            }

            _repository.Update(movie);
            return Ok();
        }

    }
}
