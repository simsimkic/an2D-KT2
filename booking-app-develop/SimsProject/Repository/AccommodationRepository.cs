using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.RepositoryInterface;

namespace SimsProject.Repository
{
    public class AccommodationRepository: IAccommodationRepository
    {
        private const string FilePath = "../../../Resources/Data/accommodations.csv";

        private readonly Serializer<Accommodation> _serializer;

        private List<Accommodation> _accommodations;

        public AccommodationRepository()
        {
            _serializer = new Serializer<Accommodation>();
            _accommodations = _serializer.FromCsv(FilePath);
        }

        public List<Accommodation> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public Accommodation GetById(int id)
        {
            _accommodations = _serializer.FromCsv(FilePath);
            return _accommodations.FirstOrDefault(a => a.Id == id);
        }

        public int NextId()
        {
            _accommodations = _serializer.FromCsv(FilePath);
            if (_accommodations.Count < 1)
            {
                return 1;
            }
            return _accommodations.Max(a => a.Id) + 1;
        }

        public Accommodation Save(Accommodation accommodation)
        {
            accommodation.Id = NextId();
            _accommodations = _serializer.FromCsv(FilePath);
            _accommodations.Add(accommodation);
            _serializer.ToCsv(FilePath, _accommodations);
            return accommodation;
        }

        public Accommodation Update(Accommodation accommodation)
        {
            _accommodations = _serializer.FromCsv(FilePath);
            Accommodation current = _accommodations.Find(a => a.Id == accommodation.Id);
            int index = _accommodations.IndexOf(current);
            _accommodations.Remove(current);
            _accommodations.Insert(index, accommodation);
            _serializer.ToCsv(FilePath, _accommodations);
            return accommodation;
        }

        public void Delete(Accommodation accommodation)
        {
            _accommodations = _serializer.FromCsv(FilePath);    
            Accommodation found = _accommodations.Find(a => a.Id == accommodation.Id);
            _accommodations.Remove(found);
            _serializer.ToCsv(FilePath, _accommodations);
        }

        public List<Accommodation> GetByOwner(User owner)
        {
            _accommodations = _serializer.FromCsv(FilePath);
            return _accommodations.FindAll(a => a.Owner.Id == owner.Id);
        }
    }
}
