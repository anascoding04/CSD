using WPFCSDProject.Models;

namespace WPFCSDProject.Services
{
    public interface IDatabaseService
    {
        Task<Student?> GetStudentByNumberAsync(string studentNumber);
        Task<bool> CreateStudentAsync(Student student);
        Task<bool> UpdateStudentAsync(Student student);
        Task<List<Test>> GetAvailableTestsAsync(int studentId);
        Task<List<TestAttempt>> GetCompletedTestsAsync(int studentId);
        Task<List<Question>> GetRandomQuestionsForTestAsync(int testId, int count);
        Task<bool> SaveTestAttemptAsync(TestAttempt attempt);
        Task<Test?> GetTestByIdAsync(int testId);
        Task<List<(string SubjectName, List<(string StudentName, int Points)>)>> GetLeaderboardDataAsync();
    }
}

