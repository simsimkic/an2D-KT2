using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SimsProject.Serializer
{
    class Serializer<T> where T: ISerializable, new()
    {
        private const char Delimiter = '|';

        public void ToCsv(string fileName, List<T> objects)
        {
            StringBuilder csv = new();

            foreach(T obj in objects)
            {
                string line = string.Join(Delimiter.ToString(), obj.ToCsv());
                csv.AppendLine(line);
            }

            File.WriteAllText(fileName, csv.ToString());

        }

        public List<T> FromCsv(string fileName)
        {
            List<T> objects = new();

            foreach(string line in File.ReadLines(fileName))
            {
                string[] csvValues = line.Split(Delimiter);
                T obj = new();
                obj.FromCsv(csvValues);
                objects.Add(obj);
            }

            return objects;
        }
    }
}
