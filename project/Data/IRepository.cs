using movie_recommendation.Entities;
using System.Collections.Generic;

namespace movie_recommendation
{
    public interface IRepository<T> where T: BaseEntity
    {
        T GetById(int id);
        IEnumerable<T> GetAll(int page, int pageSize);

        void Create(T entity);

        void Update(T entity);

        void Remove(T entity);

    }
}
