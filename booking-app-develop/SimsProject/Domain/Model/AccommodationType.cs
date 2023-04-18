using System;
using SimsProject.Serializer;

namespace SimsProject.Domain.Model
{
    public class AccommodationType : ISerializable
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public AccommodationType() { }

        public AccommodationType(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public string[] ToCsv()
        {
            string[] csvValues = { Id.ToString(), Name };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Name = values[1];
        }
    }
}
