using System;
using System.Globalization;
using SimsProject.Serializer;

namespace SimsProject.Domain.Model
{
    public class TourVoucher : ISerializable
    {
        public int Id { get; set; }
        public Tour Tour { get; set; }
        public User Guest { get; set; }
        public DateTime ValidUntil { get; set; }
        public DateTime UsedOn { get; set; }

        public TourVoucher()
        {
            Id = -1;
            Tour = new Tour();
            Guest = new User();
            ValidUntil = DateTime.MinValue;
            UsedOn = DateTime.MinValue;

        }

        public TourVoucher(int id, User guest, DateTime validUntil)
        {
            Id = id;
            Tour = new Tour();
            Guest = guest;
            ValidUntil = validUntil;
            UsedOn = DateTime.MinValue;
        }

        public string[] ToCsv()
        {
            string[] csvValues =
            {
                Id.ToString(),
                Tour.Id.ToString(),
                Guest.Id.ToString(),
                ValidUntil.ToString(CultureInfo.CurrentCulture),
                UsedOn.ToString(CultureInfo.CurrentCulture)
            };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = int.Parse(values[0]);
            Tour = new Tour() { Id = Convert.ToInt32(values[1]) };
            Guest = new User() { Id = Convert.ToInt32(values[2]) };
            ValidUntil = DateTime.Parse(values[3]);
            UsedOn = DateTime.Parse(values[4]);
        }
    }
}
