using System;
using SimsProject.Serializer;

namespace SimsProject.Domain.Model
{
    public class TourDate : ISerializable
    {
        public int Id { get; set; }
        public Tour Tour { get; set; }
        public DateTime? Date { get; set; }
        public bool HasEnded { get; set; }

        public TourDate()
        {
            Id = -1;
            Tour = new Tour();
            Date = DateTime.MinValue;
            HasEnded = false;
        }

        public TourDate(int id, Tour tour, DateTime? date, bool hasEnded)
        {
            Id = id;
            Tour = tour;
            Date = date;
            HasEnded = hasEnded;
        }

        public string[] ToCsv()
        {
            string[] csvValues =
            {
                Id.ToString(),
                Tour.Id.ToString(),
                Date.ToString(),
                HasEnded.ToString()
            };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = int.Parse(values[0]);
            Tour = new Tour() { Id = Convert.ToInt32(values[1]) };
            Date = DateTime.Parse(values[2]);
            HasEnded = bool.Parse(values[3]);
        }

    }
}
