using System;
using SimsProject.Serializer;

namespace SimsProject.Domain.Model
{
    public class GuestReview : ISerializable
    {
        public int Id { get; set; }
        public int CleanlinessGrade { get; set; }
        public int ObservanceGrade { get; set; }
        public string Comment { get; set; }
        public User Owner { get; set; }
        public User Guest { get; set; }
        public AccommodationReservation Reservation { get; set; }

        public GuestReview() { }

        public GuestReview(int cleanlinessGrade, int observanceGrade, string comment, User owner, User guest, AccommodationReservation reservation)
        {
            CleanlinessGrade = cleanlinessGrade;
            ObservanceGrade = observanceGrade;
            Comment = comment;
            Owner = owner;
            Guest = guest;
            Reservation = reservation;
        }
        public string[] ToCsv()
        {
            string[] csvValues = { Id.ToString(), CleanlinessGrade.ToString(), ObservanceGrade.ToString(), Comment, Owner.Id.ToString(), Guest.Id.ToString(), Reservation.Id.ToString() };
            return csvValues;
        }
        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            CleanlinessGrade = Convert.ToInt32(values[1]);
            ObservanceGrade = Convert.ToInt32(values[2]);
            Comment = values[3];
            Owner = new User() { Id = Convert.ToInt32(values[4]) };
            Guest = new User() { Id = Convert.ToInt32(values[5]) };
            Reservation = new AccommodationReservation() { Id = Convert.ToInt32(values[6]) };
        }
    }
}
