using movie_recommendation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace movie_recommendation.Data
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly DataContext _context;

        public FriendshipRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Friendship> GetAll()
        {
            return _context.Friendships.ToList();
        }

        public IEnumerable<Friendship> GetFriends(int id)
        {
            return _context.Friendships.ToList()
                .Where(friendship => friendship.UserId_1 == id);
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
