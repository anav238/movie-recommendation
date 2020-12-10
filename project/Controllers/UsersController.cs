using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using movie_recommendation.Data;
using movie_recommendation.Entities;

namespace movie_recommendation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepository<User> _repository;
        private readonly IUserRepository _userRepository;



        public UsersController(IRepository<User> repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        // GET: api/Users
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers(int page = 1, int pageSize = 100)
        {
            return _repository.GetAll(page, pageSize).ToList();
        }

        [HttpGet("{id}/{friendId}/friendswatching")]

        public ActionResult<IEnumerable<Movie>> GetFriendMovies(int id, int friendId, int page = 1, int pageSize = 100)
        {
            var friendship = _userRepository.GetFriendship(id, friendId);

            if (friendship != null)
                return _userRepository.GetFriendMovies(id, friendId, page, pageSize).ToList();
            else
                return NotFound();
        }

        [HttpGet("{id}/friendswatching")]

        public ActionResult<IEnumerable<Movie>> GetFriendsMovies(int id, int page = 1, int pageSize = 100)
        {
            
                var friendsMovies = _userRepository.GetFriendsMovies(id, page, pageSize).ToList();

            if (friendsMovies.Count() != 0)
                return friendsMovies;
            else
                return NotFound();
            
        }


        // GET: api/Users/5

        [HttpGet("{id}")]
        public ActionResult<User> GetById(int id)
        {
            var user = _repository.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }



        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public ActionResult<User> Create([FromBody] User user)
        {
            _repository.Create(user);
            return CreatedAtAction("GetById", new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public ActionResult<User> Update(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest();
            else
                _repository.Update(user);
            return Ok();
        }


        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public ActionResult<User> DeleteUser(int id)
        {
            var user = _repository.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            _repository.Remove(user);     

            return user;
        }

        
    }
}
