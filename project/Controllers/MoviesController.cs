using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movie_recommendation.Entities;

namespace movie_recommendation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IRepository<Movie> _repository;

        public MoviesController(IRepository<Movie> repository)
        {
            _repository = repository;
        }

        // GET: api/Movies
        [HttpGet]
        public ActionResult<IEnumerable<Movie>> GetMovies() => _repository.GetAll().ToList();

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
