using System.Collections.Generic;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface IRepository<T>
    {
        List<T> GetAll();
        T GetById(int id);
        int NextId();
        T Save(T entity);
        T Update(T entity);
        void Delete(T entity);
    }
}
