using System;
using SimsProject.Serializer;

namespace SimsProject.Domain.Model
{
    public class CheckPoint : ISerializable
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int SerialNumber { get; set; }
        public Tour Tour { get; set; }
        public string Name { get; set; }

        public CheckPoint()
        {
            Id = -1;
            Tour = new();
            IsActive = false;
            SerialNumber = 0;
            Name = string.Empty;

        }

        public CheckPoint(int id, bool isActive, Tour tour, int serialNumber, string name)
        {
            Id = id;
            Tour = tour;
            IsActive = isActive;
            SerialNumber = serialNumber;
            Name = name;
        }

        public string[] ToCsv()
        {
            string[] csvValues =
            {
                Id.ToString(),
                Name,
                SerialNumber.ToString(),
                Tour.Id.ToString(),
                IsActive.ToString()
            };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = int.Parse(values[0]);
            Name = values[1];
            SerialNumber = int.Parse(values[2]);
            Tour = new Tour() { Id = Convert.ToInt32(values[3]) };
            IsActive = bool.Parse(values[4]);
        }
    }
}
