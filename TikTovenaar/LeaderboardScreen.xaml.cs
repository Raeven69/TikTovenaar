using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TikTovenaar.DataAccess;
using TikTovenaar.Logic;
using static System.Formats.Asn1.AsnWriter;

namespace TikTovenaar
{
    public partial class LeaderboardScreen : UserControl
    {
        public string PersonalHighScore { get; set; }
        private readonly DataHandler _data;
        private List<PartialScore<int>> _scores;
        private List<PartialScore<double>> _wpm;
        private List<PartialScore<int>> _levels;

        public LeaderboardScreen()
        {
            _data = new();
            LoadingScreen loadingScreen = new LoadingScreen();
            loadingScreen.Show();

            InitializeComponent();

            try
            {
                // Fetch leaderboards data
                Leaderboards leaderboards = _data.GetLeaderboards();

                _scores = leaderboards.Scores ?? new List<PartialScore<int>>();
                _wpm = leaderboards.WPM ?? new List<PartialScore<double>>();
                _levels = leaderboards.Levels ?? new List<PartialScore<int>>();

                foreach (var score in _scores)
                {
                    Debug.WriteLine($"Score: {score.Player} - {score.Value}");
                }
                foreach (var wpm in _wpm)
                {
                    Debug.WriteLine($"WPM: {wpm.Player} - {wpm.Value}");
                }
                foreach (var level in _levels)
                {
                    Debug.WriteLine($"Level: {level.Player} - {level.Value}");
                }

                // Set personal high score
                var currentUserScore = _scores.FirstOrDefault(x => x.Player == CurrentUser.Instance.Name)?.Value ?? 0;
                PersonalHighScore = $"Uw hoogste score is: {currentUserScore}";
                PersonalHighScoreLabel.Content = PersonalHighScore;

                // Set default filter programmatically
                FilterOptions.SelectedIndex = 0; // Default to "Score" filter
                UpdateLeaderboardTable("Score");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fout bij het laden van de leaderboards: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                loadingScreen.Close();
            }
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

        private void UpdateLeaderboardTable(string filter)
        {
            List<LeaderboardEntry> sortedData = new();
            string columnHeader;

            switch (filter)
            {
                case "WPM":
                    if (_wpm == null || !_wpm.Any())
                    {
                        Debug.WriteLine("No WPM scores found");
                        return;
                    }
                    columnHeader = "WPM";
                    sortedData = _wpm
                        .OrderByDescending(x => x.Value)
                        .Select((x, index) => new LeaderboardEntry
                        {
                            Ranking = index + 1,
                            Name = x.Player,
                            Value = x.Value,  // Set the dynamic Value to WPM
                            Colorcode = (Brush)new BrushConverter().ConvertFromString(x.Player.Equals(CurrentUser.Instance.Name) ? "#2732c2" : "#000435") //decides the hex code needed for the display; should highlight the name of the personal highscore
                        })
                        .ToList();
                    break;

                case "Score":
                    if (_scores == null || !_scores.Any())
                    {
                        Debug.WriteLine("No scores found");
                        return;
                    }
                    columnHeader = "Score";
                    sortedData = _scores
                        .OrderByDescending(x => x.Value)
                        .Select((x, index) => new LeaderboardEntry
                        {
                            Ranking = index + 1,
                            Name = x.Player,
                            Value = x.Value,  // Set the dynamic Value to Score
                            Colorcode = (Brush)new BrushConverter().ConvertFromString(x.Player.Equals(CurrentUser.Instance.Name) ? "#2732c2" : "#000435") //decides the hex code needed for the display; should highlight the name of the personal highscore
                        })
                        .ToList();
                    break;

                case "Level":
                    if (_levels == null || !_levels.Any())
                    {
                        Debug.WriteLine("No levels found");
                        return;
                    }
                    columnHeader = "Level";
                    sortedData = _levels
                        .OrderByDescending(x => x.Value)
                        .Select((x, index) => new LeaderboardEntry
                        {
                            Ranking = index + 1,
                            Name = x.Player,
                            Value = x.Value,
                            Colorcode = (Brush)new BrushConverter().ConvertFromString(x.Player.Equals(CurrentUser.Instance.Name) ? "#2732c2" : "#000435") //decides the hex code needed for the display; should highlight the name of the personal highscore                                                                                                                              // Set the dynamic Value to Level
                        })
                        .ToList();
                    break;
                default:
                    throw new InvalidOperationException("Ongeldig filter geselecteerd");
            }

            // Update the column header for the "Value" column
            var scoreColumn = HighscoreTable.Columns[2]; // Assuming "Value" column is at index 2
            scoreColumn.Header = columnHeader;

            // Update the leaderboard table with the sorted data
            HighscoreTable.ItemsSource = sortedData;
        }
    }
}