using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.Repository;

namespace SimsProject.WPF.View
{
    /// <summary>
    /// Interaction logic for ReserveTourForm.xaml
    /// </summary>
    public partial class ReserveTourForm : Window
    {

        public Tour selectedTour;

        public int Occupacy;
        public User currentUser;
        public TourReservationRepository tourReservationRepository;
        public ObservableCollection<TourReservation> tourReservations;
        private readonly TourDateRepository _tourDateRepository;
        public List<TourDate> TourDates { get; set; }
        public ReserveTourForm(Tour selectedTour, User currentUser)
        {
            InitializeComponent();
            DataContext = this;
            this.selectedTour = selectedTour;
            this.currentUser = currentUser;
            tourReservationRepository = new TourReservationRepository();
            _tourDateRepository = new TourDateRepository();
            TourDates = _tourDateRepository.GetByParentId(selectedTour.Id);
            tourReservations = new(tourReservationRepository.GetAll());
            foreach (TourDate tourDate in TourDates)
            {
                if (tourDate.Date != null)
                {
                    string dateString = ((DateTime)tourDate.Date).ToString("dd/MM/yyyy HH:mm:ss ", CultureInfo.CurrentCulture);
                    DatesComboBox.Items.Add(dateString);
                }

            }

        }

        private MessageBoxResult ConfirmReservation()
        {
            string sMessageBoxText = $"Are you sure you want to reserve  ?";
            string sCaption = "Confirmation";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            return MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox);
        }

        private int CurrentTourOccupacy(Tour selectedTour) {

            int sum = 0;
            foreach (TourReservation t in tourReservations) {
                if (t.Tour.Id == selectedTour.Id && t.Date == DateTime.Parse(DatesComboBox.SelectedItem.ToString())) {

                    sum += t.GuestNumber;
                }
            }

            return sum;
        }
        private int CountFreePlace(Tour tour) {
            int result = (int)(tour.MaxGuestNumber - CurrentTourOccupacy(selectedTour));
            return result;
        }

        private bool isTourValid(){

            return selectedTour.MaxGuestNumber > Convert.ToInt32(TextBoxGuests.Text) && Convert.ToInt32(TextBoxGuests.Text) <= CountFreePlace(selectedTour);

        }
        private void ReserveClick(object sender, RoutedEventArgs e)
        {
            if (isTourValid())
            {
                MessageBoxResult result = ConfirmReservation();
                if (result == MessageBoxResult.Yes)
                {


                    if (DatesComboBox.SelectedItem != null)
                    {
                        TourReservation newReservation = new(selectedTour, currentUser, Convert.ToInt32(TextBoxGuests.Text), DateTime.Parse(DatesComboBox.SelectedItem.ToString()),Convert.ToInt32(TextBoxAge.Text));
                        tourReservationRepository.Save(newReservation);
                        Close();
                    }
                }
            }
            
            else
            {
                if (CountFreePlace(selectedTour)==0) {
                    MessageBoxResult result = TourIsFull();
                    if (result == MessageBoxResult.Yes)
                    {
                        RecommendedTours recommended = new RecommendedTours(currentUser, selectedTour);
                        recommended.Show();
                    }
                }
                else {
                    int Occupacy = CountFreePlace(selectedTour);
                    MessageBox.Show(" There is no place for entered Guest Number. Current free places: " + Occupacy);
                }
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private MessageBoxResult TourIsFull()
        {
            string sMessageBoxText = $"Do you want to see other tours on same location ?";
            string sCaption = "NO FREE PLACE ON SELECTED TOUR FOR SELECTED DATE!";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            return MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox);
        }
    }
}
