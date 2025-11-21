using System.Configuration;
using System.Data;
using System.Windows;
using WPFCSDProject.Services;
using WPFCSDProject.Views;

namespace WPFCSDProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IAuthenticationService? _authService;

        private void App_Startup(object sender, StartupEventArgs e)
        {
            // Initialize authentication service
            var dbService = new DatabaseService();
            _authService = new AuthenticationService(dbService);

            // Create mock student for development (login will be handled by colleague)
            var mockStudent = new WPFCSDProject.Models.Student
            {
                Id = 1,
                StudentNumber = "STU001",
                FirstName = "Student",
                LastName = "User",
                CourseTitle = "DATS",
                IsApproved = true,
                TotalPoints = 0
            };
            _authService.SetMockStudent(mockStudent);

            // Go directly to dashboard
            var dashboard = new WPFCSDProject.Dashboard.DashboardWindow(_authService);
            dashboard.Show();
        }
    }

}
