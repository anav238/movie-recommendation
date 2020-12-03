using movie_recommendation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace movie_recommendation.Data
{
    public class TagRepository : ITagRepository
    {
        private readonly DataContext _context;

        public TagRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Tag> GetAll()
        {
            return _context.Tags.ToList();
        }

        public Tag GetById(int id)
        {
            return _context.Tags.Find(id);
        }

        public IEnumerable<Tag> GetTags(int movieId)
        {
            return _context.Tags.ToList()
                .Where(tag => tag.movieId == movieId);
        }

        public Tag GetTag(int userId, int movieId)
        {
            return _context.Tags.Find(userId, movieId);
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
