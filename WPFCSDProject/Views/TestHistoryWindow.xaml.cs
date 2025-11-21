using System.Windows;
using WPFCSDProject.Models;

namespace WPFCSDProject.Views
{
    public partial class TestHistoryWindow : Window
    {
        private readonly Test _test;
        private readonly TestAttempt _attempt;

        public TestHistoryWindow(Test test, TestAttempt attempt)
        {
            InitializeComponent();
            _test = test;
            _attempt = attempt;
            LoadTestHistory();
        }

        private void LoadTestHistory()
        {
            TestNameTextBlock.Text = _test.TestName;
            CompletionDateTextBlock.Text = _attempt.CompletedAt.ToString("MMMM dd, yyyy 'at' HH:mm");
            QuestionsCountTextBlock.Text = _attempt.TotalQuestions.ToString();
            PercentageTextBlock.Text = $"{_attempt.Percentage:F1}%";
            PointsTextBlock.Text = $"{_attempt.PointsEarned} points";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

