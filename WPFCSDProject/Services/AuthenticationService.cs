using BCrypt.Net;
using WPFCSDProject.Models;

namespace WPFCSDProject.Services
{
    /// <summary>
    /// Service for handling authentication operations.
    /// This will connect to a remote database in the future.
    /// 
    /// GDPR Compliance Notes:
    /// - Passwords are hashed before storage using BCrypt
    /// - Student data is stored securely on remote server
    /// - No local storage of sensitive credentials
    /// - User consent is obtained during registration
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IDatabaseService _databaseService;
        private Student? _currentStudent;

        public AuthenticationService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public Student? CurrentStudent => _currentStudent;
        public bool IsAuthenticated => _currentStudent != null && _currentStudent.IsApproved;

        /// <summary>
        /// Attempts to log in a student with the provided credentials.
        /// </summary>
        public async Task<bool> LoginAsync(string studentNumber, string password)
        {
            var student = await _databaseService.GetStudentByNumberAsync(studentNumber);
            
            if (student == null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(password, student.PasswordHash))
                return false;

            if (!student.IsApproved)
                return false;

            _currentStudent = student;
            return true;
        }

        /// <summary>
        /// Registers a new student account.
        /// New accounts require lecturer approval before they can access the system.
        /// </summary>
        public async Task<bool> RegisterAsync(Student student, string password)
        {
            // Check if student number already exists
            var existing = await _databaseService.GetStudentByNumberAsync(student.StudentNumber);
            if (existing != null)
                return false;

            // Hash password using BCrypt (GDPR-compliant)
            student.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
            student.IsApproved = false; // Requires lecturer approval
            student.CreatedAt = DateTime.UtcNow;

            return await _databaseService.CreateStudentAsync(student);
        }

        public async Task<bool> IsStudentApprovedAsync(string studentNumber)
        {
            var student = await _databaseService.GetStudentByNumberAsync(studentNumber);
            return student?.IsApproved ?? false;
        }

        public void Logout()
        {
            _currentStudent = null;
        }

        /// <summary>
        /// Sets a mock student for development/testing purposes.
        /// </summary>
        public void SetMockStudent(Student student)
        {
            _currentStudent = student;
        }
    }
}

