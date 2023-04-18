using System;
using System.ComponentModel;
using SimsProject.Serializer;

namespace SimsProject.Domain.Model
{
    public enum Presence
    {
        NotPresent,
        GuideConfirmed,
        GuestConfirmed,
        GuestNotPresent
    }

    public class TourAttendance : ISerializable, INotifyPropertyChanged
    {
        public int Id { get; set; }
        public Tour Tour { get; set; }
        public User User { get; set; }

        private Presence _present;
        public Presence Present
        {
            get => _present;
            set
            {
                if (_present != value)
                {
                    _present = value;
                    OnPropertyChanged("Present");
                }
            }
        }
        private CheckPoint _checkPoint;
        public CheckPoint CheckPoint
        {
            get => _checkPoint;
            set
            {
                if (_checkPoint != value)
                {
                    _checkPoint = value;
                    OnPropertyChanged("CheckPoint");
                }
            }
        }

        public TourAttendance()
        {
        }

        public TourAttendance(int id, Tour tour, User user, Presence isPresent, CheckPoint checkPoint)
        {
            Id = id;
            Tour = tour;
            User = user;
            Present = isPresent;
            CheckPoint = checkPoint;
        }

        public string[] ToCsv()
        {
            string[] csvValues =
            {
                Id.ToString(),
                Tour.Id.ToString(),
                User.Id.ToString(),
                ((int)Present).ToString(),
                CheckPoint.Id.ToString()
            };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = int.Parse(values[0]);
            Tour = new Tour() { Id = Convert.ToInt32(values[1]) };
            User = new User() { Id = Convert.ToInt32(values[2]) };
            Present = (Presence)Convert.ToInt32(values[3]);
            CheckPoint = new CheckPoint() { Id = Convert.ToInt32(values[4]) };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}