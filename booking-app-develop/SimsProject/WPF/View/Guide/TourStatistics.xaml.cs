using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using SimsProject.Domain.Model;
using SimsProject.Repository;
using User = SimsProject.Domain.Model.User;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for TourStatistics.xaml
    /// </summary>
    public partial class TourStatistics : Window
    {
        public List<TourReservation> TourReservations { get; set; }
        public List<TourVoucher> TourVouchers { get; set; }
        public List<Tour> FinishedTours { get; set; }
        public List<TourDate> TourDates { get; set; }
        public List<Image> Images { get; set; } 
        public List<Tour> Tours { get; set; }
        public SeriesCollection VoucherChart { get; set; }
        public SeriesCollection AgeChart { get; set; }

        private TourReservationRepository _tourReservationRepository;
        private TourVoucherRepository _tourVoucherRepository;
        private TourDateRepository _tourDateRepository;
        private ImageRepository _imageRepository;
        private TourRepository _tourRepository;

        private Tour SelectedTour { get; set; }
        private User LoggedInUser { get; set; }

        private int _guestsUnder18;
        private int _guestsBetween18And50;
        private int _guestsOver50;

        private int _totalGuests;
        private int _guestsWithVouchers;
        private int _guestsWithoutVouchers;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public TourStatistics(User user)
        {
            InitializeComponent();
            DataContext = this;

            LoggedInUser = user;

            InitializeRepositories();
            InitializeCollections();
        }
        private void InitializeRepositories()
        {
            _tourReservationRepository = new TourReservationRepository();
            _tourVoucherRepository = new TourVoucherRepository();
            _tourDateRepository = new TourDateRepository();
            // _imageRepository = new ImageRepository();
            _tourRepository = new TourRepository();
        }

        private void InitializeCollections()
        {
            TourReservations = new List<TourReservation>(_tourReservationRepository.GetAll());
            TourVouchers = new List<TourVoucher>(_tourVoucherRepository.GetAll());
            TourDates = new List<TourDate>(_tourDateRepository.GetAll());
            Tours = new List<Tour>(_tourRepository.GetAll());
            VoucherChart = new SeriesCollection();
            AgeChart = new SeriesCollection();
            FinishedTours = new List<Tour>();

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
                SelectedTour = (Tour)CbTours.SelectedItem;
                VoucherLabel.Content = "Guests with and without a voucher";
                AgeLabel.Content = "Age statistics";

                PopulateTour();
                CountGuestsByAgeGroup();
                CountGuestsByVouchers();
            }
        }

        private void CreateVoucherChart()
        {
            _guestsWithoutVouchers = _totalGuests - _guestsWithVouchers;

            VoucherChart.Add(new PieSeries
            {
                Title = "With voucher",
                Values = new ChartValues<double> { _guestsWithVouchers },
                DataLabels = true,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F5233")!),
                LabelPoint = point => $"{point.Y} ({point.Participation:P0})"
            });
            VoucherChart.Add(new PieSeries
            {
                Title = "Without voucher",
                Values = new ChartValues<double> { _guestsWithoutVouchers },
                DataLabels = true,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B1D8B7")!),
                LabelPoint = point => $"{point.Y} ({point.Participation:P0})"
            });
        }

        private void CreateAgeGroupChart()
        {
            AgeChart.Add(new PieSeries
            {
                Title = "Under 18",
                Values = new ChartValues<double> { _guestsUnder18 },
                DataLabels = true,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2F5233")!),
                LabelPoint = point => $"{point.Y} ({point.Participation:P0})"
            });
            AgeChart.Add(new PieSeries
            {
                Title = "18-50",
                Values = new ChartValues<double> { _guestsBetween18And50 },
                DataLabels = true,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B1D8B7")!),
                LabelPoint = point => $"{point.Y} ({point.Participation:P0})"
            });
            AgeChart.Add(new PieSeries
            {
                Title = "Over 50",
                Values = new ChartValues<double> { _guestsOver50 },
                DataLabels = true,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#94C973")!),
                LabelPoint = point => $"{point.Y} ({point.Participation:P0})"
            });
        }

        private void CountGuestsByVouchers()
        {
            foreach (var voucher in TourVouchers)
            {
                foreach (var date in TourDates)
                {
                    if (voucher.UsedOn == date.Date && voucher.Tour.Id == SelectedTour.Id && date.HasEnded &&
                        date.Tour.Id == SelectedTour.Id)
                    {
                        _guestsWithVouchers++;
                    }
                }
            }
            CreateVoucherChart();
        }
        private void CountGuestsByAgeGroup()
        {
            foreach (var reservation in TourReservations)
            {
                foreach (var date in TourDates)
                {
                    if (reservation.Tour.Id == SelectedTour.Id && date.Tour.Id == SelectedTour.Id && date.HasEnded && reservation.Date == date.Date)
                    {
                        switch (reservation.GuestAge)
                        {
                            case < 18:
                                _guestsUnder18++;
                                break;
                            case >= 18 and <= 50:
                                _guestsBetween18And50++;
                                break;
                            default:
                                _guestsOver50++;
                                break;
                        }
                    }
                }
                _totalGuests++;
            }
            CreateAgeGroupChart();
        }

        private void PopulateTour()
        {
            SelectedTour.TourDates = _tourDateRepository.GetByParentId(SelectedTour.Id);
        }

        private void OpenGuideOverview(object sender, RoutedEventArgs e)
        {
            GuideOverview guideOverview = new(LoggedInUser);
            guideOverview.Show();
            Close();
        }
    }
}
