using SimsProject.Domain.Model;
using SimsProject.Repository;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Image = SimsProject.Domain.Model.Image;
using User = SimsProject.Domain.Model.User;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for TourReviews.xaml
    /// </summary>
    public partial class TourReviews : Window, INotifyPropertyChanged
    {
        public List<TourAttendance> TourAttendances { get; set; }
        public List<CheckPoint> CheckPoints { get; set; }
        public List<TourReview> Reviews { get; set; }
        public List<TourDate> TourDates { get; set; }
        public List<Tour> FinishedTours { get; set; }
        public List<Tour> Tours { get; set; }
        private User LoggedInUser { get; set; }

        private TourAttendanceRepository _tourAttendanceRepository;
        private CheckPointRepository _checkPointRepository;
        private TourReviewRepository _tourReviewRepository;
        private TourDateRepository _tourDateRepository;
        private ImageRepository _imageRepository;
        private TourRepository _tourRepository;

        private ObservableCollection<TourReview> _filteredReviews;
        public ObservableCollection<TourReview> FilteredReviews
        {
            get => _filteredReviews;
            set
            {
                _filteredReviews = value;
                OnPropertyChanged();
            }
        }

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


        private TourReview _selectedReview;
        public TourReview SelectedReview
        {
            get => _selectedReview;
            set
            {
                if (_selectedReview != value)
                {
                    _selectedReview = value;
                    OnPropertyChanged();
                }
            }
        }

        public TourReviews(User user)
        {
            InitializeComponent();
            DataContext = this;
            ImagePopup.DataContext = this;

            LoggedInUser = user;

            InitializeRepositories();
            InitializeCollections();
        }
        private void InitializeRepositories()
        {
            _imageRepository = new ImageRepository("tourReviewImages.csv");
            _tourAttendanceRepository = new TourAttendanceRepository();
            _tourReviewRepository = new TourReviewRepository();
            _checkPointRepository = new CheckPointRepository();
            _tourDateRepository = new TourDateRepository();
            _tourRepository = new TourRepository();
        }

        private void InitializeCollections()
        {
            TourAttendances = new List<TourAttendance>(_tourAttendanceRepository.GetAll());
            CheckPoints = new List<CheckPoint>(_checkPointRepository.GetAll());
            Reviews = new List<TourReview>(_tourReviewRepository.GetAll());
            TourDates = new List<TourDate>(_tourDateRepository.GetAll());
            Tours = new List<Tour>(_tourRepository.GetAll());
            FilteredReviews = new ObservableCollection<TourReview>();

            FindFinishedTours();
        }

        private void FindFinishedTours()
        {
            FinishedTours = Tours.Where(tour =>
                TourDates.Any(date =>
                    date.HasEnded && date.Tour.Id == tour.Id)).ToList();
        }

        private void FindTour(object sender, RoutedEventArgs e)
        {
            if (CbTours.SelectedItem == null)
            {
                const string sMessageBoxText = "Please select the tour for which you want to see statistics";
                const string sCaption = "Tours statistics";
                const MessageBoxButton btnMessageBox = MessageBoxButton.OK;
                const MessageBoxImage icnMessageBox = MessageBoxImage.Warning;
                MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            }
            else
            {
                FilteredReviews = new ObservableCollection<TourReview>(Reviews.Where(review => Tours.Any(tour => tour.Id == review.Tour.Id) &&
                                                                                               TourDates.Any(date => date.Tour.Id == review.Tour.Id &&
                                                                                                                     date.Id == review.TourDate.Id)).ToList());
                if (FilteredReviews.Count == 0)
                {
                    const string sMessageBoxText = "There are no available reviews for this tour yet";
                    const string sCaption = "Reviews";
                    const MessageBoxButton btnMessageBox = MessageBoxButton.OK;
                    const MessageBoxImage icnMessageBox = MessageBoxImage.Warning;
                    MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
                }
                else
                {
                    PopulateFilteredReviews();
                    TourReviewList.ItemsSource = FilteredReviews;
                }
            }
        }

        private void PopulateFilteredReviews()
        {
            foreach (var review in FilteredReviews)
            {
                foreach (var attendance in TourAttendances)
                {
                    if (review.Guest.Id == attendance.User.Id && review.Tour.Id == attendance.Tour.Id)
                    {
                        review.CheckPoint = attendance.CheckPoint;
                    }
                    PopulateCheckPoints();
                }
                review.Tour = _tourRepository.GetByParentId(review.Tour.Id);
                review.Images = _imageRepository.GetByParentId(review.Id);
                review.Cover = review.Images[0];
            }
        }

        private void PopulateCheckPoints()
        {
            foreach (var review in FilteredReviews)
            {
                foreach (var checkPoint in CheckPoints.Where(checkPoint => checkPoint.Id == review.CheckPoint.Id))
                {
                    review.CheckPoint = checkPoint;
                    _tourReviewRepository.Update(review);
                }
            }
        }

        private void OpenGuideOverview(object sender, RoutedEventArgs e)
        {
            GuideOverview guideOverview = new(LoggedInUser);
            guideOverview.Show();
            Close();
        }

        private void ReportReview(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { Tag: TourReview selectedReview }) return;

            if (selectedReview.IsValid == false)
            {
                MessageBox.Show("Review has already been reported", "Report review", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else
            {
                const string sMessageBoxText = "Are you sure you want to report this review?";
                const string sCaption = "Report review";
                const MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
                const MessageBoxImage icnMessageBox = MessageBoxImage.Warning;
                var result = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

                if (result != MessageBoxResult.Yes) return;

                selectedReview.IsValid = false;
                _tourReviewRepository.Update(selectedReview);

                var listBox = TourReviewList;

                if (listBox.ItemContainerGenerator.ContainerFromItem(selectedReview) is not ListBoxItem listBoxItem)
                    return;
                var validTextBlock = FindChild<TextBlock>(listBoxItem, "ValidTextBlock");

                if (validTextBlock == null) return;
                var bindingExpression = validTextBlock.GetBindingExpression(TextBlock.TextProperty);
                bindingExpression?.UpdateTarget();
            }
        }

        private void ShowImages_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button { Tag: TourReview selectedReview }) return;
            SelectedReview = selectedReview;
            _selectedImageIndex = 0;
            SelectedImage = SelectedReview.Images[_selectedImageIndex];
            ImagePopup.IsOpen = true;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            _selectedImageIndex = (_selectedImageIndex + 1) % SelectedReview.Images.Count;
            SelectedImage = SelectedReview.Images[_selectedImageIndex];
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            _selectedImageIndex = (_selectedImageIndex + SelectedReview.Images.Count - 1) % SelectedReview.Images.Count;
            SelectedImage = SelectedReview.Images[_selectedImageIndex];
        }

        private void CloseImages_Click(object sender, RoutedEventArgs e)
        {
            ImagePopup.IsOpen = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            T child = null;
            var numChildren = VisualTreeHelper.GetChildrenCount(parent);

            for (var i = 0; i < numChildren; i++)
            {
                var childElement = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;

                if (childElement?.Name == childName)
                {
                    child = childElement as T;
                    break;
                }

                child = FindChild<T>(childElement, childName);

                if (child != null) break;
            }
            return child;
        }
    }
}
