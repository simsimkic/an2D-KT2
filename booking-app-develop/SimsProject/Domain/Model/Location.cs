using SimsProject.Serializer;
using System;

namespace SimsProject.Domain.Model
{
    public class Location : ISerializable
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public Location() { }

        public Location(string city, string country)
        {
            City = city;
            Country = country;
        }

        public string[] ToCsv()
        {
            string[] csvValues = { Id.ToString(), City, Country };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            City = values[1];
            Country = values[2];
        }

        public override string ToString()
        {
            return City + ", " + Country;
        }
    }
}