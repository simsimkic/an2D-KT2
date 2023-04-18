using System;
using SimsProject.Serializer;

namespace SimsProject.Domain.Model
{
    public class Image : ISerializable
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int ParentId { get; set; }

        public Image() { }

        public Image(int id, string url, int parentId)
        {
            Id = id;
            Url = url;
            ParentId = parentId;
        }

        public string[] ToCsv()
        {
            string[] csvValues = { Id.ToString(), Url, ParentId.ToString() };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Url = values[1];
            ParentId = Convert.ToInt32(values[2]);
        }
    }
}
