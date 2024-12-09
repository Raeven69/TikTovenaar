using System.Windows;
using TikTovenaar.Logic;

namespace TikTovenaar
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // set the background music
            SoundManager.PlayBackgroundSound("Sounds/wizard_theme_music.mp3");
            // SoundManager.PlaySoundEffect("Sounds/coc.mp3");

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

        public void SwitchToGameStatisticsScreen(string totaltime, string wordsperminuut, int score, string errorpercentage, string wordcount, List<Word> wordList, List<Word> wrongWordList, List<Letter> wrongLetterList)
        {
            MainContentControl.Content = new GameStatisticsScreen(totaltime, wordsperminuut, score, errorpercentage, wordcount, wordList, wrongWordList, wrongLetterList );
        }

        public void SwitchToWordListScreen(GameStatisticsScreen statisticsScreen, List<Word> wordList, List<Word> wrongWordList, List<Letter> wrongLetterList)
        {
            MainContentControl.Content = new WordListScreen(statisticsScreen, wordList, wrongWordList, wrongLetterList);
        }

        public void SwitchBackToStatisticsScreen(GameStatisticsScreen statisticsScreen)
        {
            MainContentControl.Content = statisticsScreen;
        }
    }
}
