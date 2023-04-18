using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;

namespace SimsProject.Repository
{
    public class LocationRepository
    {
        private const string FilePath = "../../../Resources/Data/locations.csv";

        private readonly Serializer<Location> _serializer;

        private List<Location> _locations;

        public LocationRepository()
        {
            _serializer = new Serializer<Location>();
            _locations = _serializer.FromCsv(FilePath);
        }

        public List<Location> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public List<string> GetAllCountries()
        {
            _locations = _serializer.FromCsv(FilePath);
            return _locations.Select(o => o.Country).Distinct().ToList();
        }

        public List<string> GetAllCities()
        {
            _locations = _serializer.FromCsv(FilePath);
            return _locations.Select(o => o.City).Distinct().ToList();
        }

        public List<Location> GetAllCitiesByCountry(string country)
        {
            _locations = _serializer.FromCsv(FilePath);
            return _locations.FindAll(o => o.Country.ToString() == country);
        }

        public Location Save(Location location)
        {
            location.Id = NextId();
            _locations = _serializer.FromCsv(FilePath);
            _locations.Add(location);
            _serializer.ToCsv(FilePath, _locations);
            return location;
        }

        public int NextId()
        {
            _locations = _serializer.FromCsv(FilePath);
            if (_locations.Count < 1)
            {
                return 1;
            }
            return _locations.Max(l => l.Id) + 1;
        }

        public void Delete(Location location)
        {
            _locations = _serializer.FromCsv(FilePath);
            Location found = _locations.Find(a => a.Id == location.Id);
            _locations.Remove(found);
            _serializer.ToCsv(FilePath, _locations);
        }

        public Location GetByCityAndCountry(string city, string country)
        {
            _locations = _serializer.FromCsv(FilePath);
            return _locations.Find(l => l.City == city && l.Country == country);
        }

        public Location GetById(int id)
        {
            _locations = _serializer.FromCsv(FilePath);
            return _locations.FirstOrDefault(l => l.Id == id);
        }
    }
}
