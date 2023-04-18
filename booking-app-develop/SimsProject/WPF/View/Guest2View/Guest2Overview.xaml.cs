using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.Repository;
using System.Windows.Navigation;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Controls;
using SimsProject.WPF.View.Guest2View;

namespace SimsProject.WPF.View.Guest2View
{
    /// <summary>
    /// Interaction logic for GuideOverview.xaml
    /// </summary>
    public partial class Guest2Overview : Window
    {

        
        public User LoggedInUser { get; set; }
        public List<TourAttendance> TourAttendances { get; set; }
        private readonly TourAttendanceRepository _tourAttendanceRepository;
        




        public Guest2Overview(User user)
        {
            InitializeComponent();
           // this.DataContext = this;
            LoggedInUser = user;
            if (LV.SelectedIndex == 1)
            {
                MyReservations reservations = new MyReservations(LoggedInUser);
                 Frame.Content = reservations;
            }
            _tourAttendanceRepository = new TourAttendanceRepository();
            TourAttendances = new List<TourAttendance>(_tourAttendanceRepository.GetAll());


        }

        private void GetNotifications()
        {
            foreach (var attendance in TourAttendances)
            {
                if (attendance.User.Id.Equals(LoggedInUser.Id) && attendance.Present == Presence.GuideConfirmed)
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show($"Are you present on {attendance.Tour.Name}?", "Confirm your presence", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        attendance.Present = Presence.GuestConfirmed;
                        _tourAttendanceRepository.Update(attendance);
                    }
                    else
                    {
                        attendance.Present = Presence.GuestNotPresent;
                        _tourAttendanceRepository.Update(attendance);
                    }
                }
            }
        }

        

        private void Logout(object sender, RoutedEventArgs e)
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

            MessageBoxResult result = System.Windows.MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);
            return result;
        }

        

        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                GetNotifications();
            }));
        }
        

        private void ListViewItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Set tooltip visibility
            if (Tg_Btn.IsChecked == true)
            {
                tt_home.Visibility = Visibility.Collapsed;
                tt_contacts.Visibility = Visibility.Collapsed;
                tt_messages.Visibility = Visibility.Collapsed;
                tt_maps.Visibility = Visibility.Collapsed;
                tt_settings.Visibility = Visibility.Collapsed;

               
            }
            else
            {
                tt_home.Visibility = Visibility.Visible;
                tt_contacts.Visibility = Visibility.Visible;
                tt_messages.Visibility = Visibility.Visible;
                tt_maps.Visibility = Visibility.Visible;
                tt_settings.Visibility = Visibility.Visible;
              
            }
        }

        private void Tg_Btn_Unchecked(object sender, RoutedEventArgs e)
        {
            img_bg.Opacity = 1;
        }

        private void Tg_Btn_Checked(object sender, RoutedEventArgs e)
        {
            img_bg.Opacity = 0.3;
        }

        private void BG_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Tg_Btn.IsChecked = false;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

       

        private void HomeClicked(object sender, MouseButtonEventArgs e)
        {
            var item = sender as System.Windows.Controls.ListViewItem;
            if(item!= null && item.IsSelected && item.Tag.Equals("MyReservations"))
            {
                MyReservations reservations = new MyReservations(LoggedInUser);
                Frame.Content = reservations;
            }


        }
        private void SearchClicked(object sender, MouseButtonEventArgs e)
        {
            var item = sender as System.Windows.Controls.ListViewItem;
            if (item != null && item.IsSelected && item.Tag.Equals("Search"))
            {
                SearchTourWindow search = new SearchTourWindow(LoggedInUser);
                Frame.Content = search;
            }

        }
    }
}