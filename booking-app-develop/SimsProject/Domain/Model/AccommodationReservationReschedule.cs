using SimsProject.Serializer;
using System;

namespace SimsProject.Domain.Model
{
    public enum Status { Waiting, Accepted, Declined };
    public class AccommodationReservationReschedule : ISerializable
    {
        public int Id { get; set; }
        public AccommodationReservation Reservation { get; set; }
        public User Guest { get; set; }
        public User Owner { get; set; }
        public DateOnly NewArrivalDate { get; set; }
        public DateOnly NewCheckoutDate { get; set; }
        public bool NewDatesAvailable { get; set; }
        public Status RequestStatus { get; set; }
        public string Comment { get; set; }
        public bool Notify { get; set; }

        public AccommodationReservationReschedule() { }

        public AccommodationReservationReschedule(AccommodationReservation accommodationReservation, User guest, User owner, DateOnly newArrivalDate, DateOnly newCheckoutDate, Status requestStatus, string comment)
        {
            Reservation = accommodationReservation;
            Guest = guest;
            Owner = owner;
            NewArrivalDate = newArrivalDate;
            NewCheckoutDate = newCheckoutDate;
            NewDatesAvailable = false;
            RequestStatus = requestStatus;
            Comment = comment;
            Notify = true;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Reservation = new AccommodationReservation() { Id = Convert.ToInt32(values[1]) };
            Guest = new User() { Id = Convert.ToInt32(values[2]) };
            Owner = new User() { Id = Convert.ToInt32(values[3]) };
            NewArrivalDate = DateOnly.Parse(values[4]);
            NewCheckoutDate = DateOnly.Parse(values[5]);
            NewDatesAvailable = bool.Parse(values[6]);
            if (values[7] == "Waiting")
                RequestStatus = Status.Waiting;
            if (values[7] == "Accepted")
                RequestStatus = Status.Accepted;
            if (values[7] == "Declined")
                RequestStatus = Status.Declined;
            Comment = values[8];
            Notify = Convert.ToBoolean(values[9]);
        }

        public string[] ToCsv()
        {
            string[] csvValues = {  Id.ToString(),
                                    Reservation.Id.ToString(),
                                    Guest.Id.ToString(),
                                    Owner.Id.ToString(),
                                    NewArrivalDate.ToString(),
                                    NewCheckoutDate.ToString(),
                                    NewDatesAvailable.ToString(),
                                    RequestStatus.ToString(),
                                    Comment,
                                    Notify.ToString()
                                    };
            return csvValues;
        }
    }
}
