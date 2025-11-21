using MySql.Data.MySqlClient;
using System.Linq;
using WPFCSDProject.Models;

namespace WPFCSDProject.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly string _connectionString;
        
        // In-memory storage for test attempts during development
        private static readonly List<TestAttempt> _testAttempts = new List<TestAttempt>();
        private static int _nextAttemptId = 100; // Start from 100 to avoid conflicts with mock data
        private static bool _isInitialized = false;
        private static readonly object _lockObject = new object();

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
            
            // Initialize with some mock data only once
            lock (_lockObject)
            {
                if (!_isInitialized && _testAttempts.Count == 0)
                {
                    InitializeMockAttempts();
                    _isInitialized = true;
                }
            }
        }
        
        private void InitializeMockAttempts()
        {
            // Add some initial mock data
            _testAttempts.Add(new TestAttempt
            {
                Id = 1,
                StudentId = 1,
                TestId = 5,
                CompletedAt = DateTime.UtcNow.AddDays(-7),
                CorrectAnswers = 8,
                TotalQuestions = 10,
                PointsEarned = 240,
                Percentage = 80.0
            });
            
            _testAttempts.Add(new TestAttempt
            {
                Id = 2,
                StudentId = 1,
                TestId = 6,
                CompletedAt = DateTime.UtcNow.AddDays(-4),
                CorrectAnswers = 6,
                TotalQuestions = 8,
                PointsEarned = 180,
                Percentage = 75.0
            });
        }

        public async Task<Student?> GetStudentByNumberAsync(string studentNumber)
        {
            // Placeholder implementation - will be connected to remote database
            // For now, return null to indicate not found
            // TODO: Implement actual database query when credentials are provided
            await Task.CompletedTask;
            return null;
        }

        public async Task<bool> CreateStudentAsync(Student student)
        {
            // Placeholder implementation
            // TODO: Implement actual database insert when credentials are provided
            await Task.CompletedTask;
            return true;
        }

        public async Task<bool> UpdateStudentAsync(Student student)
        {
            // Placeholder implementation
            await Task.CompletedTask;
            return true;
        }

        public async Task<List<Test>> GetAvailableTestsAsync(int studentId)
        {
            await Task.CompletedTask;
            
            // Mock data for development - returns tests that haven't been completed
            var allTests = new List<Test>
            {
                new Test
                {
                    Id = 1,
                    TestName = "OOP Fundamentals Quiz",
                    SubjectName = "Object-Oriented Programming",
                    LecturerId = 1,
                    QuestionCount = 10,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    IsActive = true
                },
                new Test
                {
                    Id = 2,
                    TestName = "Data Structures Assessment",
                    SubjectName = "Data Structures",
                    LecturerId = 1,
                    QuestionCount = 8,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    IsActive = true
                },
                new Test
                {
                    Id = 3,
                    TestName = "Algorithm Design Test",
                    SubjectName = "Algorithms",
                    LecturerId = 2,
                    QuestionCount = 12,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    IsActive = true
                },
                new Test
                {
                    Id = 4,
                    TestName = "Database Concepts Quiz",
                    SubjectName = "Database Systems",
                    LecturerId = 2,
                    QuestionCount = 10,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    IsActive = true
                }
            };
            
            // Return tests that the student hasn't completed yet
            // In a real implementation, this would filter by checking TestAttempts
            return allTests;
        }

        public async Task<List<TestAttempt>> GetCompletedTestsAsync(int studentId)
        {
            await Task.CompletedTask;
            
            // Return test attempts for the specified student from in-memory storage
            lock (_lockObject)
            {
                return _testAttempts.Where(ta => ta.StudentId == studentId).ToList();
            }
        }
        
        public async Task<Test?> GetTestByIdAsync(int testId)
        {
            await Task.CompletedTask;
            
            // Mock test data
            var tests = new Dictionary<int, Test>
            {
                { 1, new Test { Id = 1, TestName = "OOP Fundamentals Quiz", SubjectName = "Object-Oriented Programming", QuestionCount = 10 } },
                { 2, new Test { Id = 2, TestName = "Data Structures Assessment", SubjectName = "Data Structures", QuestionCount = 8 } },
                { 3, new Test { Id = 3, TestName = "Algorithm Design Test", SubjectName = "Algorithms", QuestionCount = 12 } },
                { 4, new Test { Id = 4, TestName = "Database Concepts Quiz", SubjectName = "Database Systems", QuestionCount = 10 } },
                { 5, new Test { Id = 5, TestName = "Programming Basics Exam", SubjectName = "Programming Fundamentals", QuestionCount = 10 } },
                { 6, new Test { Id = 6, TestName = "Software Engineering Test", SubjectName = "Software Engineering", QuestionCount = 8 } }
            };
            
            return tests.TryGetValue(testId, out var test) ? test : null;
        }
        
        public async Task<List<(string SubjectName, List<(string StudentName, int Points)>)>> GetLeaderboardDataAsync()
        {
            await Task.CompletedTask;
            
            // Mock leaderboard data - top 5 per subject
            var leaderboard = new List<(string SubjectName, List<(string StudentName, int Points)>)>
            {
                ("Object-Oriented Programming", new List<(string, int)>
                {
                    ("John Smith", 1250),
                    ("Sarah Johnson", 1180),
                    ("Mike Davis", 1120),
                    ("Emma Wilson", 1080),
                    ("David Brown", 1050)
                }),
                ("Data Structures", new List<(string, int)>
                {
                    ("Sarah Johnson", 980),
                    ("John Smith", 920),
                    ("Emma Wilson", 890),
                    ("Mike Davis", 850),
                    ("David Brown", 820)
                }),
                ("Algorithms", new List<(string, int)>
                {
                    ("Mike Davis", 1100),
                    ("John Smith", 1080),
                    ("Sarah Johnson", 1020),
                    ("David Brown", 980),
                    ("Emma Wilson", 950)
                })
            };
            
            return leaderboard;
        }

        public async Task<List<Question>> GetRandomQuestionsForTestAsync(int testId, int count)
        {
            // Generate random OOP questions for development
            await Task.CompletedTask;
            
            var questions = new List<Question>
            {
                new Question
                {
                    Id = 1,
                    TestId = testId,
                    QuestionText = "What is the main principle of Object-Oriented Programming that allows a class to hide its internal implementation details?",
                    Options = new List<string> { "Inheritance", "Encapsulation", "Polymorphism", "Abstraction" },
                    CorrectAnswerIndex = 1,
                    Points = 30
                },
                new Question
                {
                    Id = 2,
                    TestId = testId,
                    QuestionText = "Which OOP principle allows a child class to inherit properties and methods from a parent class?",
                    Options = new List<string> { "Polymorphism", "Inheritance", "Encapsulation", "Abstraction" },
                    CorrectAnswerIndex = 1,
                    Points = 30
                },
                new Question
                {
                    Id = 3,
                    TestId = testId,
                    QuestionText = "What is polymorphism in Object-Oriented Programming?",
                    Options = new List<string> 
                    { 
                        "The ability to create multiple objects from a class",
                        "The ability of different classes to be treated as instances of the same class",
                        "The process of hiding implementation details",
                        "The mechanism of reusing code"
                    },
                    CorrectAnswerIndex = 1,
                    Points = 30
                },
                new Question
                {
                    Id = 4,
                    TestId = testId,
                    QuestionText = "What is an abstract class?",
                    Options = new List<string> 
                    { 
                        "A class that cannot be instantiated and may contain abstract methods",
                        "A class with only private methods",
                        "A class that inherits from multiple parent classes",
                        "A class that cannot be inherited"
                    },
                    CorrectAnswerIndex = 0,
                    Points = 30
                },
                new Question
                {
                    Id = 5,
                    TestId = testId,
                    QuestionText = "What is the difference between method overriding and method overloading?",
                    Options = new List<string> 
                    { 
                        "Overriding changes the method signature, overloading doesn't",
                        "Overriding provides a new implementation in a derived class, overloading creates multiple methods with the same name but different parameters",
                        "There is no difference",
                        "Overriding is for static methods, overloading is for instance methods"
                    },
                    CorrectAnswerIndex = 1,
                    Points = 30
                },
                new Question
                {
                    Id = 6,
                    TestId = testId,
                    QuestionText = "What is a constructor in OOP?",
                    Options = new List<string> 
                    { 
                        "A method that destroys an object",
                        "A special method that initializes an object when it is created",
                        "A method that returns a value",
                        "A method that cannot be called directly"
                    },
                    CorrectAnswerIndex = 1,
                    Points = 30
                },
                new Question
                {
                    Id = 7,
                    TestId = testId,
                    QuestionText = "What is the purpose of an interface in OOP?",
                    Options = new List<string> 
                    { 
                        "To provide a complete implementation of methods",
                        "To define a contract that classes must implement",
                        "To create multiple inheritance",
                        "To hide all methods from other classes"
                    },
                    CorrectAnswerIndex = 1,
                    Points = 30
                },
                new Question
                {
                    Id = 8,
                    TestId = testId,
                    QuestionText = "What is the 'this' keyword used for in a class?",
                    Options = new List<string> 
                    { 
                        "To refer to the parent class",
                        "To refer to the current instance of the class",
                        "To create a new object",
                        "To delete the current object"
                    },
                    CorrectAnswerIndex = 1,
                    Points = 30
                },
                new Question
                {
                    Id = 9,
                    TestId = testId,
                    QuestionText = "What is composition in OOP?",
                    Options = new List<string> 
                    { 
                        "A relationship where a class contains an instance of another class",
                        "A type of inheritance",
                        "A method of code organization",
                        "A way to make classes abstract"
                    },
                    CorrectAnswerIndex = 0,
                    Points = 30
                },
                new Question
                {
                    Id = 10,
                    TestId = testId,
                    QuestionText = "What is the difference between public, private, and protected access modifiers?",
                    Options = new List<string> 
                    { 
                        "Public: accessible everywhere, Private: only within the class, Protected: within the class and derived classes",
                        "They are all the same",
                        "Public: only within the class, Private: accessible everywhere, Protected: only in derived classes",
                        "They control the speed of method execution"
                    },
                    CorrectAnswerIndex = 0,
                    Points = 30
                }
            };

            // Return random questions up to the requested count
            var random = new Random();
            return questions.OrderBy(x => random.Next()).Take(count).ToList();
        }

        public async Task<bool> SaveTestAttemptAsync(TestAttempt attempt)
        {
            await Task.CompletedTask;
            
            // Ensure StudentId is valid
            if (attempt.StudentId <= 0)
            {
                return false; // Cannot save attempt without valid student ID
            }
            
            lock (_lockObject)
            {
                // Check if this test was already completed by this student
                var existingAttempt = _testAttempts.FirstOrDefault(
                    ta => ta.StudentId == attempt.StudentId && ta.TestId == attempt.TestId);
                
                if (existingAttempt != null)
                {
                    // Update existing attempt (allow retaking tests)
                    existingAttempt.CompletedAt = attempt.CompletedAt;
                    existingAttempt.CorrectAnswers = attempt.CorrectAnswers;
                    existingAttempt.TotalQuestions = attempt.TotalQuestions;
                    existingAttempt.PointsEarned = attempt.PointsEarned;
                    existingAttempt.Percentage = attempt.Percentage;
                }
                else
                {
                    // Add new attempt
                    attempt.Id = _nextAttemptId++;
                    _testAttempts.Add(attempt);
                }
            }
            
            return true;
        }
    }
}

