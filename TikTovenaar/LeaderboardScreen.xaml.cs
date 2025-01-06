using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using TikTovenaar.DataAccess;
using TikTovenaar.Logic;

namespace TikTovenaar
{
    public partial class LeaderboardScreen : UserControl
    {
        public string PersonalHighScore { get; set; }
        private readonly DataHandler _data;
        private int _scoreValue;
        public string PlayerName { get; private set; }
        private List<PartialScore<int>> _scores;
        private List<PartialScore<double>> _wpm;
        private List<PartialScore<int>> _levels;

        public LeaderboardScreen()
        {
            _data = new();
            LoadingScreen loadingScreen = new LoadingScreen();
            loadingScreen.Show();

            InitializeComponent();

            _scoreValue = _data.GetScores(CurrentUser.Instance.Token!).OrderByDescending(x => x.Value).Select(x => x.Value).DefaultIfEmpty(0).First(); //get the highest score of an individual; if there are no scores the value will become 0
            PersonalHighScore = $"Uw hoogste score is: {_scoreValue}";
            PersonalHighScoreLabel.Content = PersonalHighScore;
            HighscoreTable.ItemsSource = (System.Collections.IEnumerable)SortLeaderboard(_data.GetLeaderboards(), CurrentUser.Instance.Token!); //sort and bind it to the highscore datagrid
            // Fetch leaderboards data
            var leaderboards = _data.GetLeaderboards();
            _scores = leaderboards.Scores;
            _wpm = leaderboards.WPM;
            _levels = leaderboards.Levels;

            // Set personal high score
            var currentUserScore = _scores.FirstOrDefault(x => x.Player == CurrentUser.Instance.Name)?.Value ?? 0;
            PersonalHighScore = $"Uw hoogste score is: {currentUserScore}";
            PersonalHighScoreLabel.Content = PersonalHighScore;

            // Set default filter programmatically
            FilterOptions.SelectedIndex = 0; // Default to "Score" filter
            UpdateLeaderboardTable("Score");

            loadingScreen.Close();
        }

        private void FilterOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterOptions.SelectedItem is ComboBoxItem selectedOption)
            {
                string filter = selectedOption.Tag.ToString();
                UpdateLeaderboardTable(filter);
            }
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchToHomeScreen();
        }
        private object SortLeaderboard(Leaderboards scores, string? token)
        {
            return scores.Scores
                .OrderByDescending(score => score.Value) // Sort by score descending
                .Select((score, index) => new LeaderboardEntry //class with all the entries needed for the leaderboard screen
                {
                    Ranking = index + 1,
                    Name = score.Player,
                    Score = score.Value,
                    Colorcode = (Brush)new BrushConverter().ConvertFromString(score.Player.Equals(CurrentUser.Instance.Name) ? "#2732c2" : "#000435") //decides the hex code needed for the display; should highlight the name of the personal highscore
                })
                .ToList();
        }

        private void UpdateLeaderboardTable(string filter)
        {
            List<LeaderboardEntry> sortedData = new();
            string columnHeader;

            switch (filter)
            {
                case "WPM":
                    if (_wpm == null || !_wpm.Any())
                    {
                        // Handle empty WPM data gracefully
                        MessageBox.Show("No WPM data available.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    columnHeader = "WPM";
                    sortedData = _wpm
                        .OrderByDescending(x => x.Value)
                        .Select((x, index) => new LeaderboardEntry
                        {
                            Ranking = index + 1,
                            Name = x.Player,
                            Score = 0,
                            WPM = x.Value,
                            Level = 0
                        })
                        .ToList();
                    break;

                case "Score":
                    if (_scores == null || !_scores.Any())
                    {
                        MessageBox.Show("No Score data available.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    columnHeader = "Score";
                    sortedData = _scores
                        .OrderByDescending(x => x.Value)
                        .Select((x, index) => new LeaderboardEntry
                        {
                            Ranking = index + 1,
                            Name = x.Player,
                            Score = x.Value,
                            WPM = 0,
                            Level = 0
                        })
                        .ToList();
                    break;

                case "Level":
                    if (_levels == null || !_levels.Any())
                    {
                        MessageBox.Show("No Level data available.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    columnHeader = "Level";
                    sortedData = _levels
                        .OrderByDescending(x => x.Value)
                        .Select((x, index) => new LeaderboardEntry
                        {
                            Ranking = index + 1,
                            Name = x.Player,
                            Score = 0,
                            WPM = 0,
                            Level = x.Value
                        })
                        .ToList();
                    break;

                default:
                    throw new InvalidOperationException("Invalid filter selected");
            }

            // Update the column header for the "Value" column
            var scoreColumn = HighscoreTable.Columns[2]; // Assuming "Value" column is at index 2
            scoreColumn.Header = columnHeader;

            // Update the leaderboard table with the sorted data
            HighscoreTable.ItemsSource = sortedData;
        }

    }
}
