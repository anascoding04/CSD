using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFCSDProject.Config;
using WPFCSDProject.Models;
using WPFCSDProject.Services;
using WPFCSDProject.Views;

namespace WPFCSDProject.Dashboard
{
    public partial class DashboardWindow : Window
    {
        private readonly IAuthenticationService _authService;
        private readonly IDatabaseService _databaseService;

        public DashboardWindow(IAuthenticationService authService)
        {
            InitializeComponent();
            _authService = authService;
            _databaseService = new DatabaseService(DatabaseConfig.GetConnectionString());
            LoadDashboardData();
        }

        private async void LoadDashboardData()
        {
            if (_authService.CurrentStudent == null)
                return;

            var student = _authService.CurrentStudent;
            WelcomeTextBlock.Text = $"Welcome, {student.FirstName} {student.LastName}!";
            
            // Load completed tests to calculate metrics
            var completedTests = await _databaseService.GetCompletedTestsAsync(student.Id);
            
            // Calculate and display metrics
            int totalPoints = completedTests.Sum(t => t.PointsEarned);
            TotalPointsTextBlock.Text = totalPoints.ToString();
            
            int testsCompleted = completedTests.Count;
            TestsCompletedTextBlock.Text = testsCompleted.ToString();
            
            double averagePercentage = completedTests.Count > 0 
                ? completedTests.Average(t => t.Percentage) 
                : 0;
            AveragePercentageTextBlock.Text = $"{averagePercentage:F1}%";
            
            // Load and display test lists
            await LoadTestLists(student.Id);
            
            // Load and display leaderboard
            await LoadLeaderboard();
            
            // Create grade graph
            CreateGradeGraph(completedTests);
        }

        private async Task LoadTestLists(int studentId)
        {
            // Load available tests (to-do)
            var availableTests = await _databaseService.GetAvailableTestsAsync(studentId);
            
            // Load completed tests to filter them out
            var completedTests = await _databaseService.GetCompletedTestsAsync(studentId);
            var completedTestIds = completedTests.Select(ct => ct.TestId).ToList();
            
            // Filter out completed tests from available tests
            var todoTests = availableTests.Where(test => !completedTestIds.Contains(test.Id)).ToList();
            
            ToDoTestsListBox.Items.Clear();
            foreach (var test in todoTests)
            {
                var displayText = $"{test.TestName} - {test.SubjectName} ({test.QuestionCount} questions)";
                ToDoTestsListBox.Items.Add(new { Test = test, DisplayText = displayText });
            }
            
            // Display completed tests
            CompletedTestsListBox.Items.Clear();
            foreach (var attempt in completedTests)
            {
                var test = await _databaseService.GetTestByIdAsync(attempt.TestId);
                if (test != null)
                {
                    var displayText = $"{test.TestName} - {attempt.Percentage:F1}% ({attempt.CompletedAt:MM/dd/yyyy})";
                    CompletedTestsListBox.Items.Add(new { TestAttempt = attempt, Test = test, DisplayText = displayText });
                }
            }
        }

        private async Task LoadLeaderboard()
        {
            var leaderboardData = await _databaseService.GetLeaderboardDataAsync();
            
            // Clear existing leaderboard UI
            LeaderboardStackPanel.Children.Clear();
            
            foreach (var (subjectName, topStudents) in leaderboardData)
            {
                // Create subject header
                var subjectBorder = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 0, 0, 10)
                };
                
                var subjectText = new TextBlock
                {
                    Text = subjectName,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White
                };
                subjectBorder.Child = subjectText;
                LeaderboardStackPanel.Children.Add(subjectBorder);
                
                // Create student list
                for (int i = 0; i < topStudents.Count; i++)
                {
                    var (studentName, points) = topStudents[i];
                    
                    var studentBorder = new Border
                    {
                        Background = new SolidColorBrush(Color.FromRgb(236, 240, 241)),
                        BorderBrush = new SolidColorBrush(Color.FromRgb(189, 195, 199)),
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(3),
                        Padding = new Thickness(10, 8, 10, 8),
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    
                    var studentGrid = new Grid();
                    studentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                    studentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    studentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                    
                    // Rank icon
                    var rankText = new TextBlock
                    {
                        Text = i == 0 ? "🥇" : i == 1 ? "🥈" : i == 2 ? "🥉" : $"{i + 1}.",
                        FontSize = 18,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 0, 10, 0)
                    };
                    Grid.SetColumn(rankText, 0);
                    studentGrid.Children.Add(rankText);
                    
                    // Student name
                    var nameText = new TextBlock
                    {
                        Text = studentName,
                        FontSize = 14,
                        FontWeight = FontWeights.SemiBold,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    Grid.SetColumn(nameText, 1);
                    studentGrid.Children.Add(nameText);
                    
                    // Points with progress bar
                    var pointsContainer = new StackPanel { Orientation = Orientation.Horizontal };
                    var pointsText = new TextBlock
                    {
                        Text = $"{points} pts",
                        FontSize = 12,
                        Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141)),
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(10, 0, 5, 0)
                    };
                    pointsContainer.Children.Add(pointsText);
                    
                    // Simple progress indicator (bar width based on points relative to max)
                    int maxPoints = topStudents[0].Points;
                    double progressWidth = Math.Min((double)points / maxPoints * 100, 100);
                    var progressBar = new Border
                    {
                        Background = new SolidColorBrush(Color.FromRgb(46, 204, 113)),
                        Height = 8,
                        Width = progressWidth,
                        CornerRadius = new CornerRadius(2),
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    pointsContainer.Children.Add(progressBar);
                    
                    Grid.SetColumn(pointsContainer, 2);
                    studentGrid.Children.Add(pointsContainer);
                    
                    studentBorder.Child = studentGrid;
                    LeaderboardStackPanel.Children.Add(studentBorder);
                }
            }
        }

        private void CreateGradeGraph(List<TestAttempt> completedTests)
        {
            GradeGraphCanvas.Children.Clear();
            
            if (completedTests.Count == 0)
            {
                var noDataText = new TextBlock
                {
                    Text = "No test data available yet. Complete some tests to see your progress!",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141))
                };
                Canvas.SetLeft(noDataText, 200);
                Canvas.SetTop(noDataText, 90);
                GradeGraphCanvas.Children.Add(noDataText);
                return;
            }
            
            // Sort by completion date
            var sortedTests = completedTests.OrderBy(t => t.CompletedAt).ToList();
            
            // Use fixed dimensions that match the XAML
            double canvasWidth = 700;
            double canvasHeight = 180;
            double maxPercentage = 100;
            double availableWidth = canvasWidth - 40; // Leave margins
            double spacing = 10;
            int testCount = sortedTests.Count;
            double barWidth = testCount > 0 ? (availableWidth - (spacing * (testCount - 1))) / testCount : 50;
            if (barWidth < 20) barWidth = 20; // Minimum bar width
            double maxBarHeight = canvasHeight - 60;
            
            // Draw axes
            var xAxis = new Line
            {
                X1 = 20,
                Y1 = canvasHeight - 30,
                X2 = canvasWidth - 20,
                Y2 = canvasHeight - 30,
                Stroke = new SolidColorBrush(Color.FromRgb(44, 62, 80)),
                StrokeThickness = 2
            };
            GradeGraphCanvas.Children.Add(xAxis);
            
            var yAxis = new Line
            {
                X1 = 20,
                Y1 = 20,
                X2 = 20,
                Y2 = canvasHeight - 30,
                Stroke = new SolidColorBrush(Color.FromRgb(44, 62, 80)),
                StrokeThickness = 2
            };
            GradeGraphCanvas.Children.Add(yAxis);
            
            // Draw percentage markers
            for (int i = 0; i <= 4; i++)
            {
                double percentage = i * 25;
                double y = canvasHeight - 30 - (percentage / maxPercentage * maxBarHeight);
                
                var marker = new Line
                {
                    X1 = 15,
                    Y1 = y,
                    X2 = 20,
                    Y2 = y,
                    Stroke = new SolidColorBrush(Color.FromRgb(127, 140, 141)),
                    StrokeThickness = 1
                };
                GradeGraphCanvas.Children.Add(marker);
                
                var label = new TextBlock
                {
                    Text = $"{percentage}%",
                    FontSize = 10,
                    Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141)),
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                Canvas.SetLeft(label, 0);
                Canvas.SetTop(label, y - 8);
                GradeGraphCanvas.Children.Add(label);
            }
            
            // Draw bars for each test
            double startX = 30;
            for (int i = 0; i < sortedTests.Count; i++)
            {
                var attempt = sortedTests[i];
                double barHeight = (attempt.Percentage / maxPercentage) * maxBarHeight;
                double x = startX + i * (barWidth + spacing);
                
                // Draw bar
                var bar = new Rectangle
                {
                    Width = barWidth,
                    Height = barHeight,
                    Fill = new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                    RadiusX = 2,
                    RadiusY = 2
                };
                Canvas.SetLeft(bar, x);
                Canvas.SetTop(bar, canvasHeight - 30 - barHeight);
                GradeGraphCanvas.Children.Add(bar);
                
                // Draw percentage label on top of bar
                var percentageLabel = new TextBlock
                {
                    Text = $"{attempt.Percentage:F0}%",
                    FontSize = 9,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Canvas.SetLeft(percentageLabel, x + barWidth / 2 - 15);
                Canvas.SetTop(percentageLabel, canvasHeight - 30 - barHeight - 18);
                GradeGraphCanvas.Children.Add(percentageLabel);
                
                // Draw test number label
                var testLabel = new TextBlock
                {
                    Text = $"Test {i + 1}",
                    FontSize = 9,
                    Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141)),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Canvas.SetLeft(testLabel, x + barWidth / 2 - 15);
                Canvas.SetTop(testLabel, canvasHeight - 20);
                GradeGraphCanvas.Children.Add(testLabel);
            }
        }

        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to sign out?",
                "Sign Out",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _authService.Logout();
                
                var landingPage = new LandingPage(_authService);
                landingPage.Show();
                
                MessageBox.Show("You have been successfully signed out.", "Signed Out", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Optional: Confirm before closing if needed
        }

        private void ToDoTest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ToDoTestsListBox.SelectedItem == null)
                return;

            dynamic selectedItem = ToDoTestsListBox.SelectedItem;
            Test selectedTest = selectedItem.Test;
            
            // Open test taking window
            var testTakingWindow = new TestTakingWindow(_authService, _databaseService, selectedTest);
            testTakingWindow.ShowDialog();
            
            // Reload dashboard data after test is completed
            LoadDashboardData();
            
            ToDoTestsListBox.SelectedItem = null;
        }

        private void CompletedTest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CompletedTestsListBox.SelectedItem == null)
                return;

            dynamic selectedItem = CompletedTestsListBox.SelectedItem;
            TestAttempt attempt = selectedItem.TestAttempt;
            Test test = selectedItem.Test;
            
            // Open test history window
            var testHistoryWindow = new TestHistoryWindow(test, attempt);
            testHistoryWindow.ShowDialog();
            
            CompletedTestsListBox.SelectedItem = null;
        }

        private void ViewLeaderboardButton_Click(object sender, RoutedEventArgs e)
        {
            // Open full leaderboard window
            var leaderboardWindow = new LeaderboardWindow(_databaseService);
            leaderboardWindow.Owner = this;
            leaderboardWindow.ShowDialog();
        }
    }
}
