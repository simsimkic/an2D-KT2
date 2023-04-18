using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SimsProject.Domain.Model;
using SimsProject.Repository;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for LiveTourTracking.xaml
    /// </summary>
    public partial class LiveTourTracking : Window, INotifyPropertyChanged
    {
        public ObservableCollection<CheckPoint> CheckPoints { get; set; }
        public static ObservableCollection<Tour> Tours { get; set; }
        public ObservableCollection<Tour> ToursToday { get; set; }
        public ObservableCollection<User> Guests { get; set; }
        public List<TourReservation> TourReservations { get; set; }
        public TourAttendance SelectedTourAttendance { get; set; }
        public Tour SelectedTour { get; set; }
        public bool HasStarted { get; set; }

        private ObservableCollection<TourAttendance> _tourAttendances;
        private TourReservationRepository _tourReservationRepository;
        private TourAttendanceRepository _tourAttendanceRepository; 
        private CheckPointRepository _checkPointRepository;
        private LocationRepository _locationRepository;
        private TourDateRepository _tourDateRepository;
        private ImageRepository _imageRepository;
        private TourRepository _tourRepository;
        private UserRepository _userRepository;
        private CheckPoint _checkPoint;

        public CheckPoint CheckPoint
        {
            get => _checkPoint;
            set
            {
                _checkPoint = value;
                OnPropertyChanged(nameof(CheckPoint));
            }
        }

        public ObservableCollection<TourAttendance> TourAttendances
        {
            get => _tourAttendances;
            set
            {
                _tourAttendances = value;
                OnPropertyChanged(nameof(TourAttendances));
            }
        }

        public LiveTourTracking()
        {
            InitializeComponent();
            DataContext = this;
            
            InitializeRepositories();
            InitializeCollections();
            PopulateTours();
            GetToursToday();
            LoadTourData();
        }

        private void InitializeRepositories()
        {
            _imageRepository = new ImageRepository("tourImages.csv");
            _tourReservationRepository = new TourReservationRepository();
            _tourAttendanceRepository = new TourAttendanceRepository();
            _checkPointRepository = new CheckPointRepository();
            _locationRepository = new LocationRepository();
            _tourDateRepository = new TourDateRepository();
            _tourRepository = new TourRepository();
            _userRepository = new UserRepository();
        }

        private void InitializeCollections()
        {
            Tours = new ObservableCollection<Tour>(_tourRepository.GetAll());
            TourAttendances = new ObservableCollection<TourAttendance>();
            TourReservations = _tourReservationRepository.GetAll();
            CheckPoints = new ObservableCollection<CheckPoint>();
            ToursToday = new ObservableCollection<Tour>();
            Guests = new ObservableCollection<User>();
            HasStarted = false;
        }

        private void PopulateTours()
        {
            foreach (var tour in Tours)
            {
                tour.TourLocation = _locationRepository.GetById(tour.TourLocation.Id);
                tour.CheckPoints = _checkPointRepository.GetByParentId(tour.Id);
                tour.TourDates = _tourDateRepository.GetByParentId(tour.Id);
                tour.Images = _imageRepository.GetByParentId(tour.Id);
                tour.User = _userRepository.GetById(tour.User.Id);
                tour.Cover = tour.Images[0];
            }
        }

        private void PopulateTourAttendances()
        {
            foreach (var attendance in TourAttendances)
            {
                attendance.User = _userRepository.GetById(attendance.User.Id);
                attendance.CheckPoint = _checkPointRepository.Get(attendance.CheckPoint.Id);
            }

            if (HasStarted)
            {
                foreach (var attendance in TourAttendances)
                {
                    if (attendance.Present.Equals(Presence.GuestNotPresent))
                    {
                        attendance.CheckPoint.Name = "Guest did not show up";
                        attendance.CheckPoint.Id = -1;
                    }
                }
            }
        }

        public void LoadTourData()
        {
            foreach (var tour in Tours)
            {
                foreach (var checkPoint in tour.CheckPoints)
                {
                    if (checkPoint.IsActive)
                    {
                        SelectedTour = tour;
                        HasStarted = true;
                        GetSortedCheckPoints();
                        RemoveTracklessTour(SelectedTour);
                        GetGuests();
                        GetTourAttendances();
                        PopulateTourAttendances();
                    }
                }
            }
        }

        private void GetTourAttendances()
        {
            foreach (var guest in Guests)
            {
                TourAttendances.Add(_tourAttendanceRepository.GetByGuest(guest));
            }
        }
        private void RecordAttendance(object sender, RoutedEventArgs e)
        {
            if (SelectedTourAttendance == null)
            {
                MessageBox.Show("Please choose a guest");
            }
            else
            {
                if (SelectedTourAttendance.Present == Presence.NotPresent)
                {
                    SelectedTourAttendance.Present = Presence.GuideConfirmed;
                    SelectedTourAttendance.CheckPoint = CheckPoint;
                    _tourAttendanceRepository.Update(SelectedTourAttendance);
                }
                else
                {
                    MessageBox.Show("Guest is already present!", "Attendance", MessageBoxButton.OK);
                }
            }
        }
        
        public void GetToursToday()
        {
            DateTime dateOnly = GetSystemDateOnly();
            foreach (Tour tour in Tours)
            {
                foreach (var tourDate in tour.TourDates)
                {
                    DateTime? tourDateOnly = ExtractTourDate(tourDate);

                    if (tourDateOnly.Equals(dateOnly) && !tourDate.HasEnded)
                    {
                        ToursToday.Add(tour);
                    }
                }
            }
        }

        public static DateTime GetSystemDateOnly()
        {
            DateTime dateTime = DateTime.Now;
            return dateTime.Date;
        }

        public static DateTime ExtractTourDate(TourDate tourDate)
        {
            DateTime? date = tourDate.Date;
            if (date != null)
            {
                return date.Value.Date;
            }
            return DateTime.MinValue;
        }

        private void FollowTourLive(object sender, RoutedEventArgs e)
        {
            if (SelectedTour == null)
            {
                MessageBox.Show("Tour is not selected", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            } 
            else if(HasStarted)
            {
                MessageBox.Show("Tour has already started", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                HasStarted = true;
                GetGuests();
                GetSortedCheckPoints();
                RemoveTracklessTour(SelectedTour);
                SaveTourAttendances();
                PopulateTourAttendances();
            }
        }

        private void SaveTourAttendances()
        {
            foreach (var guest in Guests)
            {
                TourAttendance savedAttendance = _tourAttendanceRepository.Save(SelectedTour, guest, Presence.NotPresent, new CheckPoint());
                TourAttendances.Add(savedAttendance);
            }
        }

        private void GetGuests()
        {
            var tourDate = SelectedTour.TourDates.Find(t => t.Date != null && t.Date.Value.Date == DateTime.Today);
            Guests = new ObservableCollection<User>(_tourReservationRepository.GetUsersByTourAndDate(SelectedTour.Id, tourDate.Date));
            PopulateGuests();
        }

        private void PopulateGuests()
        {
            foreach (var guest in Guests)
            {
                guest.Username = _userRepository.GetById(guest.Id).Username;
            }
        }

        public void RemoveTracklessTour(Tour selectedTour)
        {
            foreach (Tour tour in ToursToday.ToList())
            {
                if (tour.Id != selectedTour.Id)
                {
                    ToursToday.Remove(tour);
                }
            }
        }

        public void GetSortedCheckPoints()
        {
            foreach (var checkPoint in SelectedTour.CheckPoints)
            {
                CheckPoints.Add(checkPoint);
            }
            SortCheckPoints(SelectedTour);
        }

        public void SortCheckPoints(Tour selectedTour)
        {
            CheckPoint secondCheckPoint = selectedTour.CheckPoints[1];
            CheckPoints.RemoveAt(1);
            CheckPoints.Add(secondCheckPoint);
            CheckPoints[0].IsActive = true;
            _checkPointRepository.Update(CheckPoints[0], selectedTour.Id);
        }

        private void EndTourPrematurely(object sender, RoutedEventArgs e)
        {
            if (HasStarted)
            {
                MessageBoxResult result = ConfirmEndTour();
                if (result == MessageBoxResult.Yes)
                {
                    foreach (var tourDate in SelectedTour.TourDates)
                    {
                        DateTime dateOnly = GetSystemDateOnly();
                        DateTime? tourDateOnly = ExtractTourDate(tourDate);

                        if (tourDateOnly.Equals(dateOnly) && tourDate.Tour.Id == SelectedTour.Id)
                        {
                            tourDate.HasEnded = true;
                            _tourDateRepository.Update(tourDate);
                        }
                    }

                    _checkPointRepository.SetDefault();
                    Close();
                }
            }
            else
            {
                MessageBox.Show("No tours have started yet", "End tour prematurely", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static MessageBoxResult ConfirmEndTour()
        {
            var messageBoxText = "Are you sure you want to end the tour prematurely?";
            var messageBoxCaption = "Warning";
            var messageBoxButton = MessageBoxButton.YesNo;
            var messageBoxImage = MessageBoxImage.Warning;

            var result = MessageBox.Show(messageBoxText, messageBoxCaption, messageBoxButton, messageBoxImage);

            return result;
        }

        private void MarkCurrentCheckPoint(object sender, RoutedEventArgs e)
        {
            CheckPoint selectedCheckPoint = null;
            RadioButton selectedRadioButton = sender as RadioButton;
            if (selectedRadioButton != null)
            {
                selectedCheckPoint = selectedRadioButton.DataContext as CheckPoint;
            }

            selectedRadioButton.IsEnabled = false;

            _checkPointRepository.Update(selectedCheckPoint, SelectedTour.Id);
            CheckPoint = selectedCheckPoint;

            if (selectedCheckPoint != null && selectedCheckPoint.SerialNumber == 2)
            {
                MessageBox.Show("Tour has successfully ended!", "TourAttendance", MessageBoxButton.OK, MessageBoxImage.Information);
                
                foreach (var tourDate in SelectedTour.TourDates)
                {
                    DateTime dateOnly = GetSystemDateOnly();
                    DateTime? tourDateOnly = ExtractTourDate(tourDate);

                    if (tourDateOnly.Equals(dateOnly) && tourDate.Tour.Id == SelectedTour.Id)
                    {
                        tourDate.HasEnded = true;
                        _tourDateRepository.Update(tourDate);
                    }
                }
                _checkPointRepository.SetDefault();
                Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
