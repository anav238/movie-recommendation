using movie_recommendation.Entities;
using System.Collections.Generic;

namespace movie_recommendation.Data
{
    public interface ITagRepository
    {
        Tag GetById(int id);
        void Create(Tag tag);
        IEnumerable<Tag> GetAll(int page, int pageSize);
        IEnumerable<Tag> GetTags(int movieId, int page, int pageSize);
        Tag GetTag(int userId, int movieId);
        void Remove(Tag tag);
        void Update(Tag tag);
    }
}