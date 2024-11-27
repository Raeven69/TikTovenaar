using System.Windows;

namespace TikTovenaar
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // set the background music
            SoundManager.PlayBackgroundSound("Sounds/wizard_theme_music.mp3");

            // set the first screen
            MainContentControl.Content = new Loginscreen();
        }

        public void SwitchToHomeScreen()
        {
            MainContentControl.Content = new Homescreen();  
        }

        public void SwitchToLoginScreen()
        {
            MainContentControl.Content = new Loginscreen();
        }

        public void SwitchToGameScreen()
        {
            MainContentControl.Content = new Gamescreen();
        }

        public void SwitchToGameStatisticsScreen(string totaltime, string wordsperminuut, int score, string errorpercentage, string wordcount)
        {
            MainContentControl.Content = new GameStatisticsScreen(totaltime, wordsperminuut, score, errorpercentage, wordcount);
        }

        public void SwitchToLeaderboardscreen()
        {
            MainContentControl.Content = new LeaderboardScreen();
        }
    }
}
