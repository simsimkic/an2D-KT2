using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimsProject.Repository
{
    public class AccommodationReservationRepository
    {
        private const string FilePath = "../../../Resources/Data/accommodationReservations.csv";

        private readonly Serializer<AccommodationReservation> _serializer;

        private List<AccommodationReservation> _accommodationReservations;

        public AccommodationReservationRepository()
        {
            _serializer = new Serializer<AccommodationReservation>();
            _accommodationReservations = _serializer.FromCsv(FilePath);
        }

        public List<AccommodationReservation> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }
        public AccommodationReservation Save(AccommodationReservation accommodationReservation) 
        {
            accommodationReservation.Id = NextId();
            _accommodationReservations = _serializer.FromCsv(FilePath);
            _accommodationReservations.Add(accommodationReservation);
            _serializer.ToCsv(FilePath, _accommodationReservations);
            return accommodationReservation;
        }
        public AccommodationReservation Update(AccommodationReservation accommodationReservation)
        {
            _accommodationReservations = _serializer.FromCsv(FilePath);
            AccommodationReservation current = _accommodationReservations.Find(a => a.Id == accommodationReservation.Id);
            int index = _accommodationReservations.IndexOf(current);
            _accommodationReservations.Remove(current);
            _accommodationReservations.Insert(index, accommodationReservation);
            _serializer.ToCsv(FilePath, _accommodationReservations);
            return accommodationReservation;
        }

        public int NextId()
        {
            _accommodationReservations = _serializer.FromCsv(FilePath);
            if (_accommodationReservations.Count < 1)
            {
                return 1;
            }
            return _accommodationReservations.Max(c => c.Id) + 1;
        }

        public void Delete(AccommodationReservation accommodationReservation)
        {
            _accommodationReservations = _serializer.FromCsv(FilePath);
            AccommodationReservation founded = _accommodationReservations.Find(c => c.Id == accommodationReservation.Id);
            _accommodationReservations.Remove(founded);
            _serializer.ToCsv(FilePath, _accommodationReservations);
        }

        public List<AccommodationReservation> GetByAccommodation(Accommodation accommodation)
        {
            _accommodationReservations = _serializer.FromCsv(FilePath);
            return _accommodationReservations.FindAll(a => a.Accommodation.Id == accommodation.Id);
        }

        // TODO : eliminate next two methods by replacing them with GetByUser and moving the filtering logic to ViewModel
        public List<AccommodationReservation> GetByUserAndDate(User user, DateOnly date)
        {
            _accommodationReservations = _serializer.FromCsv(FilePath);//d u
            return _accommodationReservations.FindAll(a => a.Guest.Id == user.Id && a.CheckoutDate >= date);
        }

        public List<AccommodationReservation> GetByUserAndDateEnded(User user, DateOnly date)
        {
            _accommodationReservations = _serializer.FromCsv(FilePath);//d u
            return _accommodationReservations.FindAll(a => a.Guest.Id == user.Id && a.CheckoutDate < date && a.CheckoutDate.AddDays(5) >= date);
        }

        internal AccommodationReservation GetById(int id)
        {
            _accommodationReservations = _serializer.FromCsv(FilePath);
            return _accommodationReservations.FirstOrDefault(a => a.Id == id);
        }
    }
}
