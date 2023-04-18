using System;
using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.Repository;

namespace SimsProject.WPF.View.Guest1
{
    /// <summary>
    /// Interaction logic for RequestReservationMoveForm.xaml
    /// </summary>
    public partial class RequestReservationMoveForm : Window
    {
        public AccommodationReservation SelectedReservation { get; set;}
        public User CurrentUser { get; set;}
        public AccommodationReservationRescheduleRepository _accommodationReservationMoveRequestRepository;
        public RequestReservationMoveForm(AccommodationReservation selectedReservation, User currentUser)
        {
            InitializeComponent();
            SelectedReservation = selectedReservation;
            CurrentUser= currentUser;
            _accommodationReservationMoveRequestRepository = new AccommodationReservationRescheduleRepository();
        }

        private void RequestClick(object sender, RoutedEventArgs e)
        {
            if (DateOnly.TryParseExact(TbDate1.Text, "dd.MM.yyyy.", out DateOnly date1) && DateOnly.TryParseExact(TbDate2.Text, "dd.MM.yyyy.", out DateOnly date2) && date1 <= date2)
            {
                AccommodationReservationReschedule acmr = new AccommodationReservationReschedule(SelectedReservation, CurrentUser, SelectedReservation.Accommodation.Owner, date1, date2, Status.Waiting, "No comment");
                _accommodationReservationMoveRequestRepository.Save(acmr);
                Close();
            }
            else MessageBox.Show("Invalid date input: dd.MM.yyyy.");
        }
    }
}
