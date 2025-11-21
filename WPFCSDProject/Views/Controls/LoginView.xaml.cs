using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFCSDProject.Config;
using WPFCSDProject.Services;

namespace WPFCSDProject.Views.Controls
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        private readonly IAuthenticationService _authService;

        public event EventHandler? OnLoginSuccess;
        public event EventHandler? OnSwitchToRegister;

        public LoginView()
        {
            InitializeComponent();
            // TODO: Get from dependency injection container
            var dbService = new DatabaseService(DatabaseConfig.GetConnectionString());
            _authService = new AuthenticationService(dbService);
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            // Validation
            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError("Please enter your username.");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter your password.");
                return;
            }

            // Clear previous errors
            HideError();

            try
            {
                // Attempt login
                bool success = await _authService.LoginAsync(username, password);

                if (success)
                {
                    // Check if student is approved
                    bool isApproved = await _authService.IsStudentApprovedAsync(username);
                    
                    if (!isApproved)
                    {
                        ShowError("Your account is pending approval by a lecturer. Please wait for approval before accessing the system.");
                        return;
                    }

                    // Login successful
                    OnLoginSuccess?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    ShowError("Invalid username or password. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"An error occurred: {ex.Message}");
            }
        }

        private void SignUpText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OnSwitchToRegister?.Invoke(this, EventArgs.Empty);
        }

        private void ShowError(string message)
        {
            ErrorMessageTextBlock.Text = message;
            ErrorMessageTextBlock.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
            ErrorMessageTextBlock.Text = string.Empty;
        }
    }
}
