using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface IUserRepository : IRepository<User>
    {
        User GetByUsername(string username);
    }
}
