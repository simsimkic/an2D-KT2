using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using SimsProject.Domain.Model;
using SimsProject.Repository;
using Image = SimsProject.Domain.Model.Image;

namespace SimsProject.WPF.View.Owner
{
    /// <summary>
    /// Interaction logic for AccommodationRegistrationForm.xaml
    /// </summary>
    public partial class AccommodationRegistrationForm : Window, INotifyPropertyChanged
    {

        public ObservableCollection<Image> Images { get; set; }
        public ObservableCollection<AccommodationType> Types { get; set; }
        public List<Location> Locations { get; set; }
        public List<string> Countries { get; set; }
        public List<string> Cities { get; set; }

        public User AccommodationOwner { get; set; }

        private AccommodationRepository _accommodationRepository;
        private LocationRepository _locationRepository;
        private ImageRepository _imageRepository;

        private string _name;
        private string _city;
        private string _country;
        private AccommodationType _type;
        private int _maxGuestNumber;
        private int _minReservationDays;
        private int _minDaysBeforeCancellation;
        public string AccommodationName
        {
            get => _name;
            set
            {
                if (value != _name)
                {
                    _name = value;
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
        public AccommodationType Type
        {
            get => _type;
            set
            {
                if (value != _type)
                {
                    _type = value;
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
        public int MinReservationDays
        {
            get => _minReservationDays;
            set
            {
                if (value != _minReservationDays)
                {
                    _minReservationDays = value;
                    OnPropertyChanged();
                }
            }
        }
        public int MinDaysBeforeCancellation
        {
            get => _minDaysBeforeCancellation;
            set
            {
                if (value != _minDaysBeforeCancellation)
                {
                    _minDaysBeforeCancellation = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AccommodationRegistrationForm(User owner)
        {
            InitializeComponent();
            DataContext = this;
            AccommodationOwner = owner;

            InitializeRepositories();
            InitializeCollections();
            GetLocations();
            SetDefaultValues();
        }

        private void InitializeCollections()
        {
            Locations = new List<Location>(_locationRepository.GetAll());
            Types = new ObservableCollection<AccommodationType>(new AccommodationTypeRepository().GetAll());
            Images = new ObservableCollection<Image>();
        }

        private void InitializeRepositories()
        {
            _accommodationRepository = new AccommodationRepository();
            _locationRepository = new LocationRepository();
            _imageRepository = new ImageRepository("accommodationImages.csv");
        }

        private void GetLocations()
        {
            Countries = GetDistinctCountries(Locations);
            var filteredLocations = FilterLocationsByCountry(Locations, Countries[0]);
            Cities = GetDistinctCities(filteredLocations);
        }

        private void SetDefaultValues()
        {
            MinDaysBeforeCancellation = 1;
            CboCountry.SelectedItem = Countries[0];
            CboCity.SelectedItem = Cities[0];
            Type = Types[0];
        }

        private void Register()
        {
            Location location = _locationRepository.GetByCityAndCountry(City, Country);
            Accommodation newAccommodation = new(AccommodationName, location, Type, MaxGuestNumber, MinReservationDays, MinDaysBeforeCancellation, AccommodationOwner, Images.ToList());
            Accommodation savedAccommodation= _accommodationRepository.Save(newAccommodation);
            _imageRepository.SaveAllByParentId(savedAccommodation.Id, Images.ToList());
            OwnerOverview.Accommodations.Add(savedAccommodation);
        }

        private void Cancel(object sender, RoutedEventArgs e)
        { 
            Close();
        }

        private void UploadImagesFromComputer(object sender, RoutedEventArgs e)
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
        
        private void UploadImagesFromUrl(object sender, RoutedEventArgs e)
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
            string pattern = @"^(http|https)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$";
            return Regex.IsMatch(url, pattern);
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

        private void CboCountrySelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Country = CboCountry.SelectedItem.ToString();

            Cities = Locations.Where(t => t.Country == Country).Select(t => t.City).ToList();

            CboCity.SelectedItem = Cities[0];
            CboCity.ItemsSource = Cities;
        }

        private void CboCitySelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            City = CboCity.SelectedItem.ToString();
        }

        private void ExecuteRegister(object sender, ExecutedRoutedEventArgs e)
        {
            Register();
            Close();
        }

        private void CanRegisterExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsFormValid();
        }

        private bool IsFormValid()
        {
            return Images.Count >= 1 && !string.IsNullOrEmpty(AccommodationName);
        }
    }
}
