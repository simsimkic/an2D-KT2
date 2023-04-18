using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.Model;

namespace SimsProject.Repository
{
    public class TourRepository
    {
        private const string FilePath = "../../../Resources/Data/tours.csv";

        private readonly Serializer<Tour> _serializer;
        private List<Tour> _tours;

        public TourRepository()
        {
            _serializer = new Serializer<Tour>();
            _tours = _serializer.FromCsv(FilePath);
        }

        public List<Tour> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public int NextId()
        {
            _tours = _serializer.FromCsv(FilePath);
            if (_tours.Count < 1)
            {
                return 1;
            }
            return _tours.Max(t => t.Id) + 1;
        }

        public Tour Save(string name, string description, string language, int? maxGuestNumber, Location location, int? duration, List<Image>images, User user)
        {
            var id = NextId();
            _tours = _serializer.FromCsv(FilePath);
            Tour tour = new(id, name, description, language, maxGuestNumber, location,  duration, images, user);
            _tours.Add(tour);
            _serializer.ToCsv(FilePath, _tours);
            return tour;
        }

        public List<Tour> GetByUser(User user)
        {
            _tours = _serializer.FromCsv(FilePath);
            return _tours.FindAll(t => t.User.Id == user.Id);
        }

        public void Update(Tour tour)
        {
            _tours = _serializer.FromCsv(FilePath);
            Tour current = _tours.Find(t => t.Id == tour.Id);
            int index = _tours.IndexOf(current);
            _tours.Remove(current);
            _tours.Insert(index, tour);
            _serializer.ToCsv(FilePath, _tours);
        }

        public void Delete(Tour tour)
        {
            _tours = _serializer.FromCsv(FilePath);
            Tour found = _tours.Find(a => a.Id == tour.Id);
            _tours.Remove(found);
            _serializer.ToCsv(FilePath, _tours);
        }

        public Tour GetByParentId(int parentId)
        {
            _tours = GetAll();
            return _tours.Find(t => t.Id == parentId);
        }
    }
}