using System;
using System.Collections.ObjectModel;
using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.Repository;

namespace SimsProject.WPF.View.Guest1
{
    /// <summary>
    /// Interaction logic for ReserveAccommodationForm.xaml
    /// </summary>
    public partial class ReserveAccommodationForm : Window
    {
        public DateRange SelectedDateRange { get; set; }
        public ObservableCollection<DateRange> ValidDays { get; set; }
        public Accommodation selectedAccommodation;
        public User currentUser;
        public AccommodationReservationRepository accommodationReservationRepository;
        public ObservableCollection<AccommodationReservation> accommodationReservations;
        public int lenghtOfStay, guestNumber;
        public DateOnly date1, date2;

        public ReserveAccommodationForm(Accommodation selectedAccommodation, User currentUser)
        {
            InitializeComponent();
            DataContext = this;
            this.selectedAccommodation = selectedAccommodation;
            this.currentUser = currentUser;
            accommodationReservationRepository = new AccommodationReservationRepository();
            accommodationReservations = new(accommodationReservationRepository.GetByAccommodation(selectedAccommodation));
            ValidDays = new ObservableCollection<DateRange>();
        }
        private bool IsImputValid()
        {
            if (int.TryParse(TbLenghtOfStay.Text, out lenghtOfStay) && int.TryParse(TbGuestNumber.Text, out guestNumber))
            {
                if (lenghtOfStay >= selectedAccommodation.MinReservationDays)
                {
                    if (guestNumber <= selectedAccommodation.MaxGuestNumber)
                    {
                        if (DateOnly.TryParseExact(TbDate1.Text, "dd.MM.yyyy.", out date1) && DateOnly.TryParseExact(TbDate2.Text, "dd.MM.yyyy.", out date2))
                        {
                            if (date1 <= date2)
                            {
                                if (date1 > DateOnly.FromDateTime(DateTime.Today))
                                {
                                    MessageBox.Show("Available dates listed.");
                                    return true;
                                }
                                MessageBox.Show("Arrival date in past.");
                                return false;
                            }
                            MessageBox.Show("Error: Dates not in order.");
                            return false;
                        }
                        MessageBox.Show("Error: Date format.");
                        return false;
                    }
                    MessageBox.Show("Error: Guest limit.");
                    return false;
                }
                MessageBox.Show("Error: Min reservation days.");
                return false;
            }
            MessageBox.Show("Error: Input not valid.");
            return false;
        }

        private bool IsDateAvailable(DateOnly date)
        {
            foreach (AccommodationReservation ar in accommodationReservations)
            {
                if (ar.IsCanceled) return true;
                DateOnly arrivalDay = ar.ArrivalDate.AddDays(-lenghtOfStay + 1);
                DateOnly departureDay = ar.ArrivalDate.AddDays(ar.StayLength - 1);
                if (date >= arrivalDay && date <= departureDay)
                {
                    return false;
                }
            }
            return true;
        }

        private void SearchDatesClick(object sender, RoutedEventArgs e)
        {
            ValidDays.Clear();
            if (!IsImputValid())
            {
                return;
            }
            while (date1 <= date2.AddDays(-lenghtOfStay + 1))
            {
                if (IsDateAvailable(date1))
                {
                    DateRange validDate = new(date1, date1.AddDays(lenghtOfStay - 1));
                    ValidDays.Add(validDate);
                }
                date1 = date1.AddDays(1);
            }
            if (ValidDays.Count == 0)
            {
                MessageBox.Show("No available in searched range. Recomendations listed.");
                int recomendedDay = 0;
                while (recomendedDay < 10)
                {
                    if (IsDateAvailable(date2))
                    {
                        recomendedDay++;
                        DateRange validDate = new(date2, date2.AddDays(lenghtOfStay - 1));
                        ValidDays.Add(validDate);
                    }
                    date2 = date2.AddDays(1);
                }
            }
        }

        private MessageBoxResult ConfirmReservation()
        {
            string sMessageBoxText = $"Are you sure you want to reserve {SelectedDateRange} ?";
            string sCaption = "Confirmation";
            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            return MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox);
        }

        private void ReserveClick(object sender, RoutedEventArgs e)
        {
            if (SelectedDateRange != null)
            {
                MessageBoxResult result = ConfirmReservation();
                if (result == MessageBoxResult.Yes)
                {
                    AccommodationReservation newReservation = new(selectedAccommodation, currentUser, SelectedDateRange.StartDate, lenghtOfStay, guestNumber);
                    accommodationReservationRepository.Save(newReservation);
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Select dates for reservation.");
            }
        }
    }
}
