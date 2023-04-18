using SimsProject.Domain.Model;
using SimsProject.Serializer;
using System.Collections.Generic;
using System.Linq;

namespace SimsProject.Repository
{
    public class GuestReviewRepository
    {
        private const string FilePath = "../../../Resources/Data/guestReviews.csv";

        private readonly Serializer<GuestReview> _serializer;

        private List<GuestReview> _guestReviews;

        public GuestReviewRepository()
        {
            _serializer = new Serializer<GuestReview>();
            _guestReviews = _serializer.FromCsv(FilePath);
        }

        public List<GuestReview> GetAll()
        {
            List<GuestReview> guestReviews = _serializer.FromCsv(FilePath);
            return guestReviews;
        }

        public GuestReview Save(GuestReview guestReview)
        {
            guestReview.Id = NextId();
            _guestReviews = _serializer.FromCsv(FilePath);
            _guestReviews.Add(guestReview);
            _serializer.ToCsv(FilePath, _guestReviews);
            return guestReview;
        }

        public int NextId()
        {
            _guestReviews = _serializer.FromCsv(FilePath);
            if (_guestReviews.Count < 1)
            {
                return 1;
            }
            return _guestReviews.Max(r => r.Id) + 1;
        }

        public void Delete(GuestReview guestReview)
        {
            _guestReviews = _serializer.FromCsv(FilePath);   
            GuestReview found = _guestReviews.Find(r => r.Id == guestReview.Id);
            _guestReviews.Remove(found);
            _serializer.ToCsv(FilePath, _guestReviews);
        }

        public GuestReview Update(GuestReview guestReview)
        {   
            _guestReviews = _serializer.FromCsv(FilePath);
            GuestReview current = _guestReviews.Find(r => r.Id == guestReview.Id);
            int index = _guestReviews.IndexOf(current);
            _guestReviews.Remove(current);
            _guestReviews.Insert(index, guestReview);     
            _serializer.ToCsv(FilePath, _guestReviews);
            return guestReview;
        }

        public List<GuestReview> GetByOwner(User owner)
        {
            _guestReviews = _serializer.FromCsv(FilePath);
            return _guestReviews.FindAll(r => r.Owner.Id == owner.Id);
        }

        public List<GuestReview> GetByGuest(User guest)
        {
            _guestReviews = _serializer.FromCsv(FilePath);
            return _guestReviews.FindAll(r => r.Guest.Id == guest.Id);
        }

        public bool Exists(AccommodationReservation reservation)
        {
            _guestReviews = _serializer.FromCsv(FilePath);
            return _guestReviews.Exists(r => r.Reservation.Id == reservation.Id);
        }
    }
}
