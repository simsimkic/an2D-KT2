using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using SimsProject.Domain.Model;
using SimsProject.Repository;
using Image = SimsProject.Domain.Model.Image;
using MessageBox = System.Windows.MessageBox;

namespace SimsProject.WPF.View.Owner
{
    /// <summary>
    /// Interaction logic for OwnerOverview.xaml
    /// </summary>
    ///
    public partial class OwnerOverview : Window, INotifyPropertyChanged
    {
        public static ObservableCollection<Accommodation> Accommodations { get; set; }
        public static ObservableCollection<AccommodationReservation> Reservations { get; set; }
        public static ObservableCollection<AccommodationReservation> UpcomingReservations { get; set; }
        public static ObservableCollection<AccommodationReservation> ActiveReservations { get; set; }
        public static ObservableCollection<AccommodationReservation> PastReservations { get; set; }
        public static ObservableCollection<GuestReview> GuestReviews { get; set; }
        public static ObservableCollection<AccommodationReview> AccommodationReviews { get; set; }
        public static ObservableCollection<AccommodationReview> FilteredAccommodationReviews { get; set; }
        public static ObservableCollection<AccommodationReservationReschedule> RescheduleRequests { get; set; }

        public Accommodation SelectedAccommodation { get; set; }
        public AccommodationReservation SelectedPastReservation { get; set; }
        public GuestReview SelectedGuestReview { get; set; }
        public AccommodationReservationReschedule SelectedRequest { get; set; }
        public User AccommodationOwner { get; set; }

        private const int ReviewPeriodDays = 5;
        private const int MinSuperOwnerReviewCount = 5;
        private const double MinSuperOwnerRating = 4.5;

        private NotifyIcon _notifyIcon;
        private int _selectedImageIndex;

        private Image _selectedImage;
        public Image SelectedImage
        {
            get => _selectedImage;
            set
            {
                if (value != _selectedImage)
                {
                    _selectedImage = value;
                    OnPropertyChanged();
                }
            }
        }

        private AccommodationRepository _accommodationRepository;
        private AccommodationReservationRepository _reservationRepository;
        private GuestReviewRepository _guestReviewRepository;
        private AccommodationReviewRepository _accommodationReviewRepository;
        private AccommodationReservationRescheduleRepository _requestRepository;
        private LocationRepository _locationRepository;
        private AccommodationTypeRepository _accommodationTypeRepository;
        private UserRepository _userRepository;
        private ImageRepository _imageRepository;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public OwnerOverview(User owner)
        {
            InitializeComponent();
            DataContext = this;
            AccommodationOwner = owner;

            InitializeRepositories();
            InitializeCollections();
            InitializeNotification();
            PopulateAll();
            FilterReservations();
            UpdateSuperOwnerStatus();
            UpdateRescheduleDatesAvailability();
            NotifyUser();
        }


        private void InitializeRepositories()
        {
            _accommodationRepository = new AccommodationRepository();
            _reservationRepository = new AccommodationReservationRepository();
            _guestReviewRepository = new GuestReviewRepository();
            _accommodationReviewRepository = new AccommodationReviewRepository();
            _requestRepository = new AccommodationReservationRescheduleRepository();
            _locationRepository = new LocationRepository();
            _accommodationTypeRepository = new AccommodationTypeRepository();
            _userRepository = new UserRepository();
            _imageRepository = new ImageRepository("accommodationImages.csv");
        }

        private void InitializeCollections()
        {
            Accommodations = new ObservableCollection<Accommodation>(_accommodationRepository.GetByOwner(AccommodationOwner));
            Reservations = GetAllReservations();
            GuestReviews = new ObservableCollection<GuestReview>(_guestReviewRepository.GetByOwner(AccommodationOwner));
            AccommodationReviews = new ObservableCollection<AccommodationReview>(_accommodationReviewRepository.GetByOwner(AccommodationOwner));
            FilteredAccommodationReviews = new ObservableCollection<AccommodationReview>(FilterAccommodationReviews());
            UpcomingReservations = new ObservableCollection<AccommodationReservation>();
            ActiveReservations = new ObservableCollection<AccommodationReservation>();
            PastReservations = new ObservableCollection<AccommodationReservation>();
            RescheduleRequests = new ObservableCollection<AccommodationReservationReschedule>(_requestRepository.GetByOwner(AccommodationOwner).Where(request => request.RequestStatus == Status.Waiting));
        }

        private List<AccommodationReview> FilterAccommodationReviews()
        {
            return (from accommodationReview in AccommodationReviews
                    let isReviewed = _guestReviewRepository.Exists(accommodationReview.Reservation)
                    where isReviewed
                    select accommodationReview).ToList();
        }

        private void InitializeNotification()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = new Icon("../../../Resources/Images/notification.ico"),
                Visible = true
            };
        }

        private void PopulateAll()
        {
            PopulateAccommodations();
            PopulateAccommodationReservations();
            PopulateGuestReviews();
            PopulateAccommodationReviews();
            PopulateRescheduleRequests();
        }

        private ObservableCollection<AccommodationReservation> GetAllReservations()
        {
            ObservableCollection<AccommodationReservation> accommodationReservations = new();
            foreach (var accommodation in Accommodations)
            {
                var reservations = _reservationRepository.GetByAccommodation(accommodation);
                foreach (var reservation in reservations)
                {
                    accommodationReservations.Add(reservation);
                }
            }
            return accommodationReservations;
        }

        private void PopulateAccommodations()
        {
            foreach (var accommodation in Accommodations)
            {
                PopulateAccommodation(accommodation);
            }
        }

        private void PopulateAccommodation(Accommodation accommodation)
        {
            accommodation.Location = _locationRepository.GetById(accommodation.Location.Id);
            accommodation.Type = _accommodationTypeRepository.GetById(accommodation.Type.Id);
            accommodation.Owner = _userRepository.GetById(accommodation.Owner.Id);
            accommodation.Images = _imageRepository.GetByParentId(accommodation.Id);
            accommodation.Cover = accommodation.Images[0];
        }

        private void PopulateAccommodationReservations()
        {
            foreach (var reservation in Reservations)
            {
                PopulateAccommodationReservation(reservation);
            }
        }

        private void PopulateAccommodationReservation(AccommodationReservation reservation)
        {
            reservation.Accommodation = _accommodationRepository.GetById(reservation.Accommodation.Id);
            PopulateAccommodation(reservation.Accommodation);
            reservation.Guest = _userRepository.GetById(reservation.Guest.Id);
        }

        private void PopulateGuestReviews()
        {
            foreach (var guestReview in GuestReviews)
            {
                PopulateGuestReview(guestReview);
            }
        }

        private void PopulateGuestReview(GuestReview guestReview)
        {
            guestReview.Comment = guestReview.Comment.Replace("^", Environment.NewLine);
            guestReview.Guest = _userRepository.GetById(guestReview.Guest.Id);
            guestReview.Owner = _userRepository.GetById(guestReview.Owner.Id);
            guestReview.Reservation = _reservationRepository.GetById(guestReview.Reservation.Id);
            guestReview.Reservation.Accommodation = _accommodationRepository.GetById(guestReview.Reservation.Accommodation.Id);
            PopulateAccommodation(guestReview.Reservation.Accommodation);
        }

        private void PopulateAccommodationReviews()
        {
            foreach (var accommodationReview in AccommodationReviews)
            {
                PopulateAccommodationReview(accommodationReview);
            }
        }

        private void PopulateAccommodationReview(AccommodationReview accommodationReview)
        {
            accommodationReview.Comment = accommodationReview.Comment.Replace("^", Environment.NewLine);
            accommodationReview.Guest = _userRepository.GetById(accommodationReview.Guest.Id);
            accommodationReview.Owner = _userRepository.GetById(accommodationReview.Owner.Id);
            accommodationReview.Reservation = _reservationRepository.GetById(accommodationReview.Reservation.Id);
            accommodationReview.Reservation.Accommodation = _accommodationRepository.GetById(accommodationReview.Reservation.Accommodation.Id);
            PopulateAccommodation(accommodationReview.Reservation.Accommodation);
        }        
        
        private void PopulateRescheduleRequests()
        {
            foreach (var rescheduleRequest in RescheduleRequests)
            {
                PopulateRescheduleRequest(rescheduleRequest);
            }
        }

        private void PopulateRescheduleRequest(AccommodationReservationReschedule rescheduleRequest)
        {
            rescheduleRequest.Reservation = _reservationRepository.GetById(rescheduleRequest.Reservation.Id);
            rescheduleRequest.Guest = _userRepository.GetById(rescheduleRequest.Guest.Id);
            rescheduleRequest.Owner = _userRepository.GetById(rescheduleRequest.Owner.Id);
            rescheduleRequest.Reservation = _reservationRepository.GetById(rescheduleRequest.Reservation.Id);
            rescheduleRequest.Reservation.Accommodation = _accommodationRepository.GetById(rescheduleRequest.Reservation.Accommodation.Id);
            PopulateAccommodation(rescheduleRequest.Reservation.Accommodation);
        }

        private static void FilterReservations()
        {
            foreach (var reservation in Reservations)
            {
                if (reservation.IsCanceled) continue;

                if (IsCurrentlyActive(reservation))
                {
                    ActiveReservations.Add(reservation);

                }
                else if (HasPassed(reservation))
                {
                    PastReservations.Add(reservation);
                }
                else
                {
                    UpcomingReservations.Add(reservation);
                }
            }
        }

        private static bool IsCurrentlyActive(AccommodationReservation reservation)
        {
            var hasStarted = reservation.ArrivalDate.CompareTo(DateOnly.FromDateTime(DateTime.Today)) <= 0;
            return hasStarted && !HasPassed(reservation);
        }

        private static bool HasPassed(AccommodationReservation reservation)
        {
            return reservation.CheckoutDate.CompareTo(DateOnly.FromDateTime(DateTime.Today)) <= 0;
        }

        public bool IsSuperOwner()
        {
            var reviews = _accommodationReviewRepository.GetByOwner(AccommodationOwner);

            if (reviews == null || !reviews.Any()) return false;

            var averageRating = reviews.Average(r => r.OwnerGrade);
            return reviews.Count >= MinSuperOwnerReviewCount && averageRating >= MinSuperOwnerRating;
        }

        public void UpdateSuperOwnerStatus()
        {
            var isSuperOwner = IsSuperOwner();
            if (isSuperOwner != AccommodationOwner.IsSuperUser)
            {
                AccommodationOwner.IsSuperUser = isSuperOwner;
                _userRepository.Update(AccommodationOwner);
            }
        }

        public static bool IsDateAvailable(AccommodationReservationReschedule request)
        {
            foreach (var reservation in Reservations)
            {
                if (reservation.IsCanceled || reservation.Id == request.Reservation.Id) continue;

                if (reservation.ArrivalDate < request.NewCheckoutDate && reservation.CheckoutDate > request.NewArrivalDate)
                {
                    return false;
                }
            }
            return true;
        }

        private void UpdateRescheduleDatesAvailability()
        {
            foreach (var request in RescheduleRequests)
            {
                var isDateAvailable = IsDateAvailable(request);
                if (isDateAvailable != request.NewDatesAvailable)
                {
                    request.NewDatesAvailable = isDateAvailable;
                    _requestRepository.Update(request);
                }
            }
        }

        private void RegisterAccommodation(object sender, RoutedEventArgs e)
        {
            AccommodationRegistrationForm accommodationRegistrationForm = new(AccommodationOwner);
            accommodationRegistrationForm.ShowDialog();
        }

        private void DeleteAccommodation(object sender, RoutedEventArgs e)
        {
            if (SelectedAccommodation != null)
            {
                MessageBoxResult result = ConfirmAction("Delete Accommodation", "delete the accommodation", SelectedAccommodation.Name);
                if (result == MessageBoxResult.Yes)
                {
                    _accommodationRepository.Delete(SelectedAccommodation);
                    _imageRepository.DeleteAllByParentId(SelectedAccommodation.Id);
                    Accommodations.Remove(SelectedAccommodation);
                }
            }
            else
            {
                MessageBox.Show("Choose an accommodation to delete.");
            }
        }

        private void ReviewGuest(object sender, RoutedEventArgs e)
        {
            if (SelectedPastReservation != null)
            {
                if (CanBeReviewed(SelectedPastReservation))
                {
                    GuestReviewForm guestReviewForm = new(SelectedPastReservation);
                    guestReviewForm.ShowDialog();
                    ShowAccommodationReview();
                    ColorReviewableReservations(DgrPastReservation);
                }
            }
            else
            {
                MessageBox.Show("Choose a guest to review from the list of past reservations.");
            }
        }

        private void ShowAccommodationReview()
        {
            var review = AccommodationReviews.Where(r =>
            {
                var isReviewed = _guestReviewRepository.Exists(r.Reservation);
                var isShown = FilteredAccommodationReviews.Contains(r);
                return isReviewed && !isShown;
            });

            var newReview = review.FirstOrDefault();

            if (newReview != null)
            {
                FilteredAccommodationReviews.Add(newReview);
            }
        }

        private void DeleteGuestReview(object sender, RoutedEventArgs e)
        {
            if (SelectedGuestReview != null)
            {
                MessageBoxResult result = ConfirmAction("Delete Guest Review", "delete the guest review", SelectedGuestReview.Guest.Username);
                if (result == MessageBoxResult.Yes)
                {
                    _guestReviewRepository.Delete(SelectedGuestReview);
                    GuestReviews.Remove(SelectedGuestReview);

                    ColorReviewableReservations(DgrPastReservation);
                }
            }
            else
            {
                MessageBox.Show("Choose a guest review to delete.");
            }
        }

        private bool CanBeReviewed(AccommodationReservation reservation, bool showMessageBox = true)
        {
            var isReviewed = _guestReviewRepository.Exists(reservation);
            if (isReviewed)
            {
                if (showMessageBox) MessageBox.Show("Guest already reviewed.");
                return false;
            }

            var isReviewPeriodExpired = reservation.CheckoutDate < DateOnly.FromDateTime(DateTime.Today.AddDays(-ReviewPeriodDays));
            if (isReviewPeriodExpired)
            {
                if (showMessageBox) MessageBox.Show($"Guests can only be reviewed {ReviewPeriodDays} days after their checkout date.");
                return false;
            }

            return true;
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = ConfirmAction("Logout", "logout");
            if (result == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Windows.OfType<Window>()
                    .Where(w => w != this)
                    .ToList()
                    .ForEach(w => w.Close());

                LogInForm logInForm = new();
                logInForm.Show();

                Close();
            }
        }

        private static MessageBoxResult ConfirmAction(string caption, string action, string name = "")
        {
            string sMessageBoxText = name != "" ? $"Are you sure you want to {action} {name}?" : $"Are you sure you want to {action}?";
            string sCaption = caption;

            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            MessageBoxImage icnMessageBox = MessageBoxImage.Question;

            MessageBoxResult result = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            return result;
        }

        private void NotifyUser()
        {
            var message = WriteReviewNotificationMessage();
            if (!string.IsNullOrEmpty(message))
            {
                ShowNotification(message);
            }

            message = WriteCanceledReservationNotificationMessage();
            if (!string.IsNullOrEmpty(message))
            {
                ShowNotification(message);
            }

            message = WriteRescheduleNotificationMessage();
            if (!string.IsNullOrEmpty(message))
            {
                ShowNotification(message);
            }
        }

        private string WriteReviewNotificationMessage()
        {
            List<string> usernames = new();
            foreach (var reservation in PastReservations)
            {
                if (CanBeReviewed(reservation, false))
                {
                    AddUsernameToNotification(usernames, reservation.Guest.Username);
                }
            }

            if (usernames.Any())
            {
                StringBuilder stringBuilder = new("Don't forget to review the following guests: ");
                stringBuilder.Append(string.Join(", ", usernames));
                return stringBuilder.ToString();
            }

            return null;
        }

        private string WriteCanceledReservationNotificationMessage()
        {
            List<string> canceledReservations = new();
            foreach (var reservation in Reservations)
            {
                if (reservation.NotifyOwner)
                {
                    UpdateReservationNotificationStatus(reservation);
                    canceledReservations.Add($"{reservation.Accommodation.Name} ({reservation.Guest.Username})");
                }
            }

            if (canceledReservations.Any())
            {
                StringBuilder stringBuilder = new("These reservations have been canceled: ");
                stringBuilder.Append(string.Join(", ", canceledReservations));
                return stringBuilder.ToString();
            }

            return null;
        }

        private string WriteRescheduleNotificationMessage()
        {
            List<string> requests = new();
            foreach (var request in RescheduleRequests)
            {
                if (request.Notify && request.RequestStatus == Status.Waiting)
                {
                    UpdateRescheduleNotificationStatus(request);
                    requests.Add($"{request.Reservation.Accommodation.Name} ({request.Guest.Username})");
                }
            }

            if (requests.Any())
            {
                StringBuilder stringBuilder = new("You have new reschedule requests for: ");
                stringBuilder.Append(string.Join(", ", requests));
                return stringBuilder.ToString();
            }

            return null;
        }

        private void UpdateReservationNotificationStatus(AccommodationReservation reservation)
        {
            reservation.NotifyOwner = false;
            _reservationRepository.Update(reservation);
        }

        private void UpdateRescheduleNotificationStatus(AccommodationReservationReschedule request)
        {
            request.Notify = false;
            _requestRepository.Update(request);
        }

        private static void AddUsernameToNotification(List<string> usernames, string username)
        {
            if (!usernames.Contains(username))
            {
                usernames.Add(username);
            }
        }

        private void ShowNotification(string message)
        {
            _notifyIcon.BalloonTipTitle = "Guest Review";
            _notifyIcon.BalloonTipText = message;
            _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;

            _notifyIcon.ShowBalloonTip(5000);
        }

        public static void ColorAccommodationAvailability(DataGrid dataGrid)
        {
            foreach (Accommodation accommodation in dataGrid.Items)
            {
                var isTaken = Reservations.Any(r => r.Accommodation.Id == accommodation.Id && IsCurrentlyActive(r));

                var row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(accommodation);

                row.Background = isTaken ? new SolidColorBrush(Colors.IndianRed) : new SolidColorBrush(Colors.DarkSeaGreen);
            }
        }

        public void ColorReviewableReservations(DataGrid dataGrid)
        {
            // BUG FIX
            /*foreach (Reservation reservation in dataGrid.Items)
            {
                var row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(reservation);

                row.Background = CanBeReviewed(reservation, false) ? new SolidColorBrush(Colors.PowderBlue) : new SolidColorBrush(Colors.White);
            }*/
        }

        private void DgrAccommodationsLoaded(object sender, RoutedEventArgs e)
        {
            ColorAccommodationAvailability(DgrAccommodations);
        }

        private void DgrPastReservationLoaded(object sender, RoutedEventArgs e)
        {
            ColorReviewableReservations(DgrPastReservation);
        }

        private void ShowImagesClick(object sender, RoutedEventArgs e)
        {
            _selectedImageIndex = 0;
            SelectedImage = SelectedAccommodation.Images[_selectedImageIndex];
            ImagePopup.IsOpen = true;
        }

        private void ShowNextImageClick(object sender, RoutedEventArgs e)
        {
            _selectedImageIndex = (_selectedImageIndex + 1) % SelectedAccommodation.Images.Count;
            SelectedImage = SelectedAccommodation.Images[_selectedImageIndex];
        }

        private void ShowPreviousImageClick(object sender, RoutedEventArgs e)
        {
            _selectedImageIndex = (_selectedImageIndex + SelectedAccommodation.Images.Count - 1) % SelectedAccommodation.Images.Count;
            SelectedImage = SelectedAccommodation.Images[_selectedImageIndex];
        }

        private void CloseImagesClick(object sender, RoutedEventArgs e)
        {
            ImagePopup.IsOpen = false;
        }

        private void AcceptRescheduleClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = ConfirmAction("Reschedule Reservation", "reschedule the reservation", SelectedRequest.Reservation.Accommodation.Name);
            if (result == MessageBoxResult.Yes)
            {
                AcceptReschedule(SelectedRequest);
                RescheduleRequests.Remove(SelectedRequest);
            }
        }

        private void AcceptReschedule(AccommodationReservationReschedule request)
        {
            RescheduleReservation(request);
            AcceptRequest(request);
        }

        private void RescheduleReservation(AccommodationReservationReschedule request)
        {
            foreach (var reservation in Reservations)
            {
                if(reservation.Id == request.Reservation.Id) continue;

                if (reservation.ArrivalDate < request.NewCheckoutDate && reservation.CheckoutDate > request.NewArrivalDate)
                {
                    _reservationRepository.Delete(reservation);
                }
            }

            UpdateReservation(request);
        }

        private void UpdateReservation(AccommodationReservationReschedule request)
        {
            var rescheduledReservation = request.Reservation;
            rescheduledReservation.ArrivalDate = request.NewArrivalDate;
            rescheduledReservation.CheckoutDate = request.NewCheckoutDate;
            rescheduledReservation.StayLength = (request.NewCheckoutDate.ToDateTime(new TimeOnly()) - request.NewArrivalDate.ToDateTime(new TimeOnly())).Days;

            _reservationRepository.Update(rescheduledReservation);
        }

        private void AcceptRequest(AccommodationReservationReschedule request)
        {
            request.RequestStatus = Status.Accepted;
            request.Notify = true;

            _requestRepository.Update(request);
        }

        private void RejectRescheduleClick(object sender, RoutedEventArgs e)
        {
            var dialog = new ReschedulePrompt();
            if (dialog.ShowDialog() == true)
            {
                RejectRequest(SelectedRequest, dialog.ResponseText);
                RescheduleRequests.Remove(SelectedRequest);
            }
        }

        private void RejectRequest(AccommodationReservationReschedule request, string comment)
        {
            request.RequestStatus = Status.Declined;
            request.Notify = true;
            request.Comment = comment;

            _requestRepository.Update(request);
        }
    }
}
