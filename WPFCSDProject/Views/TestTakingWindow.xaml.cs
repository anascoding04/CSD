using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFCSDProject.Models;
using WPFCSDProject.Services;

namespace WPFCSDProject.Views
{
    public partial class TestTakingWindow : Window
    {
        private readonly IAuthenticationService _authService;
        private readonly IDatabaseService _databaseService;
        private readonly Test _test;
        private List<Question> _questions = new List<Question>();
        private List<int> _selectedAnswers = new List<int>();
        private int _currentQuestionIndex = 0;
        private bool _isTestCompleted = false;

        public TestTakingWindow(IAuthenticationService authService, IDatabaseService databaseService, Test test)
        {
            InitializeComponent();
            _authService = authService;
            _databaseService = databaseService;
            _test = test;
            LoadTest();
        }

        private async void LoadTest()
        {
            try
            {
                TestNameTextBlock.Text = _test.TestName;
                SubjectNameTextBlock.Text = _test.SubjectName;

                // Get random questions for this test
                _questions = await _databaseService.GetRandomQuestionsForTestAsync(_test.Id, _test.QuestionCount);
                _selectedAnswers = new List<int>(new int[_questions.Count]);

                if (_questions.Count == 0)
                {
                    MessageBox.Show("No questions available for this test.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                DisplayQuestion(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading test: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void DisplayQuestion(int index)
        {
            if (index < 0 || index >= _questions.Count)
                return;

            _currentQuestionIndex = index;
            var question = _questions[index];

            QuestionTextBlock.Text = question.QuestionText;
            QuestionCounterTextBlock.Text = $"Question {index + 1} of {_questions.Count}";

            // Clear previous options
            OptionsStackPanel.Children.Clear();

            // Add radio buttons for each option
            for (int i = 0; i < question.Options.Count; i++)
            {
                var radioButton = new RadioButton
                {
                    Content = question.Options[i],
                    FontSize = 16,
                    Margin = new Thickness(0, 5, 0, 5),
                    Tag = i,
                    IsChecked = _selectedAnswers[index] == i
                };
                radioButton.Checked += (s, e) => { _selectedAnswers[index] = (int)radioButton.Tag; };
                OptionsStackPanel.Children.Add(radioButton);
            }

            // Update navigation buttons
            PreviousButton.IsEnabled = index > 0;
            NextButton.IsEnabled = index < _questions.Count - 1;
            
            if (index == _questions.Count - 1)
            {
                NextButton.Visibility = Visibility.Collapsed;
                SubmitButton.Visibility = Visibility.Visible;
            }
            else
            {
                NextButton.Visibility = Visibility.Visible;
                SubmitButton.Visibility = Visibility.Collapsed;
            }

            // Update progress bar
            UpdateProgressBar();
        }

        private void UpdateProgressBar()
        {
            double progress = ((double)(_currentQuestionIndex + 1) / _questions.Count) * 100;
            if (ProgressBarContainer.ActualWidth > 0)
            {
                ProgressBar.Width = (progress / 100) * ProgressBarContainer.ActualWidth;
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentQuestionIndex > 0)
            {
                DisplayQuestion(_currentQuestionIndex - 1);
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentQuestionIndex < _questions.Count - 1)
            {
                DisplayQuestion(_currentQuestionIndex + 1);
            }
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isTestCompleted)
                return;

            var result = MessageBox.Show(
                "Are you sure you want to submit your test? You cannot change your answers after submission.",
                "Confirm Submission",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                _isTestCompleted = true;
                SubmitButton.IsEnabled = false;

                // Calculate score
                int correctAnswers = 0;
                int totalPoints = 0;

                for (int i = 0; i < _questions.Count; i++)
                {
                    if (_selectedAnswers[i] == _questions[i].CorrectAnswerIndex)
                    {
                        correctAnswers++;
                        totalPoints += 30; // +30 for correct answer
                    }
                    else if (_selectedAnswers[i] != -1) // -10 for incorrect answer (only if answered)
                    {
                        totalPoints -= 10;
                    }
                }

                // Ensure points don't go negative
                if (totalPoints < 0)
                    totalPoints = 0;

                double percentage = (double)correctAnswers / _questions.Count * 100;

                // Save test attempt
                if (_authService.CurrentStudent?.Id == 0 || _authService.CurrentStudent == null)
                {
                    MessageBox.Show("Warning: Student ID is not set. Test attempt may not be saved correctly.", 
                        "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                var attempt = new TestAttempt
                {
                    StudentId = _authService.CurrentStudent?.Id ?? 0,
                    TestId = _test.Id,
                    CompletedAt = DateTime.UtcNow,
                    CorrectAnswers = correctAnswers,
                    TotalQuestions = _questions.Count,
                    PointsEarned = totalPoints,
                    Percentage = percentage
                };

                await _databaseService.SaveTestAttemptAsync(attempt);

                // Show results
                string message = $"Test submitted successfully!\n\n" +
                               $"Correct Answers: {correctAnswers} / {_questions.Count}\n" +
                               $"Percentage: {percentage:F1}%\n" +
                               $"Points Earned: {totalPoints}";

                MessageBox.Show(message, "Test Results", MessageBoxButton.OK, MessageBoxImage.Information);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error submitting test: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                _isTestCompleted = false;
                SubmitButton.IsEnabled = true;
            }
        }
    }
}

