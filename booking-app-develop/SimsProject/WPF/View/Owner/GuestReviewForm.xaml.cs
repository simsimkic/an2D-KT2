using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.Repository;

namespace SimsProject.WPF.View.Owner
{
    /// <summary>
    /// Interaction logic for GuestReviewForm.xaml
    /// </summary>
    public partial class GuestReviewForm : Window, INotifyPropertyChanged
    {
        public AccommodationReservation Reservation { get; set; }
        public User AccommodationOwner { get; set; }
        public User Guest { get; set; }

        private readonly GuestReviewRepository _repository;

        private int _cleanlinessGrade;
        private int _observanceGrade;
        private string _comment;
        public int CleanlinessGrade
        {
            get => _cleanlinessGrade;
            set
            {
                if (value != _cleanlinessGrade)
                {
                    _cleanlinessGrade = value;
                    OnPropertyChanged();
                }
            }
        }
        public int ObservanceGrade
        {
            get => _observanceGrade;
            set
            {
                if (value != _observanceGrade)
                {
                    _observanceGrade = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Comment
        {
            get => _comment;
            set
            {
                if (value != _comment)
                {
                    _comment = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public GuestReviewForm(AccommodationReservation reservation)
        {
            InitializeComponent();
            DataContext = this;
            Reservation = reservation;
            AccommodationOwner = reservation.Accommodation.Owner;
            Guest = reservation.Guest;

            _repository = new GuestReviewRepository();

            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            CleanlinessGrade = 1;
            ObservanceGrade = 1;
        }

        private void Review()
        {
            string commentWithoutNewline = Comment?.Replace(Environment.NewLine, "^") ?? "";
            GuestReview newGuestReview = new(CleanlinessGrade, ObservanceGrade, commentWithoutNewline, AccommodationOwner, Guest, Reservation);
            GuestReview savedGuestReview = _repository.Save(newGuestReview);
            savedGuestReview.Comment = Comment;
            OwnerOverview.GuestReviews.Add(savedGuestReview);

            Close();
        }
        private void ExecuteReview(object sender, RoutedEventArgs e)
        {
            Review();
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
