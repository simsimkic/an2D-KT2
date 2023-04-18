using System;
using System.Collections.Generic;
using SimsProject.Serializer;

namespace SimsProject.Domain.Model
{
    public class Tour : ISerializable
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public int? MaxGuestNumber { get; set; }
        public Location TourLocation { get; set; }
        public int? Duration { get; set; }
        public List<CheckPoint> CheckPoints { get; set; }
        public List<TourDate> TourDates { get; set; }
        public List<Image> Images { get; set; }
        public Image Cover { get; set; }


        public Tour()
        {
            Id = -1;
        }

        public Tour(int id, string name, string description, string language, int? maxGuestNumber, Location tourLocation, int? duration, List<Image> images, User user)
        {
            Id = id;
            Name = name;
            Description = description;
            Language = language;
            MaxGuestNumber = maxGuestNumber;
            TourLocation = tourLocation;
            Duration = duration;
            CheckPoints = new List<CheckPoint>();
            TourDates = new List<TourDate>();
            Images = images;
            Cover = images[0];
            User = user;
        }

        public string[] ToCsv()
        {
            string[] csvValues =
            {
                Id.ToString(),
                Name,
                Description,
                Language,
                MaxGuestNumber.ToString(),
                TourLocation.Id.ToString(),
                Duration.ToString(),
                User.Id.ToString(),
            };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = int.Parse(values[0]);
            Name = values[1];
            Description = values[2];
            Language = values[3];
            MaxGuestNumber = Convert.ToInt32(values[4]);
            TourLocation = new Location() { Id = Convert.ToInt32(values[5]) };
            Duration = Convert.ToInt32(values[6]);
            User = new User() { Id = Convert.ToInt32(values[7]) };
        }
    }
}
