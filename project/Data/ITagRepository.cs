using movie_recommendation.Entities;
using System.Collections.Generic;

namespace movie_recommendation.Data
{
    public interface ITagRepository
    {
        Tag GetById(int id);
        void Create(Tag tag);
        IEnumerable<Tag> GetAll();
        IEnumerable<Tag> GetTags(int movieId);
        Tag GetTag(int userId, int movieId);
        void Remove(Tag tag);
        void Update(Tag tag);
    }
}