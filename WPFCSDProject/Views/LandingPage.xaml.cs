using System.Windows;
using System.Windows.Controls;
using WPFCSDProject.Services;
using WPFCSDProject.Views.Controls;

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
            var loginView = new LoginView();
            loginView.OnLoginSuccess += (s, e) => NavigateToDashboard();
            loginView.OnSwitchToRegister += (s, e) => ShowRegistrationView();
            AuthContentControl.Content = loginView;
        }

        public void ShowRegistrationView()
        {
            var registrationView = new RegisterView();
            registrationView.OnRegistrationSuccess += (s, e) => ShowLoginView();
            registrationView.OnSwitchToLogin += (s, e) => ShowLoginView();
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

