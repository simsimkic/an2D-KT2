using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SimsProject.Domain.Model;
using SimsProject.Domain.RepositoryInterface;
using SimsProject.Serializer;

namespace SimsProject.Repository
{
    public class UserRepository : IUserRepository
    {
        private const string FilePath = "../../../Resources/Data/users.csv";

        private readonly Serializer<User> _serializer;

        private List<User> _users;

        public UserRepository()
        {
            _serializer = new Serializer<User>();
            _users = _serializer.FromCsv(FilePath);
        }

        public List<User> GetAll()
        {
            return _serializer.FromCsv(FilePath);
        }

        public User GetById(int id)
        {
            _users = _serializer.FromCsv(FilePath);
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public int NextId()
        {
            _users = _serializer.FromCsv(FilePath);
            if (_users.Count < 1)
            {
                return 1;
            }
            return _users.Max(u => u.Id) + 1;
        }

        public User Save(User user)
        {
            user.Id = NextId();
            _users = _serializer.FromCsv(FilePath);
            _users.Add(user);
            _serializer.ToCsv(FilePath, _users);
            return user;
        }

        public User Update(User user)
        {
            _users = _serializer.FromCsv(FilePath);
            User current = _users.Find(u => u.Id == user.Id);
            int index = _users.IndexOf(current);
            _users.Remove(current);
            _users.Insert(index, user);
            _serializer.ToCsv(FilePath, _users);
            return user;
        }

        public void Delete(User user)
        {
            _users = _serializer.FromCsv(FilePath);
            User found = _users.Find(u => u.Id == user.Id);
            _users.Remove(found);
            _serializer.ToCsv(FilePath, _users);
        }

        public User GetByUsername(string username)
        {
            _users = _serializer.FromCsv(FilePath);
            return _users.FirstOrDefault(u => u.Username == username);
        }
    }
}
