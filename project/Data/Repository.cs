using movie_recommendation.Entities;
using System.Collections.Generic;
using System.Linq;

namespace movie_recommendation.Data
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DataContext _context;
        public Repository(DataContext context)
        {
            _context = context;
        }
        public void Create(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
        }

        public IEnumerable<T> GetAll(int page, int pageSize)
        {
            return _context.Set<T>().Skip((page - 1) * pageSize).Take(pageSize).AsEnumerable();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            if (user == null)
                return null;

            if (password != user.Password)
                return null;

            // authentication successful
            return user;
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
        }
    }
}
