using SimsProject.Domain.Model;
using SimsProject.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Image = SimsProject.Domain.Model.Image;


namespace SimsProject.WPF.View.Guest1
{
    /// <summary>
    /// Interaction logic for ReviewAccommodation.xaml
    /// </summary>
    public partial class ReviewAccommodationForm : Window
    {
        public static ObservableCollection<Image> Images { get; set; }
        public AccommodationReservation Reservation { get; set; }
        public User CurrentUser { get; set; }
        private ImageRepository _imageRepository;
        public ReviewAccommodationForm(AccommodationReservation reservation, User currentUser)
        {
            InitializeComponent();
            Reservation = reservation;
            CurrentUser = currentUser;
            Images = new ObservableCollection<Image>();
            _imageRepository = new ImageRepository("accommodationReviewImages.csv");
        }

        private void SubmitReviewClick(object sender, RoutedEventArgs e)
        {
            RadioButton selectedRadioButton1 = null;
            foreach (var radioButton in ((StackPanel)rb1.Content).Children.OfType<RadioButton>())
            {
                if (radioButton.IsChecked == true)
                {
                    selectedRadioButton1 = radioButton;
                    break;
                }
            }
            RadioButton selectedRadioButton2 = null;
            foreach (var radioButton in ((StackPanel)rb2.Content).Children.OfType<RadioButton>())
            {
                if (radioButton.IsChecked == true)
                {
                    selectedRadioButton2 = radioButton;
                    break;
                }
            }
            int selectedOption1 = 0, selectedOption2 = 0;
            if (selectedRadioButton1 != null)
            {
                selectedOption1 = Convert.ToInt32(selectedRadioButton1.Tag);
            }
            if (selectedRadioButton2 != null)
            {
                selectedOption2 = Convert.ToInt32(selectedRadioButton2.Tag);
            }
            AccommodationReview rev = new(selectedOption1, selectedOption2, TbComment.Text, Reservation.Accommodation.Owner, CurrentUser, Reservation, null); // TODO : images
            AccommodationReviewRepository _ar = new();
            _ar.Save(rev);
            _imageRepository.SaveAllByParentId(rev.Id, Images.ToList());
            ReviewsForm.AccommodationReservations.Remove(Reservation);
            Close();
        }

        private static bool IsUrlValid(string url)
        {
            var pattern = @"^(http|https)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$";
            return Regex.IsMatch(url, pattern);
        }

        private void AddImageClick(object sender, RoutedEventArgs e)
        {
            var url = TbPicture.Text;
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
            TbPicture.Text = "";
        }

        private void ViewImageClick(object sender, RoutedEventArgs e)
        {
            if (Images.Count == 0)
            {
                MessageBox.Show("No images added.");
                return;
            }
            ImageViewer form = new(Images, true);
            form.ShowDialog();
        }
    }
}
