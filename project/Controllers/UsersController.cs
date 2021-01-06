using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
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


        [HttpGet("{id}/recommendations")]
        public ActionResult<IEnumerable<Movie>> GetRecommendationsForUser (int id)
        {
            var recommendations = _userRepository.GetRecommendedMovies(id);

            if (recommendations == null)
            {
                return NotFound();
            }

            return recommendations.ToList();
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

        [HttpPost("authenticate")]
        public ActionResult<User> Authenticate([FromBody] User user)
        {
            var user_log = _repository.Authenticate(user.Username, user.Password);
            if (user_log == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("My-Top-Secret-Password");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                Id = user_log.Id,
                Username = user_log.Username,
                Token = tokenString
            });
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
