using System.Windows;
using System.Windows.Controls;
using WPFCSDProject.Services;

namespace WPFCSDProject.Views
{
    public partial class LandingPage : Window
    {
        private readonly IAuthenticationService _authService;

        public LandingPage(IAuthenticationService authService)
        {
            InitializeComponent();
            _authService = authService;
            ShowLoginView();
        }

        public void ShowLoginView()
        {
            var loginView = new LoginView(_authService, this);
            AuthContentControl.Content = loginView;
        }

        public void ShowRegistrationView()
        {
            var registrationView = new RegistrationView(_authService, this);
            AuthContentControl.Content = registrationView;
        }

        public void NavigateToDashboard()
        {
            var dashboard = new Dashboard.DashboardWindow(_authService);
            dashboard.Show();
            this.Close();
        }
    }
}

