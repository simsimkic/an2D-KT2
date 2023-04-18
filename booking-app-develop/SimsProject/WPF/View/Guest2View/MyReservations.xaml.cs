using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Collections.ObjectModel;

using SimsProject.Domain.Model;
using SimsProject.Repository;

namespace SimsProject.WPF.View.Guest2View
{
    /// <summary>
    /// Interaction logic for MyReservations.xaml
    /// </summary>
    public partial class MyReservations : Page
    {
        public static ObservableCollection<Tour> Tours { get; set; }
        public List<Tour> AllTours { get; set; }

        public List<TourReservation> TourReservations { get; set; }
        public List<TourAttendance> TourAttendances { get; set; }
        public Tour SelectedTour { get; set; }
        public User LoggedInUser { get; set; }


        private readonly TourReservationRepository _tourReservationRepository;
        private readonly LocationRepository _locationRepository;
        private readonly CheckPointRepository _checkPointRepository;
        private readonly TourDateRepository _tourDateRepository;
        private readonly ImageRepository _imageRepository;
        private readonly UserRepository _userRepository;
        private readonly TourRepository _tourRepository;
        private readonly TourAttendanceRepository _tourAttendanceRepository;


        public MyReservations(User user)
        {
            InitializeComponent();
            this.DataContext = this;
            LoggedInUser = user;


            _tourReservationRepository = new TourReservationRepository();
            _locationRepository = new LocationRepository();
            _checkPointRepository = new CheckPointRepository();
            _tourDateRepository = new TourDateRepository();
            _userRepository = new UserRepository();
            _imageRepository = new ImageRepository("tourImages.csv");
            _tourRepository = new TourRepository();
            _tourAttendanceRepository = new TourAttendanceRepository();
            TourAttendances = new List<TourAttendance>(_tourAttendanceRepository.GetAll());

            TourReservations = new List<TourReservation>(_tourReservationRepository.GetAll());
            AllTours = new List<Tour>(_tourRepository.GetAll());
            Tours = new ObservableCollection<Tour>(_tourReservationRepository.GetReservedToursByUser(user, AllTours));
            PopulateTours();
            PopulateTourAttendances();
            GetSortedCheckPoints();



        }
        
        

        

        private void PopulateTourAttendances()
        {
            foreach (var attendance in TourAttendances)
            {
                foreach (var tour in Tours)
                {
                    if (tour.Id == attendance.Tour.Id)
                    {
                        attendance.Tour.Name = tour.Name;
                    }
                }
            }
        }

        private void PopulateTours()
        {
            foreach (var tour in AllTours)
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

        public void SortCheckPoints(List<CheckPoint> checkPoints)
        {
            CheckPoint lastCheckPoint = checkPoints[1];
            checkPoints.RemoveAt(1);
            checkPoints.Add(lastCheckPoint);
        }


     



    
    }
}

