using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using SimsProject.Domain.Model;
using SimsProject.Repository;
using Image = SimsProject.Domain.Model.Image;

namespace SimsProject.WPF.View.Guide
{
    /// <summary>
    /// Interaction logic for CreateTour.xaml
    /// </summary>
    public partial class CreateTour : Window
    {
        public ObservableCollection<Image> Images { get; set; }
        public List<Location> Locations { get; set; }
        public List<string> Countries { get; set; }
        public List<string> Cities { get; set; }
        public string[] LanguageList { get; set; }
        public User LoggedInUser { get; set; }

        private TourRepository _tourRepository;
        private CheckPointRepository _checkPointRepository;
        private TourDateRepository _tourDateRepository;
        private LocationRepository _locationRepository;
        private ImageRepository _imageRepository;

        private List<CheckPoint> _temporaryCheckPoints;
        private List<TourDate> _temporaryTourDates;
        private List<string> _messages;

        private string _checkPointLocation;
        private string _description;
        private string _tourName;
        private string _language;
        private string _country;
        private string _city;
        private DateTime? _tourDate;
        private int _currentCheckPoint;
        private int _maxGuestNumber;
        private int _duration;
        private Tour _newTour;

        public string TourName
        {
            get => _tourName;
            set
            {
                if (value != _tourName)
                {
                    _tourName = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Description
        {
            get => _description;
            set
            {
                if (value != _description)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TourLanguage
        {
            get => _language;
            set
            {
                if (value != _language)
                {
                    _language = value;
                    OnPropertyChanged();
                }
            }
        }
        public int MaxGuestNumber
        {
            get => _maxGuestNumber;
            set
            {
                if (value != _maxGuestNumber)
                {
                    _maxGuestNumber = value;
                    OnPropertyChanged();
                }
            }
        }
        public DateTime? TourDate
        {
            get => _tourDate;
            set
            {
                if (value != _tourDate)
                {
                    _tourDate = value;
                    OnPropertyChanged();
                }
            }
        }
        public int Duration
        {
            get => _duration;
            set
            {
                if (value != _duration)
                {
                    _duration = value;
                    OnPropertyChanged();
                }
            }
        }
        public string CheckPointLocation
        {
            get => _checkPointLocation;
            set
            {
                if (value != _checkPointLocation)
                {
                    _checkPointLocation = value;
                    OnPropertyChanged();
                }
            }
        }
        public string City
        {
            get => _city;
            set
            {
                if (value != _city)
                {
                    _city = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Country
        {
            get => _country;
            set
            {
                if (value != _country)
                {
                    _country = value;
                    OnPropertyChanged();
                }
            }
        }

        public CreateTour(User user)
        {
            InitializeComponent();
            DataContext = this;
            LoggedInUser = user;

            InitializeRepositories();
            InitializeCollections();

            CboCountry.SelectedItem = Countries[0];
            CboCity.SelectedItem = Cities[0];
            CboLanguage.SelectedItem = LanguageList[0];
        }

        private void InitializeRepositories()
        {
            _imageRepository = new ImageRepository("tourImages.csv");
            _checkPointRepository = new CheckPointRepository();
            _tourDateRepository = new TourDateRepository();
            _locationRepository = new LocationRepository();
            _tourRepository = new TourRepository();
        }

        private void InitializeCollections()
        {
            Locations = new List<Location>(_locationRepository.GetAll());
            _temporaryCheckPoints = new List<CheckPoint>();
            _temporaryTourDates = new List<TourDate>();
            Images = new ObservableCollection<Image>();

            _currentCheckPoint = 0;
            _messages = new List<string>
            {
                "End check point: ",
                "Additional check points: "
            };

            LanguageList = new[] { "English", "Serbian", "Spanish", "Hungarian", "Italian" };

            Countries = GetDistinctCountries(Locations);

            var filteredLocations = FilterLocationsByCountry(Locations, Countries[0]);
            Cities = GetDistinctCities(filteredLocations);
        }


        private void UploadImages(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Image files|*.jpeg;*.jpg;*.png",
                FilterIndex = 1,
                Multiselect = true
            };
            try
            {
                if (openFileDialog.ShowDialog() != true)
                {
                    return;
                }
                foreach (var filename in openFileDialog.FileNames)
                {
                    Image image = new()
                    {
                        Url = new Uri(filename).AbsoluteUri,
                    };
                    Images.Add(image);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Error adding image from the computer: {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UploadUrl(object sender, RoutedEventArgs e)
        {
            var url = TbxImageUrls.Text;
            if (IsUrlValid(url))
            {
                try
                {
                    Image image = new()
                    {
                        Url = url,
                    };
                    Images.Add(image);
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"Error adding image from URL '{url}': {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show($"Invalid URL '{url}'", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            TbxImageUrls.Text = "";
        }


        private static bool IsUrlValid(string url)
        {
            var pattern = @"^(http|https)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$";
            return Regex.IsMatch(url, pattern);
        }

        private void Create()
        {
            var location = _locationRepository.GetByCityAndCountry(City, Country);
            _newTour = _tourRepository.Save(TourName, Description, TourLanguage, MaxGuestNumber, location, Duration, Images.ToList(), LoggedInUser);
            _imageRepository.SaveAllByParentId(_newTour.Id, Images.ToList());

            GuideOverview.Tours.Add(_newTour);

            SaveCheckPoints();
            SaveTourDates();

            Close();
        }

        private void SaveTourDates()
        {
            foreach (var tourDate in _temporaryTourDates)
            {
                tourDate.Tour = _newTour;
                _newTour.TourDates.Add(tourDate);
                _tourDateRepository.Save(tourDate.Tour, tourDate.Date);
            }
        }

        private void SaveCheckPoints()
        {
            foreach (var checkPoint in _temporaryCheckPoints)
            {
                checkPoint.Tour = _newTour;
                _newTour.CheckPoints.Add(checkPoint);
                _checkPointRepository.Save(checkPoint.IsActive, checkPoint.Tour, checkPoint.SerialNumber, checkPoint.Name);
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Click_AddCheckPoint(object sender, RoutedEventArgs e)
        {
            var checkPoint = new CheckPoint
            {
                Id = -1,
                Name = CheckPointLocation,
                SerialNumber = _temporaryCheckPoints.Count + 1,
                Tour = new Tour(),
                IsActive = false
            };

            ++_currentCheckPoint;
            _temporaryCheckPoints.Add(checkPoint);

            CheckBoxLabel.Content = _currentCheckPoint == 1 ? _messages[0] : _messages[1];
            
            CheckPointTextBox.Clear();
        }

        private void Click_AddTourDate(object sender, RoutedEventArgs e)
        {
            var tourDateInput = DateTextBox.Text;

            if (DateTime.TryParseExact(tourDateInput, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                if (parsedDate >= DateTime.Today)
                {
                    TourDate tourDate = new()
                    {
                        Id = -1,
                        Date = parsedDate,
                        Tour = new Tour()
                    };

                    _temporaryTourDates.Add(tourDate);
                }
                else
                {
                    MessageBox.Show("Tour date cannot be before today's date", "Invalid date", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Date is not in correct format", "Incorrect format", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ComboBoxLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TourLanguage = CboLanguage.SelectedItem.ToString();
        }

        public static List<string> GetDistinctCountries(List<Location> locations)
        {
            return locations.Select(location => location.Country).Distinct().ToList();
        }

        public static List<string> GetDistinctCities(List<Location> locations)
        {
            return locations.Select(location => location.City).Distinct().ToList();
        }

        public static List<Location> FilterLocationsByCountry(List<Location> locations, string country)
        {
            return locations.Where(location => location.Country == country).ToList();
        }

        private void CboCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Country = CboCountry.SelectedItem.ToString();
            Cities = Locations.Where(t => t.Country == Country).Select(t => t.City).ToList();

            CboCity.SelectedItem = Cities[0];
            CboCity.ItemsSource = Cities;
        }

        private void CboCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            City = CboCity.SelectedItem.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Create_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Create();
            Close();
        }

        private void Create_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsValid();
        }

        private bool IsValid()
        {
            return _temporaryTourDates.Count > 0 && Images.Count >= 1 && !string.IsNullOrEmpty(TourName) && !string.IsNullOrEmpty(Description) && (_temporaryCheckPoints.Count + 1 > 2);
        }
    }
}