using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimsProject.Serializer;

namespace SimsProject.Domain.Model
{
    public class TourReview : ISerializable
    {

        public int Id { get; set; }
        public int GuideKnowledge { get; set; }
        public int GuideLanguage { get; set; }
        public int TourInterestigness { get; set; }
        public string Comment { get; set; }
        public TourReservation Reservation { get; set; }
        public List<Image> Images { get; set; }
        public User Guest { get; set; }
        public Tour Tour { get; set; }
        public TourDate TourDate { get; set; }
        public Image Cover { get; set; }
        public CheckPoint CheckPoint { get; set; }

        private bool _isValid;
        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged("IsValid");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TourReview() { }

        public TourReview(int guideKnowledge, int guideLanguage, int tourInterestigness,string comment, User guest, TourReservation reservation, List<Image> images, TourDate tourDate, Tour tour)
        {
            GuideKnowledge = guideKnowledge;
            GuideLanguage = guideLanguage;
            TourInterestigness = tourInterestigness;
            Comment = comment;
            Guest = guest;
            Reservation = reservation;
            Images = images;
            Tour = tour;
            TourDate = tourDate;
            Cover = images[0];
            CheckPoint = new CheckPoint();
            IsValid = true;
        }

        public string[] ToCsv()
        {
            string[] csvValues = { Id.ToString(), GuideKnowledge.ToString(), GuideLanguage.ToString(), TourInterestigness.ToString(), Comment, Guest.Id.ToString(), Reservation.Id.ToString(), TourDate.Id.ToString(), Tour.Id.ToString(), CheckPoint.Id.ToString(), IsValid.ToString() };
            return csvValues;
        }

        public void FromCsv(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            GuideKnowledge = Convert.ToInt32(values[1]);
            GuideLanguage = Convert.ToInt32(values[2]);
            TourInterestigness = Convert.ToInt32(values[3]);
            Comment = values[4];
            Guest = new User() { Id = Convert.ToInt32(values[5]) };
            Reservation = new TourReservation() { Id = Convert.ToInt32(values[6]) };
            Tour = new Tour() { Id = Convert.ToInt32(values[7]) };
            TourDate = new TourDate() { Id = Convert.ToInt32(values[7]) };
            CheckPoint = new CheckPoint() { Id = Convert.ToInt32(values[9]) };
            IsValid = bool.Parse(values[10]);
        }
    }
}
