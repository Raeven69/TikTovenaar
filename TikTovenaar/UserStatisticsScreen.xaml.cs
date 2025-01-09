using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using TikTovenaar.Logic;
using static System.Formats.Asn1.AsnWriter;

namespace TikTovenaar
{
    // This is the UserControl that displays the statistics screen for the user.
    public partial class UserStatisticsScreen : UserControl
    {
        // Constructor for the UserStatisticsScreen.


        private List<StatisticsWord> _words = new List<StatisticsWord>();

        public ObservableCollection<string> SortOptions { get; set; }

        private IDataHandler _dataHandler;

        private List<Score> _userScores;

        public UserStatisticsScreen(IDataHandler _dataHandler)
        {

            LoadingScreen loadingScreen = new LoadingScreen();
            loadingScreen.Show();
            this._dataHandler = _dataHandler;
            _userScores = this._dataHandler.GetScores(CurrentUser.Instance.Token);
            List<string> wordsList = new List<string>();
            Dictionary<string, (int totalCorrect, int totalIncorrect)> wordsDictionary = new Dictionary<string, (int, int)>();



            InitializeComponent();

            // These are the available options
            SortOptions = new ObservableCollection<string>
            {
                "Alfabetisch",
                "Aantal woorden goed",
                "Aantal woorden fout",
                "Totaal gespeelde spellen"
            };

            // Set logic to be executed once the screen is fully loaded.
            this.Loaded += (s, e) =>
            {
                int totalWordsTypedCorrectly = 0;
                int totalWordsTypedIncorrectly = 0;

                foreach (Score score in _userScores)
                {
                    foreach (var incorrectWord in score.IncorrectWords)
                    {
                        totalWordsTypedIncorrectly++;
                        if (!wordsDictionary.ContainsKey(incorrectWord))
                        {
                            wordsList.Add(incorrectWord);
                            wordsDictionary.Add(incorrectWord, (0, 1));
                        }
                        else
                        {
                            (int totalCorrect, int totalIncorrect) current = wordsDictionary[incorrectWord];
                            wordsDictionary[incorrectWord] = (current.totalCorrect, current.totalIncorrect + 1);
                        }

                    }
                    foreach (var correctWord in score.CorrectWords)
                    {
                        totalWordsTypedCorrectly++;
                        if (!wordsDictionary.ContainsKey(correctWord))
                        {
                            wordsList.Add(correctWord);
                            wordsDictionary.Add(correctWord, (1, 0));
                        }
                        else
                        {
                            (int totalCorrect, int totalIncorrect) current = wordsDictionary[correctWord];
                            wordsDictionary[correctWord] = (current.totalCorrect + 1, current.totalIncorrect);
                        }
                    }
                }

                wordsList = wordsList.Distinct().ToList();
                foreach (string word in wordsList)
                {
                    _words.Add(new StatisticsWord(word, wordsDictionary[word].totalCorrect, wordsDictionary[word].totalIncorrect));
                }
                PrintButtons();

                int averageScore = 0;
                int totalScore = 0;
                int totalWordsTyped = 0;

                int totalGamesPlayed = _userScores.Count;



                // Sample data for graphs.
                List<int> wpmData = new List<int> { }; // WPM values.
                List<int> goodPercentageData = new List<int> { }; // Error percentages.
                List<int> rightWordsData = new List<int> { }; // Correct words.
                List<int> wrongWordsData = new List<int> { }; // Incorrect words.

                foreach (Score score in _userScores)
                {
                    double gameTime = score.Duration.TotalMinutes;
                    wpmData.Add(gameTime == 0 ? 0 : (int)(score.WordsAmount / gameTime));
                    goodPercentageData.Add(score.WordsAmount == 0 ? 0 : (int)(((double)score.CorrectWords.Count / score.WordsAmount) * 100));
                    rightWordsData.Add(score.CorrectWords.Count);
                    wrongWordsData.Add(score.IncorrectWords.Count);

                    averageScore += score.Value;
                    totalScore += score.Value;
                    totalWordsTyped += score.WordsAmount;

                }

                if (_userScores.Count == 0)
                {
                    averageScore = 0;
                }
                else
                {
                    averageScore /= _userScores.Count;
                }

                // Sample data to display the statistics - replace with actual data.
                AvgScoreText.Text = averageScore.ToString(); // Average score.
                TotalScoreText.Text = totalScore.ToString(); // Total score.
                TotalWordsTypedText.Text = totalWordsTyped.ToString(); // Total words typed..
                TotalWordsTypedIncorrectly.Text = totalWordsTypedIncorrectly.ToString(); // Total incorrect words typed.
                TotalWordsTypedCorrectly.Text = totalWordsTypedCorrectly.ToString(); // Total correct words typed.
                TotalGamesPlayed.Text = totalGamesPlayed.ToString(); // Total games played.

                // Draw the various graphs with the sample data.
                DrawWPMGraph(wpmData);
                DrawGoodPercentageGraph(goodPercentageData);
                DrawRightWrongGraph(rightWordsData, wrongWordsData);


                // Lower the scroll speed to avoid it scrolling too fast.
                scrollViewer.PreviewMouseWheel += (s, e) => scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + (e.Delta > 0 ? -20 : 20));
                loadingScreen.Close();
            };

            DataContext = this;

        }

        // Method to return to the home screen.
        private void Return_Click(object sender, RoutedEventArgs e)
        {
            // Get a reference to the main window.
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            // Switch to the home screen.
            mainWindow.SwitchToHomeScreen();
        }

        // Method to handle the click events of the dynamically created word buttons.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Get the clicked button.
            Button clickedButton = sender as Button;
            // Show the name of the clicked button in a message window.
            DetailsDialog detailsDialog = new DetailsDialog(clickedButton.Tag.ToString());
            detailsDialog.ShowDialog();
        }

        private DispatcherTimer _searchDebounceTimer;

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            // The extra code is needed to perform fewer calculations and make the program faster
            // If the timer doesn't exist, create it.
            if (_searchDebounceTimer == null)
            {
                _searchDebounceTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(300)
                };
                // Execute the filter function once the timer expires.
                _searchDebounceTimer.Tick += (s, args) =>
                {
                    _searchDebounceTimer.Stop();
                    PrintButtons(); // Execute the Print function
                };
            }
            // If the timer is already running, stop it and start again.
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }


        private void PrintButtons()
        {
            StatisticsWord[] sortedWords;

            string searchText = searchBar.Text.ToLower();

            // Here, the words are sorted and ordered
            if (OrderButton.Content.Equals("▲"))
            {
                if (SortOptionsButton.SelectedItem.Equals("Alfabetisch"))
                {
                    // Sort by alphabetical order
                    sortedWords = _words.Where(word => word.Word.StartsWith(searchText)).OrderBy(w => w.Word).ToArray();
                }
                else if (SortOptionsButton.SelectedItem.Equals("Aantal woorden goed"))
                {
                    // Sort by number of correct, large to small, and if two have the same number of correct, sort alphabetically
                    sortedWords = _words.Where(word => word.Word.StartsWith(searchText)).OrderByDescending(w => w.TotalCorrect).ThenBy(w => w.Word).ToArray();
                }
                else if (SortOptionsButton.SelectedItem.Equals("Aantal woorden fout"))
                {
                    // Sort by number of incorrect, large to small, and if two have the same number of incorrect, sort alphabetically
                    sortedWords = _words.Where(word => word.Word.StartsWith(searchText)).OrderByDescending(w => w.TotalWrong).ThenBy(w => w.Word).ToArray();
                }
                else if (SortOptionsButton.SelectedItem.Equals("Totaal gespeelde spellen"))
                {
                    // Sort by total played, large to small, and if two have the same total played, sort alphabetically
                    sortedWords = _words.Where(word => word.Word.StartsWith(searchText)).OrderByDescending(w => w.TotalPlayed).ThenBy(w => w.Word).ToArray();
                }
                else // if no option is selected, this is impossible but in case of a bug, just sort alphabetically
                {
                    sortedWords = _words.Where(word => word.Word.StartsWith(searchText)).OrderBy(w => w.Word).ToArray();
                }
            }
            else
            {
                if (SortOptionsButton.SelectedItem.Equals("Alfabetisch"))
                {
                    // Sort in reverse alphabetical order
                    sortedWords = _words.Where(word => word.Word.StartsWith(searchText)).OrderByDescending(w => w.Word).ToArray();
                }
                else if (SortOptionsButton.SelectedItem.Equals("Aantal woorden goed"))
                {
                    // Sort by number of correct, small to large, and if two have the same number of correct, sort alphabetically
                    sortedWords = _words.Where(word => word.Word.StartsWith(searchText)).OrderBy(w => w.TotalCorrect).ThenBy(w => w.Word).ToArray();
                }
                else if (SortOptionsButton.SelectedItem.Equals("Aantal woorden fout"))
                {
                    // Sort by number of incorrect, small to large, and if two have the same number of incorrect, sort alphabetically
                    sortedWords = _words.Where(word => word.Word.StartsWith(searchText)).OrderBy(w => w.TotalWrong).ThenBy(w => w.Word).ToArray();
                }
                else if (SortOptionsButton.SelectedItem.Equals("Totaal gespeelde spellen"))
                {
                    // Sort by total played, small to large, and if two have the same total played, sort alphabetically
                    sortedWords = _words.Where(word => word.Word.StartsWith(searchText)).OrderBy(w => w.TotalPlayed).ThenBy(w => w.Word).ToArray();
                }
                else // if no option is selected, this is impossible but in case of a bug, just sort in reverse alphabetical order
                {
                    sortedWords = _words.Where(word => word.Word.StartsWith(searchText)).OrderByDescending(w => w.Word).ToArray();
                }

            }


            // Clears the grid to make room for a new grid with the sorted words
            DynamicGrid.Children.Clear();
            DynamicGrid.RowDefinitions.Clear();
            DynamicGrid.ColumnDefinitions.Clear();
            int wordCount = sortedWords.Length;
            int cellCount;
            if (wordCount > 30)
            {
                cellCount = 30;
            }
            else
            {
                cellCount = sortedWords.Length;
            }
            int numCols = 2;
            int numRows = (int)Math.Ceiling((double)cellCount / numCols);

            for (int i = 0; i < numCols; ++i)
            {
                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < numRows; ++i)
            {
                DynamicGrid.RowDefinitions.Add(new RowDefinition());
            }


            foreach (var g in DynamicGrid.RowDefinitions)
            {
                g.Height = new GridLength(ActualHeight / 8);
            }

            foreach (var g in DynamicGrid.ColumnDefinitions)
            {
                g.Width = new GridLength(ActualWidth / numCols * 0.8);
            }

            for (int i = 0; i < cellCount; i++)
            {
                Button button = new Button
                {
                    Tag = sortedWords[i].Word,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0D1117")),
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0")),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F0F0")),
                    Content = new StackPanel
                    {
                        Children =
                        {
                            new TextBlock
                            {
                                FontSize = 20,
                                Text = sortedWords[i].ToStringWord(),
                                TextAlignment = TextAlignment.Center
                            },
                            new TextBlock
                            {
                                FontSize = 15,
                                Text = sortedWords[i].ToStringStats(),
                                TextAlignment = TextAlignment.Center
                            }
                        }
                    }
                };

                button.Click += Button_Click;

                DynamicGrid.Children.Add(button);

                button.SetValue(Grid.RowProperty, i / numCols);
                button.SetValue(Grid.ColumnProperty, i % numCols);
            }
        }
    

        private void OrderButtonClick(object sender, RoutedEventArgs e)
        {
            if (OrderButton.Content.Equals("▲"))
            {
                OrderButton.Content = "▼";
            }
            else
            {
                OrderButton.Content = "▲";
            }
            PrintButtons();
        }

        private void SortOptiesVerandering(object sender, SelectionChangedEventArgs e)
        {
            PrintButtons();
        }

        // Method to draw wpm graph
        private void DrawWPMGraph(List<int> data)
        {
            if (data.Count <= 2)
            {
                // Show the message and hide the graph
                WPMPlotView.Visibility = Visibility.Collapsed;
                WPMMessageText.Visibility = Visibility.Visible;
                return;
            }

            // Hide the message and show the graph
            WPMPlotView.Visibility = Visibility.Visible;
            WPMMessageText.Visibility = Visibility.Collapsed;

            // Create a new chart model for WPM.
            var plotModel = new PlotModel { Title = "Gemiddeld WPM" };

            // Set styles for the chart.
            plotModel.SubtitleColor = OxyColors.White;
            plotModel.TextColor = OxyColors.White;
            plotModel.PlotAreaBorderColor = OxyColors.White;

            // Create a line series for the WPM values.
            var lineSeries = new LineSeries
            {
                Title = "WPM",
                Color = OxyColors.Blue
            };

            // Determine the maximum WPM value for the y-axis.
            double maxValue = 0;
            for (int i = 0; i < data.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i, data[i]));
                if (data[i] > maxValue)
                    maxValue = data[i];
            }

            // Add the series to the chart model.
            plotModel.Series.Add(lineSeries);

            // Add an x-axis representing the number of games.
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Aantal gespeelde spellen",
                Minimum = 0,
                Maximum = data.Count - 1,
                MajorStep = 1,
                AbsoluteMinimum = 0,
                IsZoomEnabled = false
            };

            // Add a y-axis for the WPM values.
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "WPM",
                Minimum = 0,
                Maximum = maxValue + (maxValue * 0.1), // Add a 10% margin to the maximum value.
                AbsoluteMinimum = 0,
                IsZoomEnabled = false
            };

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);

            // Bind the chart model to the WPM plot view.
            WPMPlotView.Model = plotModel;
        }

        // Method to draw a graph for the average correct percentage.
        private void DrawGoodPercentageGraph(List<int> data)
        {
            if (data.Count <= 2)
            {
                // Show the message and hide the graph
                WrongPercentagePlotView.Visibility = Visibility.Collapsed;
                GoodPercentageMessageText.Visibility = Visibility.Visible;
                return;
            }

            WrongPercentagePlotView.Visibility = Visibility.Visible;
            GoodPercentageMessageText.Visibility = Visibility.Collapsed;

            // Create a new chart model for correct percentages.
            var plotModel = new PlotModel { Title = "Gemiddeld goed percentage (%)" };

            // Set styles for the chart.
            plotModel.SubtitleColor = OxyColors.White;
            plotModel.TextColor = OxyColors.White;
            plotModel.PlotAreaBorderColor = OxyColors.White;

            // Create a line series for correct percentages.
            var lineSeries = new LineSeries
            {
                Title = "Correct percentage (%)",
                Color = OxyColors.Green
            };

            // Determine the maximum correct percentage for the y-axis.
            double maxValue = 0;
            for (int i = 0; i < data.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i, data[i]));
                if (data[i] > maxValue)
                    maxValue = data[i];
            }

            // Add the series to the chart model.
            plotModel.Series.Add(lineSeries);

            // Add an x-axis representing the number of games.
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Aantal gespeelde spellen",
                Minimum = 0,
                Maximum = data.Count - 1,
                MajorStep = 1,
                AbsoluteMinimum = 0,
                IsZoomEnabled = false
            };

            // Add a y-axis for correct percentages.
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Goed Percentage (%)",
                Minimum = 0,
                Maximum = Math.Min(maxValue + (maxValue * 0.1), 100), // Ensure the percentage does not exceed 100%.
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 100,
                IsZoomEnabled = false
            };

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);

            // Bind the chart model to the correct percentage plot view.
            WrongPercentagePlotView.Model = plotModel;
        }


        // Methode om een grafiek te tekenen die het aantal correcte en foute woorden toont.
        private void DrawRightWrongGraph(List<int> rightWordsData, List<int> wrongWordsData)
        {
            if (rightWordsData.Count <= 2 || wrongWordsData.Count <= 2)
            {
                // Show the message and hide the graph
                RightWrongPlotView.Visibility = Visibility.Collapsed;
                RightWrongMessageText.Visibility = Visibility.Visible;
                return;
            }
            // Hide the message and show the graph
            RightWrongPlotView.Visibility = Visibility.Visible;
            RightWrongMessageText.Visibility = Visibility.Collapsed;
            // Create a new chart model for correct and incorrect words.
            var plotModel = new PlotModel { Title = "Totaal aantal woorden goed en fout" };
            // Set styles for the chart.
            plotModel.SubtitleColor = OxyColors.White;
            plotModel.TextColor = OxyColors.White;
            plotModel.PlotAreaBorderColor = OxyColors.White;
            // Create a line series for correct words.
            var rightSeries = new LineSeries
            {
                Title = "Goed getypt",
                Color = OxyColors.Green
            };

            // Create a line series for incorrect words.
            var wrongSeries = new LineSeries
            {
                Title = "Foute woorden",
                Color = OxyColors.Red
            };

            // Determine the maximum number of words for the y-axis.
            double maxValue = 0;
            for (int i = 0; i < rightWordsData.Count; i++)
            {
                rightSeries.Points.Add(new DataPoint(i, rightWordsData[i]));
                wrongSeries.Points.Add(new DataPoint(i, wrongWordsData[i]));
                maxValue = Math.Max(maxValue, Math.Max(rightWordsData[i], wrongWordsData[i]));
            }

            // Add the series to the chart model.
            plotModel.Series.Add(rightSeries);
            plotModel.Series.Add(wrongSeries);

            // Add an x-axis representing the number of games.
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Aantal gespeelde spellen",
                Minimum = 0,
                Maximum = rightWordsData.Count - 1,
                MajorStep = 1,
                AbsoluteMinimum = 0,
                IsZoomEnabled = false
            };

            // Add a y-axis for the number of words.
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Aantal getypte woorden",
                Minimum = 0,
                Maximum = maxValue + (maxValue * 0.1), // Add a 10% margin to the maximum value.
                AbsoluteMinimum = 0,
                IsZoomEnabled = false
            };

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);

            // Bind the chart model to the plot view for correct and incorrect words.
            RightWrongPlotView.Model = plotModel;
        }


    }
}
