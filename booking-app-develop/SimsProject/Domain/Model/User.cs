using System;
using System.Data;
using SimsProject.Serializer;

namespace SimsProject.Domain.Model
{
    public enum UserType
    {
        Owner, Guide, Guest1, Guest2
    }

    public class User : ISerializable
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UserType Type { get; set; }
        public bool IsSuperUser { get; set; }

        public User() { }

        public User(string username, string password, UserType type)
        {
            Username = username;
            Password = password;
            Type = type;
            IsSuperUser = false;
        }

        public string[] ToCsv()
        {
            string[] csvValues = { Id.ToString(), Username, Password, ((int)Type).ToString(), IsSuperUser.ToString() };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Username = values[1];
            Password = values[2];
            Type = (UserType)Convert.ToInt32(values[3]);
            IsSuperUser = Convert.ToBoolean(values[4]);
        }
    }
}
