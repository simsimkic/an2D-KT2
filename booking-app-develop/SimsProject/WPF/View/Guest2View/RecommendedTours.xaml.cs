using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.Repository;

namespace SimsProject.WPF.View
{
    /// <summary>
    /// Interaction logic for SearchTourWindow.xaml
    /// </summary>
    public partial class RecommendedTours : Window
    {
        public ObservableCollection<Tour> Tours { get; set; }
        public Tour SelectedTour { get; set; }
        public Location Location { get; set; }

        public Tour GivenTour { get; set; }
        public List<Location> Locations { get; set; }
        private readonly TourRepository _tourRepository;
        private readonly LocationRepository _locationRepository;
        private readonly CheckPointRepository _checkPointRepository;
        private readonly TourDateRepository _tourDateRepository;
        private readonly ImageRepository _imageRepository;
        private readonly UserRepository _userRepository;

        public User CurrentUser { get; set; }
        public RecommendedTours(User currentUser,Tour tour)
        {
            InitializeComponent();
            DataContext = this;
            CurrentUser = currentUser;
            GivenTour = tour;
            _tourRepository = new TourRepository();
            _locationRepository = new LocationRepository();
            _checkPointRepository = new CheckPointRepository();
            _tourDateRepository = new TourDateRepository();
            _userRepository = new UserRepository();
            _imageRepository = new ImageRepository("tourImages.csv");
            Tours = new ObservableCollection<Tour>(_tourRepository.GetAll());

             PopulateTours();
             GetSortedCheckPoints();

            UpdateTable();



        }


        private void PopulateTours()
        {
            foreach (var tour in Tours)
            {
               
                    tour.TourLocation = _locationRepository.GetById(tour.TourLocation.Id);
                    tour.CheckPoints = _checkPointRepository.GetByParentId(tour.Id);
                    tour.User = _userRepository.GetById(tour.User.Id);
                    tour.TourDates = _tourDateRepository.GetByParentId(tour.Id);
                    tour.Images = _imageRepository.GetByParentId(tour.Id);
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

        public bool isTourValid(Tour tour) {

            return (tour.Id!=GivenTour.Id && tour.TourLocation.Id==GivenTour.TourLocation.Id);

        }

        public void SortCheckPoints(List<CheckPoint> checkPoints)
        {
            CheckPoint lastCheckPoint = checkPoints[1];
            checkPoints.RemoveAt(1);
            checkPoints.Add(lastCheckPoint);
        }



        private void UpdateTable()
        {
            Tours.Clear();
            foreach (var tour in _tourRepository.GetAll())
            {
                if (isTourValid(tour))
                {
                    Tours.Add(tour);
                    PopulateTours();
                    GetSortedCheckPoints();
                }
            }
        }




        private void ReserveClick(object sender, RoutedEventArgs e)
        {
            if (SelectedTour != null)
            {
                ReserveTourForm form = new(SelectedTour, CurrentUser);
                form.Show();
            }
            else
            {
                MessageBox.Show("Please select Tour.");
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}