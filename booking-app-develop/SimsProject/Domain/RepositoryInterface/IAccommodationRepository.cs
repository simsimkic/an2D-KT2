using System.Collections.Generic;
using SimsProject.Domain.Model;

namespace SimsProject.Domain.RepositoryInterface
{
    public interface IAccommodationRepository: IRepository<Accommodation>
    {
        List<Accommodation> GetByOwner(User owner);
    }
}
