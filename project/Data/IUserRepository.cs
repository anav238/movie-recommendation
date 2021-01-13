using movie_recommendation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace movie_recommendation.Data
{
    public interface IUserRepository : IRepository<User>
    {
        public string Authenticate(string username, string password);

        public User Login(string username, string password);

        IEnumerable<Movie> GetFriendsMovies(int id, int page, int pageSize);
        IEnumerable<Movie> GetFriendMovies(int id, int friendId, int page, int pageSize);

        Friendship GetFriendship(int id, int friendId);
        IEnumerable<Movie> GetRecommendedMovies(int id);

        IEnumerable<User> GetUsersByUsername(string username, int page, int pageSize);
    }
}
