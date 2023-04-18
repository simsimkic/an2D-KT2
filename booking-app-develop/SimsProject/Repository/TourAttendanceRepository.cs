using System.Collections.Generic;
using System.Linq;
using SimsProject.Domain.Model;
using SimsProject.Serializer;

namespace SimsProject.Repository
{
    public class TourAttendanceRepository
    {
        private const string FilePath = "../../../Resources/Data/tourAttendances.csv";

        private readonly Serializer<TourAttendance> _serializer;
        private List<TourAttendance> _attendances;

        public TourAttendanceRepository()
        {
            _serializer = new Serializer<TourAttendance>();
            _attendances = _serializer.FromCsv(FilePath);
        }

        public List<TourAttendance> GetAll()
        {
            List<TourAttendance> notifications = _serializer.FromCsv(FilePath);
            return notifications;
        }

        public int NextId()
        {
            _attendances = GetAll();
            if (_attendances.Count < 1)
            {
                return 1;
            }
            return _attendances.Max(c => c.Id) + 1;
        }

        public TourAttendance Save(Tour tour, User user, Presence isPresent, CheckPoint checkPoint)
        {
            _attendances = GetAll();
            var id = NextId();
            TourAttendance attendance = new(id, tour, user, isPresent, checkPoint);
            _attendances.Add(attendance);
            _serializer.ToCsv(FilePath, _attendances);

            return attendance;
        }

        public void Update(TourAttendance notification)
        {
            _attendances = GetAll();
            TourAttendance currentNotification = _attendances.Find(n => n.Id == notification.Id);

            currentNotification.Present = notification.Present;
            currentNotification.CheckPoint = notification.CheckPoint;
            var index = _attendances.IndexOf(currentNotification);

            _attendances.Remove(currentNotification);
            _attendances.Insert(index, currentNotification);
            _serializer.ToCsv(FilePath, _attendances);
        }

        internal TourAttendance GetByGuest(User guest)
        {
            _attendances = GetAll();
            return _attendances.FirstOrDefault(n => n.User.Id == guest.Id);
        }
    }
}