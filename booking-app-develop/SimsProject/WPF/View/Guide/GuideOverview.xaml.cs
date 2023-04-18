using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.Repository;
using Image = SimsProject.Domain.Model.Image;
using User = SimsProject.Domain.Model.User;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for GuideOverview.xaml
    /// </summary>
    public partial class GuideOverview : Window, INotifyPropertyChanged
    {
        public static ObservableCollection<Tour> Tours { get; set; }
        public List<TourDate> TourDates { get; set; }
        public Tour SelectedTour { get; set; }
        public User LoggedInUser { get; set; }

        private CheckPointRepository _checkPointRepository;
        private TourDateRepository _tourDateRepository;
        private LocationRepository _locationRepository;
        private ImageRepository _imageRepository;
        private TourRepository _tourRepository;
        private UserRepository _userRepository;

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

        public GuideOverview(User user)
        {
            InitializeComponent();
            this.DataContext = this;

            LoggedInUser = user;

            InitializeRepositories();
            InitializeCollections();
            PopulateTours();
            GetSortedCheckPoints();
        }

        private void InitializeRepositories()
        {
            _imageRepository = new ImageRepository("tourImages.csv");
            _checkPointRepository = new CheckPointRepository();
            _tourDateRepository = new TourDateRepository();
            _locationRepository = new LocationRepository();
            _tourRepository = new TourRepository();
            _userRepository = new UserRepository();
        }

        private void InitializeCollections()
        { 
            Tours = new ObservableCollection<Tour>(_tourRepository.GetByUser(LoggedInUser));
            TourDates = new List<TourDate>(_tourDateRepository.GetAll());
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

        public void GetSortedCheckPoints()
        {
            foreach (var tour in Tours)
            {
                SortCheckPoints(tour.CheckPoints);
            }
        }

        public void SortCheckPoints(List<CheckPoint> checkPoints)
        {
            CheckPoint lastCheckPoint = checkPoints[1];
            checkPoints.RemoveAt(1);
            checkPoints.Add(lastCheckPoint);
        }

        private void CreateTourForm(object sender, RoutedEventArgs e)
        {
            CreateTour createTour = new(LoggedInUser);
            createTour.Show();
        }

        private void TrackTourLive(object sender, RoutedEventArgs e)
        {
            var exists = CheckForToursStartingToday();
            if (exists)
            {
                LiveTourTracking liveTourTracking = new();
                liveTourTracking.Show();
            }
            else
            {
                var sMessageBoxText = "There are no tours scheduled for today";
                var sCaption = "Tours today";
                var btnMessageBox = MessageBoxButton.OK;
                var icnMessageBox = MessageBoxImage.Warning;
                MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            }
        }

        private bool CheckForToursStartingToday()
        {
            bool exists = false;
            var systemDateOnly = GetSystemDateOnly();
            foreach (var tour in Tours)
            {
                foreach (var date in tour.TourDates)
                {
                    var tourDateOnly = ExtractTourDate(date);
                    if (tourDateOnly == systemDateOnly)
                    {
                        exists = true;
                        break;
                    }
                }
            }
            return exists;
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
        
        private void Logout(object sender, RoutedEventArgs e)
        {
            var result = ConfirmLogout();
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

        private static MessageBoxResult ConfirmLogout()
        {
            var sMessageBoxText = "Are you sure you want to logout?";
            var sCaption = "Logout";
            var btnMessageBox = MessageBoxButton.YesNo;
            var icnMessageBox = MessageBoxImage.Question;

            MessageBoxResult result = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            return result;
        }

        private void ShowImages_Click(object sender, RoutedEventArgs e)
        {
            _selectedImageIndex = 0;
            SelectedImage = SelectedTour.Images[_selectedImageIndex];
            ImagePopup.IsOpen = true;
        }
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            _selectedImageIndex = (_selectedImageIndex + 1) % SelectedTour.Images.Count;
            SelectedImage = SelectedTour.Images[_selectedImageIndex];
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            _selectedImageIndex = (_selectedImageIndex + SelectedTour.Images.Count - 1) % SelectedTour.Images.Count;
            SelectedImage = SelectedTour.Images[_selectedImageIndex];
        }

        private void CloseImages_Click(object sender, RoutedEventArgs e)
        {
            ImagePopup.IsOpen = false;
        }

        private void CancelTour(object sender, RoutedEventArgs e)
        {
            if (SelectedTour == null)
            {
                var sMessageBoxText = "Select tour that you want to cancel";
                var sCaption = "Tour Cancellation";
                var btnMessageBox = MessageBoxButton.OK;
                var icnMessageBox = MessageBoxImage.Warning;
                MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            }
            else
            {
                DeleteTour deleteTour = new(SelectedTour, LoggedInUser);
                deleteTour.Show();
                Close();
            }
        }

        private void MostTourVisitations(object sender, RoutedEventArgs e)
        {
            MostVisitedTour mostVisitedTour = new(LoggedInUser);
            mostVisitedTour.Show();
            Close();
        }

        private void TourStatistics(object sender, RoutedEventArgs e)
        {
             TourStatistics tourStatistics = new(LoggedInUser);
             tourStatistics.Show();
             Close();
        }

        private void TourReviews(object sender, RoutedEventArgs e)
        {
            var hasEnded = TourDates.Any(date => date.HasEnded);

            if (hasEnded)
            {
                TourReviews tourReviews = new TourReviews(LoggedInUser);
                tourReviews.Show();
                Close();
            }
            else
            {
                var sMessageBoxText = "You don't have any finished tours yet";
                var sCaption = "Tour reviews";
                var btnMessageBox = MessageBoxButton.OK;
                var icnMessageBox = MessageBoxImage.Warning;
                MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
