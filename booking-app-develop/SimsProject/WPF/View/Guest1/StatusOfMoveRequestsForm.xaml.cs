using System.Collections.ObjectModel;
using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.Repository;

namespace SimsProject.WPF.View.Guest1
{
    /// <summary>
    /// Interaction logic for StatusOfMoveRequestsForm.xaml
    /// </summary>

    public partial class StatusOfMoveRequestsForm : Window
    {
        public static ObservableCollection<AccommodationReservationReschedule> Requests { get; set; }
        public User CurrentUser { get; set; }
        private readonly AccommodationReservationRescheduleRepository _accommodationReservationMoveRequestRepository;
        private readonly AccommodationReservationRepository _accommodationReservationRepository;
        private readonly AccommodationRepository _accommodationRepository;
        private readonly LocationRepository _locationRepository;
        private readonly AccommodationTypeRepository _accommodationTypeRepository;
        private readonly UserRepository _userRepository;
        private readonly ImageRepository _imageRepository;
        public StatusOfMoveRequestsForm(User currentUser)
        {
            InitializeComponent();
            DataContext = this;

            _accommodationReservationMoveRequestRepository = new AccommodationReservationRescheduleRepository();
            _accommodationReservationRepository = new AccommodationReservationRepository();
            _accommodationRepository = new AccommodationRepository();
            _locationRepository = new LocationRepository();
            _accommodationTypeRepository = new AccommodationTypeRepository();
            _userRepository = new UserRepository();
            _imageRepository = new ImageRepository("accommodationImages.csv");

            CurrentUser = currentUser;
            Requests = new ObservableCollection<AccommodationReservationReschedule>(_accommodationReservationMoveRequestRepository.GetByGuest(CurrentUser));
            UpdateTable();
        }
        private void UpdateTable()
        {
            Requests.Clear();
            foreach (var r in _accommodationReservationMoveRequestRepository.GetByGuest(CurrentUser))
            {
                Requests.Add(r);
            }
            PopulateRequests();
        }
        private void PopulateRequests()
        {
            foreach (var request in Requests)
            {
                AccommodationReservation reservation = _accommodationReservationRepository.GetById(request.Reservation.Id);
                Accommodation accommodation = _accommodationRepository.GetById(reservation.Accommodation.Id);
                accommodation.Location = _locationRepository.GetById(accommodation.Location.Id);
                accommodation.Type = _accommodationTypeRepository.GetById(accommodation.Type.Id);
                accommodation.Owner = _userRepository.GetById(accommodation.Owner.Id);
                accommodation.Images = _imageRepository.GetByParentId(accommodation.Id);
                accommodation.Cover = accommodation.Images[0];
                reservation.Accommodation = accommodation;
                request.Reservation= reservation;
            }
        }
    }
}
