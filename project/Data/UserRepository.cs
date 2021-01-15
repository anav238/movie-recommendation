using Microsoft.IdentityModel.Tokens;
using movie_recommendation.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace movie_recommendation.Data
{
    public class UserRepository: Repository<User>, IUserRepository
    {

        private readonly DataContext _context;

        public UserRepository(DataContext context) : base(context)
        {
            _context = context;
        }
        public string Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return "Failed";

            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            if (user != null)
                return "User exists";

            return "Ok";

        }


        public User Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            bool verified = BCrypt.Net.BCrypt.Verify(password, user.Password);

            if (user == null)
                return null;

            if (verified == false)
                return null;

            // authentication successful
            return user;
        }

        public string Token(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("My-Top-Secret-Password");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        public IEnumerable<Movie> GetFriendMovies(int id, int friendId, int page, int pageSize)
        {
            
                var friendMovies = (from rating in _context.Ratings
                                  join friend in _context.Friendships on rating.userId equals friend.UserId_2
                                  join movie in _context.Movies on rating.movieId equals movie.Id
                                  where friend.UserId_2 == friendId && friend.UserId_1 == id
                                  select movie).Skip((page - 1) * pageSize).Take(pageSize);

                return friendMovies;
            
            
        }

        public IEnumerable<Movie> GetFriendsMovies(int id, int page, int pageSize)
        {
            var friendsMovies = (from rating in _context.Ratings
                                join friend in _context.Friendships on rating.userId equals friend.UserId_2
                                join movie in _context.Movies on rating.movieId equals movie.Id
                                where friend.UserId_1 == id
                                select movie).Skip((page - 1) * pageSize).Take(pageSize);

            return friendsMovies;
        }

        public Friendship GetFriendship(int id, int friendId)
        {
            return _context.Friendships.Find(id, friendId);
        }

        IEnumerable<Movie> IUserRepository.GetRecommendedMovies(int id)
        {
            var recommendedMovies = (from recommendation in _context.Recommendations
                                     join movie in _context.Movies on recommendation.movieId equals movie.Id
                                     where recommendation.userId == id
                                     select movie);
            return recommendedMovies;
        }

        public IEnumerable<User> GetUsersByUsername(string username, int page, int pageSize)
        {
            return _context.Users
                .Where(user => user.Username.Contains(username)).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
    }
}
