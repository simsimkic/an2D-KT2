using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SimsProject.Domain.Model;
using SimsProject.Repository;

namespace SimsProject.WPF.View.Guest1
{
    /// <summary>
    /// Interaction logic for Guest1Overview.xaml
    /// </summary>
    public partial class Guest1Overview : Window
    {
        public User CurrentUser { get; set; }
        public static Accommodation SelectedAccommodation { get; set; }
        public static ObservableCollection<Accommodation> Accommodations { get; set; }
        private readonly AccommodationTypeRepository _accommodationTypeRepository;
        private readonly AccommodationRepository _accommodationRepository;
        private readonly AccommodationReservationRescheduleRepository _accommodationReservationMoveRequestRepository;
        private readonly LocationRepository _locationRepository;
        private readonly ImageRepository _imageRepository;
        private readonly UserRepository _userRepository;
        public Guest1Overview(User currentUser)
        {
            InitializeComponent();
            DataContext = this;

            _imageRepository = new ImageRepository("accommodationImages.csv");
            _accommodationTypeRepository = new AccommodationTypeRepository();
            _accommodationRepository = new AccommodationRepository();
            _accommodationReservationMoveRequestRepository = new AccommodationReservationRescheduleRepository();
            _locationRepository = new LocationRepository();
            _userRepository = new UserRepository();

            CurrentUser = currentUser;
            Accommodations = new ObservableCollection<Accommodation>(_accommodationRepository.GetAll());

            FillTypeComboBox();
            FillCountryComboBox();

            TbCity.IsEnabled = false;
            Notify();
            UpdateTable();
        }

        private void Notify()
        {
            if (_accommodationReservationMoveRequestRepository.CheckNotifyGuest(CurrentUser))
            {
                var lista = _accommodationReservationMoveRequestRepository.GetByGuest(CurrentUser);
                foreach(var l in lista)
                {
                    l.Notify = false;
                    _accommodationReservationMoveRequestRepository.Update(l);
                }
                StatusChangeNotification form = new(CurrentUser);
                form.Show();
            }
        }

        private void FillTypeComboBox() 
        {
            ObservableCollection<AccommodationType> types = new(_accommodationTypeRepository.GetAll());
            types.Insert(0, new(-1, "Any"));
            string[] tempTypes = new string[types.Count];
            for (int i = 0; i < types.Count; i++)
            {
                tempTypes[i] = types[i].Name;
            }
            TbType.ItemsSource = tempTypes;
            TbType.SelectedIndex = 0;
        }

        private void FillCountryComboBox()
        {
            ObservableCollection<string> countries = new(_locationRepository.GetAllCountries());
            countries.Insert(0, "Any");
            string[] tempCountries = new string[countries.Count];
            for (int i = 0; i < countries.Count; i++)
            {
                tempCountries[i] = countries[i];
            }
            TbCountry.ItemsSource = tempCountries;
            TbCountry.SelectedIndex = 0;
        }
        private void PopulateAccommodations()
        {
            foreach (var accommodation in Accommodations)
            {
                accommodation.Location = _locationRepository.GetById(accommodation.Location.Id);
                accommodation.Type = _accommodationTypeRepository.GetById(accommodation.Type.Id);
                accommodation.Owner = _userRepository.GetById(accommodation.Owner.Id);
                accommodation.Images = _imageRepository.GetByParentId(accommodation.Id);
                accommodation.Cover = accommodation.Images[0];
            }
        }

        private bool IsValid(Accommodation accommodation)
        {
            var isNameValid = string.IsNullOrEmpty(TbName.Text) || accommodation.Name.ToLower().Contains(TbName.Text.ToLower());
            var isGuestNumberValid = string.IsNullOrEmpty(TbGuestNumber.Text) || accommodation.MaxGuestNumber >= Convert.ToInt32(TbGuestNumber.Text);
            var isStayLengthValid = string.IsNullOrEmpty(TbStayLength.Text) || accommodation.MinReservationDays <= Convert.ToInt32(TbStayLength.Text);
            var isTypeValid = TbType.SelectedIndex == 0 || accommodation.Type.Id == TbType.SelectedIndex;
            var isCountryValid = TbCountry.SelectedIndex == 0 || _locationRepository.GetById(accommodation.Location.Id).Country == TbCountry.SelectedItem.ToString();
            var isCityValid = TbCity.SelectedIndex == 0 || _locationRepository.GetById(accommodation.Location.Id).City == TbCity.SelectedItem.ToString();

            return isNameValid && isGuestNumberValid && isStayLengthValid && isTypeValid && isCountryValid && isCityValid;
        }

        private void UpdateTable()
        {
            Accommodations.Clear();

            foreach (var accommodation in GetAllAccommodations())
            {
                if (!IsValid(accommodation)) continue;

                if (accommodation.Owner.IsSuperUser)
                {
                    // TODO : mark SuperOwners accommodations - a star, color...?
                    Accommodations.Insert(0, accommodation);
                }
                else
                {
                    Accommodations.Add(accommodation);
                }
                PopulateAccommodations();
            }
        }

        private List<Accommodation> GetAllAccommodations()
        {
            var allAccommodations = _accommodationRepository.GetAll();
            foreach (var accommodation in allAccommodations)
            {
                accommodation.Owner = _userRepository.GetById(accommodation.Owner.Id);
            }

            return allAccommodations;
        }

        private bool IsInputValid()
        {
            return (string.IsNullOrEmpty(TbGuestNumber.Text) || int.TryParse(TbGuestNumber.Text, out _))
                && (string.IsNullOrEmpty(TbStayLength.Text) || int.TryParse(TbStayLength.Text, out _));
        }


        private void SearchClick(object sender, RoutedEventArgs e)
        {
            if (IsInputValid())
            {
                UpdateTable();
            }
        }

        private void ReserveClick(object sender, RoutedEventArgs e)
        {
            if (SelectedAccommodation != null)
            {
                ReserveAccommodationForm form = new(SelectedAccommodation, CurrentUser);
                form.ShowDialog();
            }
            else
            {
                MessageBox.Show("Select accommodation.");
            }
        }

        private void YourReservationsClick(object sender, RoutedEventArgs e)
        {
            YourReservationsForm form = new(CurrentUser);
            form.ShowDialog();
        }

        private void ReviewClick(object sender, RoutedEventArgs e)
        {
            ReviewsForm form = new(CurrentUser);
            form.ShowDialog();
        }

        private void AnywhereAnytimeClick(object sender, RoutedEventArgs e)
        {

        }

        private void ForumsClick(object sender, RoutedEventArgs e)
        {

        }

        private void HelpClick(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeThemeClick(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeLanguageClick(object sender, RoutedEventArgs e)
        {

        }

        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = ConfirmLogout();
            if (result == MessageBoxResult.Yes)
            {
                System.Windows.Application.Current.Windows.OfType<Window>()
                    .Where(w => w != this)
                    .ToList()
                    .ForEach(w => w.Close());

                LogInForm logInForm = new LogInForm();
                logInForm.Show();

                Close();
            }
        }

        private static MessageBoxResult ConfirmLogout()
        {
            string sMessageBoxText = "Are you sure you want to logout?";
            string sCaption = "Logout";

            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            MessageBoxImage icnMessageBox = MessageBoxImage.Question;

            MessageBoxResult result = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            return result;
        }

        private void Country_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TbCountry.SelectedIndex != 0) TbCity.IsEnabled = true;
            else TbCity.IsEnabled = false;
            ObservableCollection<Location> cities = new(_locationRepository.GetAllCitiesByCountry(TbCountry.SelectedItem.ToString()));
            cities.Insert(0, new("Any", ""));
            string[] tempCities = new string[cities.Count];
            for (int i = 0; i < cities.Count; i++)
            {
                tempCities[i] = cities[i].City;
            }
            TbCity.ItemsSource = tempCities;
            TbCity.SelectedIndex = 0;
        }

        private void OuterGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
