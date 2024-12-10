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
    // Dit is de UserControl die het statistiekenscherm voor de gebruiker weergeeft.
    public partial class UserStatisticsScreen : UserControl
    {
        // Constructor voor het UserStatisticsScreen.

        
        List<StatisticsWord> woorden = new List<StatisticsWord>();

        public ObservableCollection<string> sortOpties { get; set; }

        private IDataHandler dataHandler;

        private List<ScoreEntry> userScores;

        public UserStatisticsScreen(IDataHandler _dataHandler)
        {

            LoadingScreen loadingScreen = new LoadingScreen();
            loadingScreen.Show();
            dataHandler = _dataHandler;
            userScores = dataHandler.GetScores(CurrentUser.Instance.Token);
            List<string> woordenList = dataHandler.GetWords();
            Dictionary<string, (int totaalGoed, int totaalFout)> woordenDictionary = new Dictionary<string, (int, int)>();




            InitializeComponent();

            // dit zijn de opties die er zijn
            sortOpties = new ObservableCollection<string>
            {
                "Alfabetisch",
                "Aantal goed",
                "Aantal fout",
                "Totaal gespeeld"
            };

            // Stel in dat de logica wordt uitgevoerd zodra het scherm volledig geladen is.
            this.Loaded += (s, e) =>
            {
                int totalWordsTypedCorrectly = 0;
                int totalWordsTypedIncorrectly = 0;
                foreach (string woord in woordenList)
                {
                    if (!woordenDictionary.ContainsKey(woord))
                    {
                        woordenDictionary.Add(woord, (0, 0));
                    }
                }
                foreach (ScoreEntry score in userScores)
                {
                    foreach (var incorrectWord in score.IncorrectWords)
                    {
                        totalWordsTypedIncorrectly++;
                        if (!woordenDictionary.ContainsKey(incorrectWord))
                        {
                            woordenList.Add(incorrectWord);
                            woordenDictionary.Add(incorrectWord, (0, 1));
                        }
                        else
                        {
                            (int totaalGoed, int totaalFout) current = woordenDictionary[incorrectWord];
                            woordenDictionary[incorrectWord] = (current.totaalGoed, current.totaalFout + 1);
                        }

                    }
                    foreach (var correctWord in score.CorrectWords)
                    {
                        totalWordsTypedCorrectly++;
                        if (!woordenDictionary.ContainsKey(correctWord))
                        {
                            woordenList.Add(correctWord);
                            woordenDictionary.Add(correctWord, (1, 0));
                        }
                        else
                        {
                            (int totaalGoed, int totaalFout) current = woordenDictionary[correctWord];
                            woordenDictionary[correctWord] = (current.totaalGoed + 1, current.totaalFout);
                        }
                    }
                }

                woordenList = woordenList.Distinct().ToList();
                foreach (string woord in woordenList)
                {
                    woorden.Add(new StatisticsWord(woord, woordenDictionary[woord].totaalGoed, woordenDictionary[woord].totaalFout));
                }
                PrintButtons();

                int averageScore = 0;
                int totalScore = 0;
                int totalWordsTyped = 0;

                int totalGamesPlayed = userScores.Count;
                

                // Voorbeeldgegevens voor grafieken.
                List<int> wpmData = new List<int> { }; // WPM-waarden.
                List<int> goodPercentageData = new List<int> { }; // Foutpercentages.
                List<int> rightWordsData = new List<int> { }; // Correcte woorden.
                List<int> wrongWordsData = new List<int> { }; // Foute woorden

                foreach ( ScoreEntry score in userScores ) {
                    int gameTime = (int)(DateTime.Now - score.Time).TotalMinutes;
                    wpmData.Add(gameTime == 0 ? 0 : (int)((double)score.WordsAmount / gameTime));
                    goodPercentageData.Add(score.WordsAmount == 0 ? 0 : (int)(((double)score.CorrectWords.Count/score.WordsAmount)*100));
                    rightWordsData.Add(score.CorrectWords.Count);
                    wrongWordsData.Add(score.IncorrectWords.Count);

                    averageScore += score.Value;
                    totalScore += score.Value;
                    totalWordsTyped += score.WordsAmount;




                }
                averageScore /= userScores.Count;

                // Voorbeeldgegevens om de statistieken te tonen - vervang door echte gegevens.
                AvgScoreText.Text = averageScore.ToString(); // Gemiddelde score.
                TotalScoreText.Text = totalScore.ToString(); // Totale score.
                TotalWordsTypedText.Text = totalWordsTyped.ToString(); // Totaal aantal getypte woorden..
                TotalWordsTypedIncorrectly.Text = totalWordsTypedIncorrectly.ToString(); // Totaal aantal fout getypte woorden.
                TotalWordsTypedCorrectly.Text = totalWordsTypedCorrectly.ToString(); // Totaal aantal correct getypte woorden.
                TotalGamesPlayed.Text = totalGamesPlayed.ToString(); // Totaal aantal gespeelde spellen.

                // Teken de verschillende grafieken met de voorbeeldgegevens.
                DrawWPMGraph(wpmData);
                DrawGoodPercentageGraph(goodPercentageData);
                DrawRightWrongGraph(rightWordsData, wrongWordsData);


                // breng de scrollsnelheid naar beneden anders gaat die wel heel snel
                scrollViewer.PreviewMouseWheel += (s, e) => scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + (e.Delta > 0 ? -20 : 20));
                loadingScreen.Close();
            };

            DataContext = this;

        }

        // Methode om terug te keren naar het beginscherm.
        private void Terug_Click(object sender, RoutedEventArgs e)
        {
            // Verkrijg een referentie naar het hoofdvenster.
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            // Wissel naar het beginscherm.
            mainWindow.SwitchToHomeScreen();
        }

        // Methode om de klikgebeurtenissen van de dynamisch gemaakte woordknoppen te verwerken.
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Verkrijg de knop die is aangeklikt.
            Button clickedButton = sender as Button;
            // Toon de naam van de aangeklikte knop in een berichtvenster.
            DetailsDialog detailsDialog = new DetailsDialog(clickedButton.Tag.ToString());
            detailsDialog.ShowDialog();
        }

        private DispatcherTimer searchDebounceTimer;

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            //de extra code is nodig zodat er minder berekeningen zijn en het programma sneller is
            // Als de timer nog niet bestaat, maak deze aan.
            if (searchDebounceTimer == null)
            {
                searchDebounceTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(300)
                };
                // Voer de filterfunctie uit zodra de timer afloopt.
                searchDebounceTimer.Tick += (s, args) =>
                {
                    searchDebounceTimer.Stop();
                    PrintButtons(); // Voer de PrintFunctie uit
                };
            }
            // Als de timer al loopt, stop deze en start opnieuw.
            searchDebounceTimer.Stop();
            searchDebounceTimer.Start();
        }


        private void PrintButtons()
        {
            StatisticsWord[] gesorteerdeWoorden;

            string searchText = searchBar.Text.ToLower();

            // hier worden de worden gesorteerd en geordert
            if (VolgwordenButton.Content.Equals("▲"))
            {
                if (SortOptiesButton.SelectedItem.Equals("Alfabetisch"))
                {
                    // word gesorteerd op alfabetische volgorde
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.StartsWith(searchText)).OrderBy(w => w.woord).ToArray();
                }
                else if (SortOptiesButton.SelectedItem.Equals("Aantal goed"))
                {
                    // word gesorteerd op aantal goed groot naar klein en als er 2 het zelfde aantal goed hebben worden ze op alfabetische volgorde gesorteerd
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.StartsWith(searchText)).OrderByDescending(w => w.totaalGoed).ThenBy(w => w.woord).ToArray();
                }
                else if (SortOptiesButton.SelectedItem.Equals("Aantal fout"))
                {
                    // word gesorteerd op aantal fout groot naar klein en als er 2 het zelfde aantal fout hebben worden ze op alfabetische volgorde gesorteerd
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.StartsWith(searchText)).OrderByDescending(w => w.totaalFout).ThenBy(w => w.woord).ToArray();
                }
                else if (SortOptiesButton.SelectedItem.Equals("Totaal gespeeld"))
                {
                    // word gesorteerd op totaal gespeelt groot naar klein en als er 2 het zelfde aantal totaal gespeelt hebben worden ze op alfabetische volgorde gesorteerd
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.StartsWith(searchText)).OrderByDescending(w => w.totaalGespeelt).ThenBy(w => w.woord).ToArray();
                }
                else // als er geen optie is geselecteert dit is onmogelijk maar als er een bug is is dit handig word gewoon op alfabetisch gedaan
                {
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.StartsWith(searchText)).OrderBy(w => w.woord).ToArray();
                }
            }
            else
            {
                if (SortOptiesButton.SelectedItem.Equals("Alfabetisch"))
                {
                    // word gesorteerd op omgekeerde alfabetische volgorde
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.StartsWith(searchText)).OrderByDescending(w => w.woord).ToArray();
                }
                else if (SortOptiesButton.SelectedItem.Equals("Aantal goed"))
                {
                    // word gesorteerd op aantal goed klein naar groot en als er 2 het zelfde aantal goed hebben worden ze op alfabetische volgorde gesorteerd
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.StartsWith(searchText)).OrderBy(w => w.totaalGoed).ThenBy(w => w.woord).ToArray();
                }
                else if (SortOptiesButton.SelectedItem.Equals("Aantal fout"))
                {
                    // word gesorteerd op aantal fout klein naar groot en als er 2 het zelfde aantal fout hebben worden ze op alfabetische volgorde gesorteerd
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.StartsWith(searchText)).OrderBy(w => w.totaalFout).ThenBy(w => w.woord).ToArray();
                }
                else if (SortOptiesButton.SelectedItem.Equals("Totaal gespeeld"))
                {
                    // word gesorteerd op totaal gespeelt klein naar grooten als er 2 het zelfde aantal totaal gespeelt hebben worden ze op alfabetische volgorde gesorteerd
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.StartsWith(searchText)).OrderBy(w => w.totaalGespeelt).ThenBy(w => w.woord).ToArray();
                }
                else // als er geen optie is geselecteert dit is onmogelijk maar als er een bug is is dit handig word gewoon op omgekeerd alfabetisch gedaan
                {
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.StartsWith(searchText)).OrderByDescending(w => w.woord).ToArray();
                }

            }


            // maakt het grid weer leeg zodat er weer voor de gesorteerde worden een nieuwe grid gemaakt kan worden 
            DynamicGrid.Children.Clear();
            DynamicGrid.RowDefinitions.Clear();
            DynamicGrid.ColumnDefinitions.Clear();
            int aantalWorden = gesorteerdeWoorden.Length;
            int cellCount;
            if (aantalWorden > 30)
            {
                cellCount = 30;
            }
            else
            {
                 cellCount = gesorteerdeWoorden.Length;
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
                    Tag = gesorteerdeWoorden[i].woord,
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
                                Text = gesorteerdeWoorden[i].ToStringWord(),
                                TextAlignment = TextAlignment.Center
                            },
                            new TextBlock
                            {
                                FontSize = 15,
                                Text = gesorteerdeWoorden[i].ToStringStats(),
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


        // zorgt er voor dat de button werkt en dus het anders sorteert
        private void VolgwordenButtonClick(object sender, RoutedEventArgs e)
        {
            if (VolgwordenButton.Content.Equals("▲"))
            {
                VolgwordenButton.Content = "▼";
            }
            else
            {
                VolgwordenButton.Content = "▲";
            }
            PrintButtons();
        }

        private void SortOptiesVerandering(object sender, SelectionChangedEventArgs e)
        {
            PrintButtons();
        }

        // Methode om een grafiek te tekenen die het gemiddelde WPM toont.
        private void DrawWPMGraph(List<int> data)
        {
            // Maak een nieuw grafiekmodel voor WPM.
            var plotModel = new PlotModel { Title = "Gemiddelde WPM" };

            // Stel stijlen in voor de grafiek.
            plotModel.SubtitleColor = OxyColors.White;
            plotModel.TextColor = OxyColors.White;
            plotModel.PlotAreaBorderColor = OxyColors.White;

            // Maak een lijnserie voor de WPM-waarden.
            var lineSeries = new LineSeries
            {
                Title = "WPM",
                Color = OxyColors.Blue
            };

            // Bepaal het maximale WPM-waarde voor de y-as.
            double maxValue = 0;
            for (int i = 0; i < data.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i, data[i]));
                if (data[i] > maxValue)
                    maxValue = data[i];
            }

            // Voeg de serie toe aan het grafiekmodel.
            plotModel.Series.Add(lineSeries);

            // Voeg een x-as toe die het aantal spellen vertegenwoordigt.
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Aantal Spellen",
                Minimum = 0,
                Maximum = data.Count - 1,
                MajorStep = 1,
                AbsoluteMinimum = 0,
                IsZoomEnabled = false
            };

            // Voeg een y-as toe voor de WPM-waarden.
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "WPM",
                Minimum = 0,
                Maximum = maxValue + (maxValue * 0.1), // Voeg 10% marge toe aan de maximumwaarde.
                AbsoluteMinimum = 0,
                IsZoomEnabled = false
            };

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);

            // Koppel het grafiekmodel aan de WPM-plotweergave.
            WPMPlotView.Model = plotModel;
        }

        // Methode om een grafiek te tekenen voor het gemiddelde foutpercentage.
        private void DrawGoodPercentageGraph(List<int> data)
        {
            // Maak een nieuw grafiekmodel voor foutpercentages.
            var plotModel = new PlotModel { Title = "Gemiddeld Goed Percentage" };

            // Stel stijlen in voor de grafiek.
            plotModel.SubtitleColor = OxyColors.White;
            plotModel.TextColor = OxyColors.White;
            plotModel.PlotAreaBorderColor = OxyColors.White;

            // Maak een lijnserie voor foutpercentages.
            var lineSeries = new LineSeries
            {
                Title = "Goed Percentage",
                Color = OxyColors.Green
            };

            // Bepaal het maximale foutpercentage voor de y-as.
            double maxValue = 0;
            for (int i = 0; i < data.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i, data[i]));
                if (data[i] > maxValue)
                    maxValue = data[i];
            }

            // Voeg de serie toe aan het grafiekmodel.
            plotModel.Series.Add(lineSeries);

            // Voeg een x-as toe die het aantal spellen vertegenwoordigt.
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Aantal Spellen",
                Minimum = 0,
                Maximum = data.Count - 1,
                MajorStep = 1,
                AbsoluteMinimum = 0,
                IsZoomEnabled = false
            };

            // Voeg een y-as toe voor foutpercentages.
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Goed Percentage (%)",
                Minimum = 0,
                Maximum = Math.Min(maxValue + (maxValue * 0.1), 100), // Zorg dat het percentage niet boven 100% komt.
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 100,
                IsZoomEnabled = false
            };

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);

            // Koppel het grafiekmodel aan de foutpercentage-plotweergave.
            WrongPercentagePlotView.Model = plotModel;
        }

        // Methode om een grafiek te tekenen die het aantal correcte en foute woorden toont.
        private void DrawRightWrongGraph(List<int> rightWordsData, List<int> wrongWordsData)
        {
            // Maak een nieuw grafiekmodel voor correcte en foute woorden.
            var plotModel = new PlotModel { Title = "Totaal Aantal Woorden Goed en Fout" };

            // Stel stijlen in voor de grafiek.
            plotModel.SubtitleColor = OxyColors.White;
            plotModel.TextColor = OxyColors.White;
            plotModel.PlotAreaBorderColor = OxyColors.White;

            // Maak een lijnserie voor correcte woorden.
            var rightSeries = new LineSeries
            {
                Title = "Goed Getypt",
                Color = OxyColors.Green
            };

            // Maak een lijnserie voor foute woorden.
            var wrongSeries = new LineSeries
            {
                Title = "Fout Getypt",
                Color = OxyColors.Red
            };

            // Bepaal het maximale aantal woorden voor de y-as.
            double maxValue = 0;
            for (int i = 0; i < rightWordsData.Count; i++)
            {
                rightSeries.Points.Add(new DataPoint(i, rightWordsData[i]));
                wrongSeries.Points.Add(new DataPoint(i, wrongWordsData[i]));
                maxValue = Math.Max(maxValue, Math.Max(rightWordsData[i], wrongWordsData[i]));
            }

            // Voeg de series toe aan het grafiekmodel.
            plotModel.Series.Add(rightSeries);
            plotModel.Series.Add(wrongSeries);

            // Voeg een x-as toe die het aantal spellen vertegenwoordigt.
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Aantal Spellen",
                Minimum = 0,
                Maximum = rightWordsData.Count - 1,
                MajorStep = 1,
                AbsoluteMinimum = 0,
                IsZoomEnabled = false
            };

            // Voeg een y-as toe voor het aantal woorden.
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Aantal Woorden",
                Minimum = 0,
                Maximum = maxValue + (maxValue * 0.1), // Voeg 10% marge toe aan de maximumwaarde.
                AbsoluteMinimum = 0,
                IsZoomEnabled = false
            };

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);

            // Koppel het grafiekmodel aan de plotweergave voor correcte en foute woorden.
            RightWrongPlotView.Model = plotModel;
        }

    }
}
