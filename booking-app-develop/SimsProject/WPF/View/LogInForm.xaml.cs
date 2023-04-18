using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.Repository;
using SimsProject.WPF.View.Guest1;
using SimsProject.WPF.View.Guide;
using SimsProject.WPF.View.Owner;
using SimsProject.WPF.View.Guest2View;

namespace SimsProject.WPF.View
{
    /// <summary>
    /// Interaction logic for LogInForm.xaml
    /// </summary>
    public partial class LogInForm : Window, INotifyPropertyChanged
    {

        private readonly UserRepository _repository;

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                if (value != _username)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public LogInForm()
        {
            InitializeComponent();
            DataContext = this;
            _repository = new UserRepository();
        }

        private void LogIn(object sender, RoutedEventArgs e)
        {
            User user = _repository.GetByUsername(Username);
            if (user != null)
            {
                if (user.Password == TxtPassword.Password)
                {
                    OpenOverview(user);
                    Close();
                }
                else
                { 
                    MessageBox.Show("Wrong password!");
                }
            }
            else
            {
                MessageBox.Show("User not found!");
            }
        }

        private static void OpenOverview(User user)
        {
            Window overview;

            switch (user.Type)
            {
                case UserType.Owner:
                    overview = new OwnerOverview(user);
                    break;
                case UserType.Guide:
                    overview = new GuideOverview(user);
                    break;
                case UserType.Guest1:
                    overview = new Guest1Overview(user);
                    break;
                case UserType.Guest2:
                    overview = new Guest2Overview(user);
                    break;
                default:
                    MessageBox.Show("Invalid user type.");
                    return;
            }

            overview.Show();
        }

        private void SignUp(object sender, RoutedEventArgs e)
        {
            SignUpForm signUpForm = new();
            signUpForm.ShowDialog();
        }
    }
}
