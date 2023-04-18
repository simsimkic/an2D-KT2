using SimsProject.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimsProject.WPF.View.Guest1
{
    /// <summary>
    /// Interaction logic for StatusChangeNotification.xaml
    /// </summary>
    public partial class StatusChangeNotification : Window
    {
        private User CurrentUser { get; set; }
        public StatusChangeNotification(User currentUser)
        {
            InitializeComponent();
            DataContext = this;
            CurrentUser = currentUser;
        }
        private void ViewStatusClick(object sender, RoutedEventArgs e)
        {
            Close();
            StatusOfMoveRequestsForm form = new(CurrentUser);
            form.ShowDialog();
        }
    }
}
