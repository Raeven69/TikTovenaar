﻿using System.ComponentModel; // Enables notifying changes in properties (e.g., for data binding).
using System.Runtime.CompilerServices; // Allows automatic property name capturing for OnPropertyChanged.
using System.Windows; // Provides WPF core functionality.
using System.Windows.Controls; // Provides WPF UI controls like UserControl.
using System.Windows.Media.Animation; // Enables storyboards and animations.

namespace TikTovenaar // Namespace for organizing the code.
{
    // A UserControl named GameStatisticsScreen for displaying game statistics.
    public partial class GameStatisticsScreen : UserControl, INotifyPropertyChanged
    {
        // Private fields for storing property values.
        private string _wordCount = string.Empty;
        private string _errorPercentage = string.Empty;
        private string _wordsPerMinute = string.Empty;
        private string _totalTime = string.Empty;
        private int _score = 0;

        // Event for notifying when a property changes, used for data binding.
        public event PropertyChangedEventHandler? PropertyChanged;

        // Properties with get and set methods to allow data binding.
        public string WordCount
        {
            get => _wordCount; // Returns the current value of the word count.
            set
            {
                if (_wordCount != value)
                {
                    _wordCount = value; // Updates the word count.
                    OnPropertyChanged(); // Notifies the UI about the change.
                }
            }
        }

        public string ErrorPercentage
        {
            get => _errorPercentage; // Returns the error percentage.
            set
            {
                if (_errorPercentage != value)
                {
                    _errorPercentage = $"{value}%"; // Updates the error percentage.
                    OnPropertyChanged(); // Notifies the UI about the change.
                }
            }
        }

        public string WordsPerMinute
        {
            get => _wordsPerMinute; // Returns words per minute.
            set
            {
                if (_wordsPerMinute != value)
                {
                    _wordsPerMinute = value; // Updates words per minute.
                    OnPropertyChanged(); // Notifies the UI about the change.
                }
            }
        }

        public string TotalTime
        {
            get => _totalTime; // Returns the total time.
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
            get => _score; // Returns the score.
            set
            {
                if (_score != value)
                {
                    _score = value; // Updates the score.
                    OnPropertyChanged(); // Notifies the UI about the change.
                }
            }
        }

        // Constructor that initializes the control and sets the data context for binding.
        public GameStatisticsScreen(string totaltime, string wordsperminuut, int score, string errorpercentage, string wordcount)
        {
            InitializeComponent(); // Initializes the XAML components.
            DataContext = this; // Sets the current class as the data source for bindings.
            this.Score = score;
            this.TotalTime = totaltime;
            this.WordCount = wordcount;
            this.WordsPerMinute = wordsperminuut;
            this.ErrorPercentage = errorpercentage;

            StartStoryboardAnimation(); // Starts the storyboard animation when the screen is loaded.
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

        // Method to start the storyboard animation.
        private void StartStoryboardAnimation()
        {
            // Ensure storyboard is defined in XAML and start it from resources
            var storyboard = (Storyboard)FindResource("AppearInSequence");
            if (storyboard != null)
            {
                storyboard.Begin();
            }
        }
    }
}