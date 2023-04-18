using SimsProject.Serializer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SimsProject.Domain.Model
{
    public class AccommodationReservation : ISerializable
    {
        public int Id { get; set; }
        public Accommodation Accommodation { get; set; }
        public User Guest { get; set; }
        public DateOnly ArrivalDate { get; set; }
        public DateOnly CheckoutDate { get; set; }
        public int StayLength { get; set; }
        public int GuestNumber { get; set; }
        public bool IsCanceled { get; set; } 
        public bool NotifyOwner { get; set; }
        public AccommodationReservation() { }

        public AccommodationReservation(AccommodationReservation reservation) // za otkazivanje
        {
            Id = reservation.Id;
            Accommodation = reservation.Accommodation;
            Guest = reservation.Guest;
            ArrivalDate = reservation.ArrivalDate;
            StayLength = reservation.StayLength;
            CheckoutDate = reservation.ArrivalDate.AddDays(reservation.StayLength);
            GuestNumber = reservation.GuestNumber;
            IsCanceled = true;
            NotifyOwner = true;
        }

        public AccommodationReservation(Accommodation accommodation, User guest, DateOnly arrivalDate, int stayLength, int guestNumber)
        {
            Accommodation = accommodation;
            Guest = guest;
            ArrivalDate = arrivalDate;
            StayLength = stayLength;
            CheckoutDate = arrivalDate.AddDays(stayLength);
            GuestNumber = guestNumber;
            IsCanceled = false;
            NotifyOwner = false;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Accommodation = new Accommodation() { Id = Convert.ToInt32(values[1]) };
            Guest = new User() { Id = Convert.ToInt32(values[2]) };
            ArrivalDate = DateOnly.Parse(values[3]);
            StayLength = Convert.ToInt32(values[4]);
            CheckoutDate = ArrivalDate.AddDays(StayLength - 1); //
            GuestNumber = Convert.ToInt32(values[5]);
            IsCanceled = Convert.ToBoolean(values[6]);
            NotifyOwner = Convert.ToBoolean(values[7]);
        }

        public string[] ToCsv()
        {
            string[] csvValues = {  Id.ToString(),
                                    Accommodation.Id.ToString(),
                                    Guest.Id.ToString(),
                                    ArrivalDate.ToString(),
                                    StayLength.ToString(),
                                    GuestNumber.ToString(),
                                    IsCanceled.ToString(),
                                    NotifyOwner.ToString()
                                    };
            return csvValues;
        }
    }
}
