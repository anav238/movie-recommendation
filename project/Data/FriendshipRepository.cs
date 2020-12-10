using movie_recommendation.Entities;
using System.Collections.Generic;
using System.Linq;

namespace movie_recommendation.Data
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly DataContext _context;

        public FriendshipRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Friendship> GetAll(int page, int pageSize)
        {
            return _context.Friendships.Skip((page-1) * pageSize).Take(pageSize).ToList();
        }

        public IEnumerable<Friendship> GetFriends(int id, int page, int pageSize)
        {
            return _context.Friendships
                .Where(friendship => friendship.UserId_1 == id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public Friendship GetFriendship(int id_1, int id_2)
        {
            return _context.Friendships.Find(id_1, id_2);
        }

        public void Create(Friendship friendship)
        {
            _context.Add(friendship);
            _context.SaveChanges();
        }

        public void Remove(Friendship friendship)
        {
            _context.Remove(friendship);
            _context.SaveChanges();
        }

        public void Update(Friendship friendship)
        {
            _context.Update(friendship);
            _context.SaveChanges();
        }
    }
}
