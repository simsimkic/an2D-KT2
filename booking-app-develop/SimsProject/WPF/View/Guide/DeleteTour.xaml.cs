using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.Repository;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for DeleteTour.xaml
    /// </summary>
    public partial class DeleteTour : Window
    {
        public ObservableCollection<TourDate> TourDates { get; set; }
        public List<TourReservation> TourReservations { get; set; }
        public List<User> Guests { get; set; }
        public TourDate SelectedDate { get; set; }
        public User LoggedInUser { get; set; }
        public Tour Tour { get; set; }

        private TourReservationRepository _tourReservationRepository;
        private TourVoucherRepository _tourVoucherRepository;
        private CheckPointRepository _checkPointRepository;
        private TourDateRepository _tourDateRepository;
        private LocationRepository _locationRepository;
        private ImageRepository _imageRepository;
        private TourRepository _tourRepository;

        public DeleteTour(Tour tour, User loggedInUser)
        {
            InitializeComponent();
            DataContext = this;

            Tour = tour;
            LoggedInUser = loggedInUser;

            InitializeRepositories();
            InitializeCollections();
            FindTourDates();
            PopulateTour();
        }

        private void InitializeCollections()
        {
            TourDates = new ObservableCollection<TourDate>();
            TourReservations = new List<TourReservation>();
            Guests = new List<User>();
        }

        private void InitializeRepositories()
        {
            _imageRepository = new ImageRepository("tourImages.csv");
            _tourReservationRepository = new TourReservationRepository();
            _tourVoucherRepository = new TourVoucherRepository();
            _checkPointRepository = new CheckPointRepository();
            _tourDateRepository = new TourDateRepository();
            _locationRepository = new LocationRepository();
            _tourRepository = new TourRepository();
        }

        private void FindTourDates()
        {
            foreach (var date in Tour.TourDates)
            {
                TourDates.Add(date);
            }
        }

        private void PopulateTour()
        {
            Tour.TourLocation = _locationRepository.GetById(Tour.TourLocation.Id);
            Tour.CheckPoints = _checkPointRepository.GetByParentId(Tour.Id);
            Tour.TourDates = _tourDateRepository.GetByParentId(Tour.Id);
            Tour.Images = _imageRepository.GetByParentId(Tour.Id);
            Tour.Cover = Tour.Images[0];
        }

        private void CancelTour(object sender, RoutedEventArgs e)
        {
            if (SelectedDate == null)
            {
                var sMessageBoxText = "Select the tour date you want to cancel";
                var sCaption = "Tour Cancellation";
                var btnMessageBox = MessageBoxButton.OK;
                var icnMessageBox = MessageBoxImage.Warning;

                MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            }
            else
            {
                var timeDifference = CalculateTimeDifference();

                if (timeDifference.TotalHours < 48)
                {
                    var sMessageBoxText = "You cannot cancel the tour as it is less than 48 hours from now";
                    var sCaption = "Tour Cancellation";
                    var btnMessageBox = MessageBoxButton.OK;
                    var icnMessageBox = MessageBoxImage.Warning;

                    MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                }
                else
                {
                    var sMessageBoxText = $"Are you sure you want to cancel the tour for {SelectedDate.Date}?";
                    var sCaption = "Tour Cancellation";
                    var btnMessageBox = MessageBoxButton.OKCancel;
                    var icnMessageBox = MessageBoxImage.Warning;

                    MessageBoxResult result = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                    if (result == MessageBoxResult.OK)
                    {
                        TourReservations = GetTourReservations();
                        SendVouchers();
                        DeleteTourData();
                        DisplayCancellationMessage();
                    }
                }
            }
        }

        private void DisplayCancellationMessage()
        {
            var sMessageBoxText = "Tour has successfully been canceled!\n All guests have received a voucher!";
            var sCaption = "Tour Cancellation";
            var btnMessageBox = MessageBoxButton.OKCancel;
            var icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult result = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            if (result == MessageBoxResult.OK)
            {
                GuideOverview guideOverview = new(LoggedInUser);
                guideOverview.Show();
                Close();
            }
        }

        private List<User> GetGuests()
        {
            return _tourReservationRepository.GetUsersByTourAndDate(Tour.Id, GetSelectedDate());
        }

        private List<TourReservation> GetTourReservations()
        {
            return _tourReservationRepository.GetByTourAndDate(Tour.Id, GetSelectedDate());
        }

        private void DeleteTourData()
        {
            _tourDateRepository.DeleteByParentId(SelectedDate);
            DeleteTourReservations();
            if (TourDates.Count == 1)
            {
                MessageBox.Show("Tour data has been completely deleted", "Delete tour", MessageBoxButton.OK, MessageBoxImage.Information);
                _tourRepository.Delete(Tour);
                _checkPointRepository.DeleteAllByParentId(Tour.Id);
                _imageRepository.DeleteAllByParentId(Tour.Id);
                _tourDateRepository.DeleteByParentId(SelectedDate);
            }
        }

        private void DeleteTourReservations()
        {
            foreach (var reservation in TourReservations)
            {
                _tourReservationRepository.Delete(reservation);
            }
        }

        private void SendVouchers()
        {
            Guests = GetGuests();
            DateTime selectedDate = GetSelectedDate();
            DateTime nextYear = selectedDate.AddYears(1);

            foreach (var guest in Guests)
            {
                _tourVoucherRepository.Save(guest, nextYear);
            }
        }

        private TimeSpan CalculateTimeDifference()
        {
            DateTime currentDate = DateTime.Now;
            DateTime selectedDate = GetSelectedDate();
            DateTime selectedDateTime = selectedDate + selectedDate.TimeOfDay;
            TimeSpan timeDifference = selectedDateTime - currentDate;

            return timeDifference;
        }

        private void OpenGuideOverview(object sender, RoutedEventArgs e)
        {
            GuideOverview guideOverview = new(LoggedInUser);
            guideOverview.Show();
            Close();
        }

        private DateTime GetSelectedDate()
        {
            return SelectedDate.Date.Value;
        }
    }
}
