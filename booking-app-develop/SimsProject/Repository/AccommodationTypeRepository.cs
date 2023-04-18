using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;

namespace SimsProject.Repository
{
    public class AccommodationTypeRepository
    {
        private const string FilePath = "../../../Resources/Data/accommodationTypes.csv";

        private readonly Serializer<AccommodationType> _serializer;

        private List<AccommodationType> _accommodationTypes;

        public AccommodationTypeRepository()
        {
            _serializer = new Serializer<AccommodationType>();
            _accommodationTypes = _serializer.FromCsv(FilePath);
        }

        public List<AccommodationType> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public AccommodationType Save(AccommodationType accommodationType)
        {
            accommodationType.Id = NextId();
            _accommodationTypes = _serializer.FromCsv(FilePath);
            _accommodationTypes.Add(accommodationType);
            _serializer.ToCsv(FilePath, _accommodationTypes);
            return accommodationType;
        }

        public int NextId()
        {
            _accommodationTypes = _serializer.FromCsv(FilePath);
            if (_accommodationTypes.Count < 1)
            {
                return 1;
            }
            return _accommodationTypes.Max(t => t.Id) + 1;
        }

        public AccommodationType GetById(int id)
        {
            _accommodationTypes = _serializer.FromCsv(FilePath);
            return _accommodationTypes.FirstOrDefault(t => t.Id == id);
        }
    }
}
