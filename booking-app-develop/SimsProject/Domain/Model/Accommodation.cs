using System;
using System.Collections.Generic;
using SimsProject.Serializer;


namespace SimsProject.Domain.Model
{
    public class Accommodation : ISerializable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Location Location { get; set; }
        public AccommodationType Type { get; set; }
        public int MaxGuestNumber { get; set; }
        public int MinReservationDays { get; set; }
        public int MinDaysBeforeCancellation { get; set; }
        public User Owner { get; set; }
        public Image Cover { get; set; }
        public List<Image> Images { get; set; }

        public Accommodation() { }

        public Accommodation(string name, Location location, AccommodationType type, int maxGuestNumber, int minReservationDays, int minDaysBeforeCancellation, User owner, List<Image> images)
        {
            Name = name;
            Location = location;
            Type = type;
            MaxGuestNumber = maxGuestNumber;
            MinReservationDays = minReservationDays;
            MinDaysBeforeCancellation = minDaysBeforeCancellation;
            Owner = owner;
            Cover = images[0];
            Images = images;
        }

        public string[] ToCsv()
        {
            string[] csvValues = { Id.ToString(), Name, Location.Id.ToString(), Type.Id.ToString(), MaxGuestNumber.ToString(), MinReservationDays.ToString(), MinDaysBeforeCancellation.ToString(), Owner.Id.ToString() };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Name = values[1];
            Location = new Location() { Id = Convert.ToInt32(values[2]) };
            Type = new AccommodationType() { Id = Convert.ToInt32(values[3]) };
            MaxGuestNumber = Convert.ToInt32(values[4]);
            MinReservationDays = Convert.ToInt32(values[5]);
            MinDaysBeforeCancellation = Convert.ToInt32(values[6]);
            Owner = new User() { Id = Convert.ToInt32(values[7]) };
        }
    }
}
