using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimsProject.Repository
{
    public class AccommodationReviewRepository
    {
        private const string FilePath = "../../../Resources/Data/accommodationReviews.csv";

        private readonly Serializer<AccommodationReview> _serializer;

        private List<AccommodationReview> _accommodationReviews;

        public AccommodationReviewRepository()
        {
            _serializer = new Serializer<AccommodationReview>();
            _accommodationReviews = _serializer.FromCsv(FilePath);
        }

        public List<AccommodationReview> GetAll()
        {
            List<AccommodationReview> accommodationReviews = _serializer.FromCsv(FilePath);
            return accommodationReviews;
        }

        public AccommodationReview Save(AccommodationReview accommodationReviews)
        {
            accommodationReviews.Id = NextId();
            _accommodationReviews = _serializer.FromCsv(FilePath);
            _accommodationReviews.Add(accommodationReviews);
            _serializer.ToCsv(FilePath, _accommodationReviews);
            return accommodationReviews;
        }

        public int NextId()
        {
            _accommodationReviews = _serializer.FromCsv(FilePath);
            if (_accommodationReviews.Count < 1)
            {
                return 1;
            }
            return _accommodationReviews.Max(r => r.Id) + 1;
        }

        public void Delete(AccommodationReview accommodationReview)
        {
            _accommodationReviews = _serializer.FromCsv(FilePath);
            AccommodationReview found = _accommodationReviews.Find(r => r.Id == accommodationReview.Id);
            _accommodationReviews.Remove(found);
            _serializer.ToCsv(FilePath, _accommodationReviews);
        }

        public List<AccommodationReview> GetByOwner(User owner)
        {
            _accommodationReviews = _serializer.FromCsv(FilePath);
            return _accommodationReviews.FindAll(r => r.Owner.Id == owner.Id);
        }

        public List<AccommodationReview> GetByGuest(User guest)
        {
            _accommodationReviews = _serializer.FromCsv(FilePath);
            return _accommodationReviews.FindAll(r => r.Guest.Id == guest.Id);
        }

        public AccommodationReview GetByReservation(AccommodationReservation reservation)
        {
            _accommodationReviews = _serializer.FromCsv(FilePath);
            return _accommodationReviews.FirstOrDefault(r => r.Reservation.Id == reservation.Id);
        }

        // TODO : get rid of method by using something like this in ViewModel
        // var isReviewed = _accommodationReviewRepository.Exists(reservation)
        public bool IsReviewed(AccommodationReservation reservation)
        {
            return _accommodationReviews.Any(r => r.Reservation.Id == reservation.Id); //
        }

        public bool Exists(AccommodationReservation reservation)
        {
            _accommodationReviews = _serializer.FromCsv(FilePath);
            return _accommodationReviews.Exists(r => r.Reservation.Id == reservation.Id);
        }
    }
}
