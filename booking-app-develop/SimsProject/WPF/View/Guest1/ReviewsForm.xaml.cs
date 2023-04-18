using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SimsProject.Domain.Model;
using SimsProject.Repository;

namespace SimsProject.WPF.View.Guest1
{
    /// <summary>
    /// Interaction logic for ReviewsForm.xaml
    /// </summary>
    public partial class ReviewsForm : Window
    {
        public User CurrentUser { get; set; }
        public DateOnly CurrentDate { get; set; }
        private readonly AccommodationReservationRepository _accommodationReservationRepository;
        public static AccommodationReservation SelectedAccommodationReservation { get; set; }
        public static ObservableCollection<AccommodationReservation> AccommodationReservations { get; set; }
        public static ObservableCollection<GuestReview> AllGuestReviews { get; set; }
        public static ObservableCollection<GuestReview> GuestReviews { get; set; }

        private readonly AccommodationTypeRepository _accommodationTypeRepository;
        private readonly AccommodationReviewRepository _accommodationReviewRepository;
        private readonly AccommodationRepository _accommodationRepository;
        private readonly LocationRepository _locationRepository;
        private readonly ImageRepository _imageRepository;
        private readonly UserRepository _userRepository;
        private readonly GuestReviewRepository _guestReviewRepository;
        public ReviewsForm(User currentUser)
        {
            InitializeComponent();
            DataContext = this;

            _accommodationReservationRepository = new AccommodationReservationRepository();
            _accommodationRepository = new AccommodationRepository();
            _locationRepository = new LocationRepository();
            _accommodationReviewRepository = new AccommodationReviewRepository();
            _accommodationTypeRepository = new AccommodationTypeRepository();
            _userRepository = new UserRepository();
            _imageRepository = new ImageRepository("accommodationImages.csv");
            _guestReviewRepository = new GuestReviewRepository();

            CurrentUser = currentUser;
            CurrentDate = DateOnly.FromDateTime(DateTime.Today);
            AccommodationReservations = new ObservableCollection<AccommodationReservation>();
            AllGuestReviews = new ObservableCollection<GuestReview>(_guestReviewRepository.GetByGuest(CurrentUser));
            GuestReviews = new ObservableCollection<GuestReview>();

            PopulateReviews();
            FilterGuestReviews();

            FilterAccommodationReviews();
            PopulateReservations();

        }

        private void FilterGuestReviews()
        {
            foreach (var guestReview in AllGuestReviews)
            {
                var isReviewed = _accommodationReviewRepository.Exists(guestReview.Reservation);
                if (isReviewed)
                {
                    GuestReviews.Add(guestReview);
                }
            }
        }

        private void FilterAccommodationReviews()
        {
            foreach (var r in _accommodationReservationRepository.GetByUserAndDateEnded(CurrentUser, CurrentDate))
            {
                if (!r.IsCanceled && !_accommodationReviewRepository.IsReviewed(r))
                {
                    AccommodationReservations.Add(r);
                }
            }
        }

        private void PopulateReservations()
        {
            foreach (var reservation in AccommodationReservations)
            {
                Accommodation accommodation = _accommodationRepository.GetById(reservation.Accommodation.Id);
                accommodation.Location = _locationRepository.GetById(accommodation.Location.Id);
                accommodation.Type = _accommodationTypeRepository.GetById(accommodation.Type.Id);
                accommodation.Owner = _userRepository.GetById(accommodation.Owner.Id);
                accommodation.Images = _imageRepository.GetByParentId(accommodation.Id);
                accommodation.Cover = accommodation.Images[0];
                reservation.Accommodation = accommodation;
            }
        }
        private void PopulateReviews()
        {
            foreach (var reviews in AllGuestReviews)
            {
                AccommodationReservation reservation = _accommodationReservationRepository.GetById(reviews.Reservation.Id);
                Accommodation accommodation = _accommodationRepository.GetById(reservation.Accommodation.Id);
                accommodation.Location = _locationRepository.GetById(accommodation.Location.Id);
                reviews.Reservation = _accommodationReservationRepository.GetById(reviews.Reservation.Id);
                reviews.Reservation.Accommodation = accommodation;
                reviews.Comment = reviews.Comment.Replace("^", Environment.NewLine);
            }
        }
        private void ReviewClick(object sender, RoutedEventArgs e)
        {
            if (SelectedAccommodationReservation != null)
            {
                ReviewAccommodationForm form = new(SelectedAccommodationReservation, CurrentUser);
                form.ShowDialog();
                ShowAccommodationReview();
            }
            else
            {
                MessageBox.Show("Select accommodation to review.");
            }
        }
        private void ShowAccommodationReview()
        {
            var review = AllGuestReviews.Where(r =>
            {
                var isReviewed = _accommodationReviewRepository.Exists(r.Reservation);
                var isShown = GuestReviews.Contains(r);
                return isReviewed && !isShown;
            });

            var newReview = review.FirstOrDefault();

            if (newReview != null)
            {
                GuestReviews.Add(newReview);
            }
        }
    }
}
