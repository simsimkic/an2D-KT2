using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.RepositoryInterface;

namespace SimsProject.Repository
{
    public class AccommodationReservationRescheduleRepository : IAccommodationReservationRescheduleRepository
    {
        private const string FilePath = "../../../Resources/Data/accommodationReservationReschedules.csv";

        private readonly Serializer<AccommodationReservationReschedule> _serializer;

        private List<AccommodationReservationReschedule> _accommodationReservationReschedules;

        public AccommodationReservationRescheduleRepository()
        {
            _serializer = new Serializer<AccommodationReservationReschedule>();
            _accommodationReservationReschedules = _serializer.FromCsv(FilePath);
        }

        public List<AccommodationReservationReschedule> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public AccommodationReservationReschedule GetById(int id)
        {
            _accommodationReservationReschedules = _serializer.FromCsv(FilePath);
            return _accommodationReservationReschedules.FirstOrDefault(a => a.Id == id);
        }

        public int NextId()
        {
            _accommodationReservationReschedules = _serializer.FromCsv(FilePath);
            if (_accommodationReservationReschedules.Count < 1)
            {
                return 1;
            }
            return _accommodationReservationReschedules.Max(c => c.Id) + 1;
        }

        public AccommodationReservationReschedule Save(AccommodationReservationReschedule accommodationReservationMoveRequest)
        {
            accommodationReservationMoveRequest.Id = NextId();
            _accommodationReservationReschedules = _serializer.FromCsv(FilePath);
            _accommodationReservationReschedules.Add(accommodationReservationMoveRequest);
            _serializer.ToCsv(FilePath, _accommodationReservationReschedules);
            return accommodationReservationMoveRequest;
        }

        public AccommodationReservationReschedule Update(AccommodationReservationReschedule accommodationReservationReschedule)
        {
            _accommodationReservationReschedules = _serializer.FromCsv(FilePath);
            AccommodationReservationReschedule current = _accommodationReservationReschedules.Find(a => a.Id == accommodationReservationReschedule.Id);
            int index = _accommodationReservationReschedules.IndexOf(current);
            _accommodationReservationReschedules.Remove(current);
            _accommodationReservationReschedules.Insert(index, accommodationReservationReschedule);
            _serializer.ToCsv(FilePath, _accommodationReservationReschedules);
            return accommodationReservationReschedule;
        }

        public void Delete(AccommodationReservationReschedule accommodationReservationMoveRequest)
        {
            _accommodationReservationReschedules = _serializer.FromCsv(FilePath);
            AccommodationReservationReschedule founded = _accommodationReservationReschedules.Find(c => c.Id == accommodationReservationMoveRequest.Id);
            _accommodationReservationReschedules.Remove(founded);
            _serializer.ToCsv(FilePath, _accommodationReservationReschedules);
        }

        public List<AccommodationReservationReschedule> GetByGuest(User guest)
        {
            _accommodationReservationReschedules = _serializer.FromCsv(FilePath);
            return _accommodationReservationReschedules.FindAll(r => r.Guest.Id == guest.Id);
        }

        public List<AccommodationReservationReschedule> GetByOwner(User owner)
        {
            _accommodationReservationReschedules = _serializer.FromCsv(FilePath);
            return _accommodationReservationReschedules.FindAll(r => r.Owner.Id == owner.Id);
        }

        public bool CheckNotifyGuest(User user)
        {
            _accommodationReservationReschedules = _serializer.FromCsv(FilePath);
            return _accommodationReservationReschedules.Any(r => r.Guest.Id==user.Id && r.RequestStatus != Status.Waiting && r.Notify);
        }
    }
}
