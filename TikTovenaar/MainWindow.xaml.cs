﻿using System.Windows;
using TikTovenaar.Logic;
using TikTovenaar.DataAccess;

namespace TikTovenaar
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            
            //zorgt dat muziek wordt afgespeeld
            SoundManager.InitMusic();
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

        public void SwitchToAdminScreen()
        {
            MainContentControl.Content = new AdminScreen();
        }
        public void SwitchToLeaderboardscreen()
        {
            MainContentControl.Content = new LeaderboardScreen();
        }
        
        public void SwitchToUserStatisticsScreen()
        {
            MainContentControl.Content = new UserStatisticsScreen(new DataHandler());
        }

        public void SwitchToDeleteUsersScreen()
        {
            MainContentControl.Content = new DeleteUsers();
        }

        public void SwitchToSettingsScreen()
        {
            MainContentControl.Content = new SettingsScreen();
        }
    }
}
