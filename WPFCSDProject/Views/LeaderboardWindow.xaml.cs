using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFCSDProject.Services;

namespace WPFCSDProject.Views
{
    public partial class LeaderboardWindow : Window
    {
        private readonly IDatabaseService _databaseService;

        public LeaderboardWindow(IDatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
            LoadLeaderboard();
        }

        private async void LoadLeaderboard()
        {
            var leaderboardData = await _databaseService.GetLeaderboardDataAsync();
            
            // Clear existing leaderboard UI
            LeaderboardStackPanel.Children.Clear();
            
            if (leaderboardData.Count == 0)
            {
                var noDataText = new TextBlock
                {
                    Text = "No leaderboard data available at this time.",
                    FontSize = 16,
                    Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 50, 0, 0)
                };
                LeaderboardStackPanel.Children.Add(noDataText);
                return;
            }
            
            foreach (var (subjectName, topStudents) in leaderboardData)
            {
                // Create subject header
                var subjectBorder = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(15),
                    Margin = new Thickness(0, 0, 0, 15)
                };
                
                var subjectText = new TextBlock
                {
                    Text = subjectName,
                    FontSize = 20,
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
                        CornerRadius = new CornerRadius(5),
                        Padding = new Thickness(15, 12, 15, 12),
                        Margin = new Thickness(0, 0, 0, 8)
                    };
                    
                    var studentGrid = new Grid();
                    studentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                    studentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    studentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                    studentGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200, GridUnitType.Pixel) });
                    
                    // Rank icon/text
                    var rankText = new TextBlock
                    {
                        Text = i == 0 ? "🥇" : i == 1 ? "🥈" : i == 2 ? "🥉" : $"{i + 1}.",
                        FontSize = 24,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(0, 0, 15, 0),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Width = 40
                    };
                    Grid.SetColumn(rankText, 0);
                    studentGrid.Children.Add(rankText);
                    
                    // Student name
                    var nameText = new TextBlock
                    {
                        Text = studentName,
                        FontSize = 16,
                        FontWeight = FontWeights.SemiBold,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = new SolidColorBrush(Color.FromRgb(44, 62, 80))
                    };
                    Grid.SetColumn(nameText, 1);
                    studentGrid.Children.Add(nameText);
                    
                    // Points text
                    var pointsText = new TextBlock
                    {
                        Text = $"{points} points",
                        FontSize = 15,
                        FontWeight = FontWeights.Bold,
                        Foreground = new SolidColorBrush(Color.FromRgb(52, 152, 219)),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Margin = new Thickness(10, 0, 10, 0)
                    };
                    Grid.SetColumn(pointsText, 2);
                    studentGrid.Children.Add(pointsText);
                    
                    // Progress bar container
                    var progressContainer = new Border
                    {
                        Background = new SolidColorBrush(Color.FromRgb(236, 240, 241)),
                        BorderBrush = new SolidColorBrush(Color.FromRgb(189, 195, 199)),
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(3),
                        Height = 20,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    
                    // Progress bar
                    int maxPoints = topStudents[0].Points;
                    double progressPercentage = Math.Min((double)points / maxPoints * 100, 100);
                    var progressBar = new Border
                    {
                        Background = new SolidColorBrush(Color.FromRgb(46, 204, 113)),
                        Height = 18,
                        Width = progressPercentage / 100 * 196,
                        CornerRadius = new CornerRadius(2),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(2, 0, 0, 0)
                    };
                    progressContainer.Child = progressBar;
                    
                    Grid.SetColumn(progressContainer, 3);
                    studentGrid.Children.Add(progressContainer);
                    
                    studentBorder.Child = studentGrid;
                    LeaderboardStackPanel.Children.Add(studentBorder);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

