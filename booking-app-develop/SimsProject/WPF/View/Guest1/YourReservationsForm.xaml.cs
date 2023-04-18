using System;
using System.Collections.ObjectModel;
using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.Repository;

namespace SimsProject.WPF.View.Guest1
{
    /// <summary>
    /// Interaction logic for YourReservationsForm.xaml
    /// </summary>
    public partial class YourReservationsForm : Window
    {
        public static AccommodationReservation SelectedReservation { get; set; }
        public static ObservableCollection<AccommodationReservation> Reservations { get; set; }
        public User CurrentUser { get; set; }
        public DateOnly CurrentDate { get; set; }
        private readonly AccommodationReservationRepository _accommodationReservationRepository;
        private readonly AccommodationRepository _accommodationRepository;
        private readonly LocationRepository _locationRepository;
        private readonly AccommodationTypeRepository _accommodationTypeRepository;
        private readonly UserRepository _userRepository;
        private readonly ImageRepository _imageRepository;
        public YourReservationsForm(User currentUser)
        {
            InitializeComponent();
            DataContext = this;

            _accommodationReservationRepository = new AccommodationReservationRepository();
            _accommodationRepository = new AccommodationRepository();
            _locationRepository = new LocationRepository();
            _accommodationTypeRepository = new AccommodationTypeRepository();
            _userRepository = new UserRepository();
            _imageRepository = new ImageRepository("accommodationImages.csv");

            CurrentUser = currentUser;
            CurrentDate = DateOnly.FromDateTime(DateTime.Today);
            Reservations = new ObservableCollection<AccommodationReservation>(_accommodationReservationRepository.GetByUserAndDate(CurrentUser,CurrentDate));
            UpdateTable();
        }
        private void UpdateTable()
        {
            Reservations.Clear();
            foreach (var r in _accommodationReservationRepository.GetByUserAndDate(CurrentUser, CurrentDate))
            {
                if (!r.IsCanceled)
                {
                    Reservations.Add(r);
                }
            }
            PopulateReservations();
        }
        private void PopulateReservations()
        {
            foreach (var reservation in Reservations)
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

        private void CancelReservationClick(object sender, RoutedEventArgs e)
        {
            if (SelectedReservation != null)
            {
                if(CurrentDate < SelectedReservation.ArrivalDate.AddDays(-SelectedReservation.Accommodation.MinDaysBeforeCancellation))
                {
                    AccommodationReservation canceledReservation = new(SelectedReservation);
                    _accommodationReservationRepository.Update(canceledReservation);
                    MessageBox.Show("Canceled.");
                    UpdateTable();
                }
                else
                {
                    MessageBox.Show("Now is to late to cancel.");
                }
            }
            else
            {
                MessageBox.Show("Select reservation.");
            }
        }

        private void RequestMoveClick(object sender, RoutedEventArgs e)
        {
            if (SelectedReservation != null)
            {
                if (CurrentDate < SelectedReservation.ArrivalDate.AddDays(-SelectedReservation.Accommodation.MinDaysBeforeCancellation))
                {
                    RequestReservationMoveForm form = new(SelectedReservation, CurrentUser);
                    form.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Now is to late to request move.");
                }
            }
            else
            {
                MessageBox.Show("Select reservation.");
            }
        }

        private void ViewStatusClick(object sender, RoutedEventArgs e)
        {
            StatusOfMoveRequestsForm form = new(CurrentUser);
            form.ShowDialog();
        }
    }
}
