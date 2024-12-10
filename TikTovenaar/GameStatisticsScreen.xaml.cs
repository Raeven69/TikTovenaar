using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using TikTovenaar.Logic;

namespace TikTovenaar
{
    public partial class GameStatisticsScreen : UserControl, INotifyPropertyChanged
    {
        private bool animationPlayed = false;

        private string _wordCount;
        private string _errorPercentage;
        private string _wordsPerMinute;
        private string _totalTime;
        private int _score = 0;

        private List<Word> _wordList;
        private List<Word> _wrongWordList;
        private List<Letter> _wrongLetterList;

        // Event for notifying when a property changes, used for data binding.
        public event PropertyChangedEventHandler? PropertyChanged;

        public string WordCount
        {
            get => _wordCount;
            set
            {
                if (_wordCount != value)
                {
                    _wordCount = value; 
                    OnPropertyChanged();
                }
            }
        }

        public string ErrorPercentage
        {
            get => _errorPercentage;
            set
            {
                if (_errorPercentage != value)
                {
                    _errorPercentage = $"{100 - float.Parse(value):0.00}%";
                    OnPropertyChanged();
                }
            }
        }

        public string WordsPerMinute
        {
            get => _wordsPerMinute;
            set
            {
                if (_wordsPerMinute != value)
                {
                    _wordsPerMinute = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TotalTime
        {
            get => _totalTime;
            set
            {
                if (_totalTime != value)
                {
                    int.TryParse(value, out int time); // Converts the string to an integer.
                    if (time > 60)
                    {
                        int minutes = time / 60; // Calculates the minutes.
                        int seconds = time % 60; // Calculates the seconds.
                        if (minutes > 1)
                        {
                            value = $"{minutes} minuten en {seconds} seconden"; // Updates the total time.
                        }
                        else
                        {
                            value = $"{minutes} minuut en {seconds} seconden"; // Updates the total time.
                        }
                        _totalTime = value; // Updates the total time.
                    }
                    else
                    {
                        _totalTime = $"{value} seconden"; // Updates total time.
                    }
                    OnPropertyChanged(); // Notifies the UI about the change.
                }
            }
        }

        public int Score
        {
            get => _score; 
            set
            {
                if (_score != value)
                {
                    _score = value; 
                    OnPropertyChanged();
                }
            }
        }

        public GameStatisticsScreen(string totaltime, string wordsperminuut, int score, string errorpercentage, string wordcount, List<Word> wordList, List<Word> wrongWordList, List<Letter> wrongLetterList)
        {
            InitializeComponent(); // Initializes the XAML components.
            DataContext = this; // Sets the current class as the data source for bindings.
            this.Score = score;
            this.TotalTime = totaltime;
            this.WordCount = wordcount;
            this.WordsPerMinute = wordsperminuut;
            this.ErrorPercentage = errorpercentage;

            this._wordList = wordList;
            this._wrongWordList = wrongWordList;
            this._wrongLetterList = wrongLetterList;

            if(!animationPlayed)
            {
                StartStoryboardAnimation(); // Starts the storyboard animation when the screen is loaded.
                animationPlayed = true;
            }
        }

        // Helper method for raising the PropertyChanged event.
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Event handler for the "Close" button.
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchToHomeScreen();
        }

        // Event handler for the "Retry" button.
        private void Retry_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchToGameScreen();
        }

        private void WordList_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchToWordListScreen(this, _wordList, _wrongWordList, _wrongLetterList);
        }

        // Method to start the storyboard animation.
        private void StartStoryboardAnimation()
        {
            var storyboard = (Storyboard)FindResource("AppearInSequence");
            if (storyboard != null)
            {
                storyboard.Begin();
            }
        }
    }
}
