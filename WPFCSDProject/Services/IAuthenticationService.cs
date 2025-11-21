using WPFCSDProject.Models;

namespace WPFCSDProject.Services
{
    public interface IAuthenticationService
    {
        Task<bool> LoginAsync(string studentNumber, string password);
        Task<bool> RegisterAsync(Student student, string password);
        Task<bool> IsStudentApprovedAsync(string studentNumber);
        void Logout();
        void SetMockStudent(Student student);
        Student? CurrentStudent { get; }
        bool IsAuthenticated { get; }
    }
}

