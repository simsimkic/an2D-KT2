using SimsProject.Serializer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SimsProject.Domain.Model
{
    public class TourReservation : ISerializable
    {
        public int Id { get; set; }
        public Tour Tour { get; set; }
        public User Guest { get; set; }

        public int GuestNumber { get; set; }
        public DateTime Date { get; set; }

        public int GuestAge { get; set; }
        public TourReservation() { }

        public TourReservation(Tour tour, User guest, int guestNumber, DateTime date, int age)
        {
            Tour = tour;
            Guest = guest;
            GuestNumber = guestNumber;
            Date = date;
            GuestAge = age;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Tour = new Tour() { Id = Convert.ToInt32(values[1]) };
            Guest = new User() { Id = Convert.ToInt32(values[2]) };

            GuestNumber = Convert.ToInt32(values[3]);
            GuestAge = Convert.ToInt32(values[4]);
            Date = DateTime.Parse(values[5]);


        }

        public string[] ToCsv()
        {
            string[] csvValues = {  Id.ToString(),
                                    Tour.Id.ToString(),
                                    Guest.Id.ToString(),
                                    GuestNumber.ToString(),
                                    GuestAge.ToString(),
                                    Date.ToString()

            };
            return csvValues;
        }
    }
}
