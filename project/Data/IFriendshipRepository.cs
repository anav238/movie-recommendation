using movie_recommendation.Entities;
using System.Collections.Generic;

namespace movie_recommendation.Data
{
    public interface IFriendshipRepository
    {
        void Create(Friendship friendship);
        IEnumerable<Friendship> GetAll(int page, int pageSize);
        IEnumerable<Friendship> GetFriends(int id, int page, int pageSize);
        Friendship GetFriendship(int id_1, int id_2);
        void Remove(Friendship friendship);
        void Update(Friendship friendship);
    }
}