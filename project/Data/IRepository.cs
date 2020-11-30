using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace movie_recommendation
{
    public interface IRepository<T> where T: BaseEntity
    {
        T GetById(int id);
        IEnumerable<T> GetAll();

        void Create(T entity);

        void Update(T entity);

        void Remove(T entity);
    }
}
