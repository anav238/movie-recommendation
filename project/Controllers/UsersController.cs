using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using movie_recommendation.Data;
using movie_recommendation.Entities;

namespace movie_recommendation.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepository<User> _repository;
        private readonly IUserRepository _userRepository;
        private readonly IMovieRepository _movieRepository;


        public UsersController(IRepository<User> repository, IUserRepository userRepository, IMovieRepository movieRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
            _movieRepository = movieRepository;
        }

        // GET: api/Users
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetUsers(int page = 1, int pageSize = 100)
        {
            return _repository.GetAll(page, pageSize).ToList();
        }

        [Authorize]
        [HttpGet("{id}/{friendId}/friendswatching")]

        public ActionResult<IEnumerable<Movie>> GetFriendMovies(int id, int friendId, int page = 1, int pageSize = 100)
        {
            var friendship = _userRepository.GetFriendship(id, friendId);

            if (friendship != null)
                return _userRepository.GetFriendMovies(id, friendId, page, pageSize).ToList();
            else
                return NotFound();
        }

        [Authorize]
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
        [Authorize]
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

        [Authorize]
        [HttpGet("{id}/recommendations")]
        public ActionResult<IEnumerable<Movie>> GetRecommendationsForUser(int id)
        {
            var recommendations = _userRepository.GetRecommendedMovies(id);

            if (recommendations == null || recommendations.ToList().Count == 0)
            {
                return _movieRepository.GetBestRatedMovies(1, 25).ToList();
            }

            return recommendations.ToList();
        }

        [Authorize]
        [HttpGet("search/{username}")]
        public ActionResult<IEnumerable<Tuple<int,string>>> GetUsersByUsername(string username, int page = 1, int pageSize = 100)
        {
            var users = _userRepository.GetUsersByUsername(username, page, pageSize).ToList();

            if (users.Count() == 0)
            {
                return NotFound();
            }

            List<Tuple<int,string>> userIds = new List<Tuple<int,string>>();
            for (int i = 0; i < users.Count(); i++)
                userIds.Add(new Tuple<int, string>(users[i].Id, users[i].Username));

            return Ok(userIds);
        }

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public ActionResult<User> Create([FromBody] User user)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Password = hashedPassword;
            _repository.Create(user);
            return CreatedAtAction("GetById", new { id = user.Id }, user);
        }

        [HttpPost("authenticate")]
        public ActionResult<User> Authenticate([FromBody] User user)
        {
            var userStatus = _userRepository.Authenticate(user.Username, user.Password);
            if (userStatus == "Failed")
            {
                return BadRequest(new { message = "Failed" });
            }
            if (userStatus == "User exists")
            {
                return BadRequest(new { message = "username exist" });
            }

            var tokenString = _userRepository.Token(user.Username);

            return Ok(new
            {
                Username = user.Username,
                password = user.Password,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            var userRegistered = _userRepository.Login(user.Username, user.Password);
            if (userRegistered == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenString = _userRepository.Token(userRegistered.Username);

            return Ok(new
            {
                Id = userRegistered.Id,
                Username = userRegistered.Username,
                Token = tokenString
            });

        }

        [Authorize]
        [HttpPut("{id}")]
        public ActionResult<User> Update(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest();
            else
                _repository.Update(user);
            return Ok();
        }

        [Authorize]
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
