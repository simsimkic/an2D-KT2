using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using SimsProject.Domain.Model;
using SimsProject.Repository;

namespace SimsProject.WPF.View
{
    /// <summary>
    /// Interaction logic for SignUpForm.xaml
    /// </summary>
    public partial class SignUpForm : Window, INotifyPropertyChanged
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
        public string[] UserTypes { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SignUpForm()
        {
            InitializeComponent();
            this.DataContext = this;

            _repository = new UserRepository();

            SetUserTypes();
        }

        private void SetUserTypes()
        {
            UserTypes = new[] { "AccommodationOwner", "Guide", "Guest1", "Guest2" };
            CboUserTypes.SelectedIndex = 0;
        }

        private void SignUp(object sender, RoutedEventArgs e)
        {
            User user = _repository.GetByUsername(Username);
            if (user == null)
            {
                User newUser = new(Username, TxtPassword.Password, (UserType)CboUserTypes.SelectedIndex);
                _repository.Save(newUser);
                MessageBox.Show("Sign up successful!");
                Close();
            }
            else
            {
                MessageBox.Show("Username already taken!");
            }
        }
    }
}
