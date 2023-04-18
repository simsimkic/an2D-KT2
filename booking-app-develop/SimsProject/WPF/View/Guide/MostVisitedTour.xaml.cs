using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using SimsProject.Domain.Model;
using SimsProject.Repository;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for MostVisitedTour.xaml
    /// </summary>
    public partial class MostVisitedTour : Window
    {
        public List<TourReservation> TourReservations { get; set; }
        public List<TourDate> TourDates { get; set; } 
        public List<Tour> Tours { get; set; }
        public List<string> Years { get; set; }

        private TourReservationRepository _tourReservationRepository;
        private CheckPointRepository _checkPointRepository;
        private LocationRepository _locationRepository;
        private TourDateRepository _tourDateRepository;
        private ImageRepository _imageRepository;
        private TourRepository _tourRepository;

        private Tour Tour { get; set; }
        private User LoggedInUser { get; set; }
        private string SelectedYear { get; set; }
        private int GuestNumber { get; set; }

        public MostVisitedTour(User user)
        {
            InitializeComponent();
            DataContext = this;
            LoggedInUser = user;

            InitializeRepositories();
            InitializeCollections();
            FindYears();
            CbYears.SelectedItem = Years[0];
        }

        private void InitializeRepositories()
        {
            _tourReservationRepository = new TourReservationRepository();
            _imageRepository = new ImageRepository("tourImages.csv");
            _checkPointRepository = new CheckPointRepository();
            _tourDateRepository = new TourDateRepository();
            _locationRepository = new LocationRepository();
            _tourRepository = new TourRepository();
        }

        private void InitializeCollections()
        {
            TourReservations = new List<TourReservation>(_tourReservationRepository.GetAll());
            TourDates = new List<TourDate>(_tourDateRepository.GetAll());
            Tours = new List<Tour>(_tourRepository.GetAll());
            Years = new List<string>();
        }

        private void FindYears()
        {
            Years.Add("All years");

            foreach (var date in TourDates)
            {
                if (date.Date != null && !Years.Contains(date.Date.Value.Year.ToString()))
                { 
                    Years.Add(date.Date.Value.Year.ToString());
                }
            }
        }

        private void OpenGuideOverview(object sender, RoutedEventArgs e)
        {
            GuideOverview guideOverview = new(LoggedInUser);
            guideOverview.Show();
            Close();
        }

        private void FindTour(object sender, RoutedEventArgs e)
        {
            SelectedYear = (string)CbYears.SelectedItem;
            var filteredReservations = TourReservations.Where(r => SelectedYear.Equals(Years[0]) || r.Date.Year == int.Parse(SelectedYear));
            var groupedReservations = filteredReservations.GroupBy(r => r.Tour.Id).Select(g => new
            {
                TourId = g.Key,
                TotalGuests = g.Sum(r => r.GuestNumber)
            });
            var tourWithMostGuests = groupedReservations.MaxBy(g => g.TotalGuests);
            Tour = Tours.Find(t => t.Id == tourWithMostGuests.TourId);
            GuestNumber = tourWithMostGuests.TotalGuests;
            PopulateTour();
        }


        private void PopulateTour()
        {
            Tour.TourLocation = _locationRepository.GetById(Tour.TourLocation.Id); 
            Tour.CheckPoints = _checkPointRepository.GetByParentId(Tour.Id);
            Tour.TourDates = _tourDateRepository.GetByParentId(Tour.Id); 
            Tour.Images = _imageRepository.GetByParentId(Tour.Id); 
            Tour.Cover = Tour.Images[0];
            
            TbName.Text = "Name: " + Tour.Name; 
            TbLocation.Text = "Location: " + Tour.TourLocation; 
            TbDescription.Text = "Description: " + Tour.Description; 
            TbLanguage.Text = "Language: " + Tour.Language; 
            TourImage.Source = new BitmapImage(new Uri(Tour.Cover.Url));
            TbGuestNumber.Text = "Total number of guests: " + GuestNumber;
        }
    }
}
