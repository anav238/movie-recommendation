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
    public class TagsController : ControllerBase
    {
        private readonly ITagRepository _repository;

        public TagsController(ITagRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Tags
        [HttpGet]
        public ActionResult<IEnumerable<Tag>> GetTags(int page=1, int pageSize=100)
        {
            return _repository.GetAll(page, pageSize).ToList();
        }

        // GET: api/Tags/5
        [HttpGet("{movieId}")]
        public ActionResult<IEnumerable<Tag>> GetTags(int movieId, int page = 1, int pageSize = 100)
        {
            var tag = _repository.GetTags(movieId, page, pageSize).ToList();

            if (tag == null)
            {
                return NotFound();
            }

            return tag;
        }

        // GET: api/Tags/1/2
        [HttpGet("{userId}/{movieId}")]
        public ActionResult<Tag> GetTag(int userId, int movieId)
        {
            var tag = _repository.GetTag(userId, movieId);

            if (tag == null)
            {
                return NotFound();
            }

            return tag;
        }



        // POST: api/Tags
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public ActionResult<Tag> Create([FromBody] Tag tag)
        {
            try
            {
                _repository.Create(tag);
            }
            catch (Exception)
            {
                if (_repository.GetTag(tag.userId, tag.movieId) != null)
                  return  Conflict();
                else
                    throw;
            }
            
           
            return CreatedAtAction("GetTag", new { userId = tag.userId, movieId = tag.movieId }, tag);
        }

        // DELETE: api/Tags/5/5
        
        [HttpDelete("{userId}/{movieId}")]
        public ActionResult<Tag> DeleteTag(int userId, int movieId)
        {
            var tag = _repository.GetTag(userId, movieId);
            if (tag == null)
            {
                return NotFound();
            }

            _repository.Remove(tag);
            return tag;
        }

        
    }
}
