using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TikTovenaar.DataAccess;
using TikTovenaar.Logic;

namespace TikTovenaar
{
    /// <summary>
    /// Interaction logic for LeaderboardScreen.xaml
    /// </summary>
    public partial class LeaderboardScreen : UserControl
    {
        public string PersonalHighScore { get; set; }
        public Dictionary<string, int> scores;
        private readonly DataHandler _data;
        private int _scoreValue;
        public string PlayerName { get; private set; }
        

        
        public LeaderboardScreen()
        {
            _data = new();

            InitializeComponent();
             _scoreValue = _data.GetScores(CurrentUser.Instance.Token!).OrderByDescending(x => x.Value).Select(x => x.Value).DefaultIfEmpty(0).First(); //get the highest score of an individual; if there are no scores the value will become 0
            PersonalHighScore = $"Uw hoogste score is: {_scoreValue}";
            PersonalHighScoreLabel.Content = PersonalHighScore;
            HighscoreTable.ItemsSource = (System.Collections.IEnumerable)SortLeaderboard(_data.GetHighscores(), CurrentUser.Instance.Token!); //sort and bind it to the highscore datagrid
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchToHomeScreen();
        }
        private object SortLeaderboard(List<PartialScore> scores, string? token)
        {
            return scores
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
    }
}
