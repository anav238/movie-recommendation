using movie_recommendation.Entities;
using System.Collections.Generic;
using System.Linq;

namespace movie_recommendation.Data
{
    public class TagRepository : ITagRepository
    {
        private readonly DataContext _context;

        public TagRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Tag> GetAll(int page, int pageSize)
        {
            return _context.Tags.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public Tag GetById(int id)
        {
            return _context.Tags.Find(id);
        }

        public IEnumerable<Tag> GetTags(int movieId, int page, int pageSize)
        {
            return _context.Tags
                .Where(tag => tag.movieId == movieId).Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public Tag GetTag(int userId, int movieId)
        {
            return _context.Tags.FirstOrDefault(tag => (tag.userId == userId && tag.movieId == movieId));
                
        }

        public void Create(Tag tag)
        {
            _context.Add(tag);
            _context.SaveChanges();
        }

        public void Remove(Tag tag)
        {
            _context.Remove(tag);
            _context.SaveChanges();
        }

        public void Update(Tag tag)
        {
            _context.Update(tag);
            _context.SaveChanges();
        }
    }
}
