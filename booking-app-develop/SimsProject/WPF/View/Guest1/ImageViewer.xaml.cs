using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Image = SimsProject.Domain.Model.Image;

namespace SimsProject.WPF.View.Guest1
{
    /// <summary>
    /// Interaction logic for ImageViewer.xaml
    /// </summary>
    public partial class ImageViewer : Window, INotifyPropertyChanged
    {
        private Image _selectedImage;
        public Image SelectedImage
        {
            get => _selectedImage;
            set
            {
                if (value != _selectedImage)
                {
                    _selectedImage = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<Image> Images { get; set; }
        int index = 0;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ImageViewer(ObservableCollection<Image> images, bool isDeleteable)
        {
            InitializeComponent();
            DataContext = this;
            if (isDeleteable) RemoveButton.Visibility = Visibility.Visible;
            Images = images;
            SelectedImage = Images[index];
        }

        private void PreviousImageClick(object sender, RoutedEventArgs e)
        {
            if (index == 0)
            {
                index = Images.Count - 1;
            }
            else
            {
                index--;
            }
            SelectedImage = Images[index];
        }

        private void NextImageClick(object sender, RoutedEventArgs e)
        {
            index = (index + 1) % Images.Count;
            SelectedImage = Images[index];
        }

        private void RemoveImageClick(object sender, RoutedEventArgs e)
        {
            Images.Remove(SelectedImage);
            ReviewAccommodationForm.Images.Remove(SelectedImage);
            if(Images.Count == 0)
            {
                Close();
                return;
            }
            index = (index + 1) % Images.Count;
            SelectedImage = Images[index];
        }
    }
}





