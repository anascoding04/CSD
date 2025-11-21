using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFCSDProject.Models;
using WPFCSDProject.Services;

namespace WPFCSDProject.Views.Controls
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        private readonly IAuthenticationService _authService;

        public event EventHandler? OnRegistrationSuccess;
        public event EventHandler? OnSwitchToLogin;

        public RegisterView()
        {
            InitializeComponent();
            // TODO: Get from dependency injection container
            var dbService = new DatabaseService();
            _authService = new AuthenticationService(dbService);
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string studentNumber = StudentNumberTextBox.Text.Trim();
            string firstName = FirstNameTextBox.Text.Trim();
            string lastName = LastNameTextBox.Text.Trim();
            string courseTitle = CourseTitleTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            // Validation
            if (string.IsNullOrWhiteSpace(studentNumber))
            {
                ShowError("Please enter your student number.");
                return;
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                ShowError("Please enter your first name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                ShowError("Please enter your last name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(courseTitle))
            {
                ShowError("Please enter your course title.");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter a password.");
                return;
            }

            if (password.Length < 6)
            {
                ShowError("Password must be at least 6 characters long.");
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Passwords do not match.");
                return;
            }

            // Clear previous errors
            HideError();

            try
            {
                // Attempt registration
                var student = new Student
                {
                    StudentNumber = studentNumber,
                    FirstName = firstName,
                    LastName = lastName,
                    CourseTitle = courseTitle
                };

                bool success = await _authService.RegisterAsync(student, password);

                if (success)
                {
                    // Registration successful
                    OnRegistrationSuccess?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    ShowError("Registration failed. Student number may already be in use.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"An error occurred: {ex.Message}");
            }
        }

        private void SignInText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OnSwitchToLogin?.Invoke(this, EventArgs.Empty);
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
