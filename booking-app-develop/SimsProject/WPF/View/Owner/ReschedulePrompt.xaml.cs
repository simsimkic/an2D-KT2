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

namespace SimsProject.WPF.View.Owner
{
    /// <summary>
    /// Interaction logic for ReschedulePrompt.xaml
    /// </summary>
    public partial class ReschedulePrompt : Window
    {
        public ReschedulePrompt()
        {
            InitializeComponent();
        }
        public string ResponseText
        {
            get => TxtResponse.Text;
            set => TxtResponse.Text = value;
        }

        private void RejectReschedule(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
