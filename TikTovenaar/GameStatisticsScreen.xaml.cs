using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TikTovenaar.Logic;

namespace TikTovenaar
{
    // Dit is de UserControl die het statistiekenscherm voor de gebruiker weergeeft.
    public partial class UserStatisticsScreen : UserControl
    {
        // Dummy woordstatistieken om de dynamische elementen te testen.
        StatisticsWord[] woorden =
                {
                    // Lijst met woorden en hun statistieken (score en fouten).
                    new StatisticsWord("kindercarnavalsoptochtvoorbereidingswerkzaamheden", 10, 5),
                    new StatisticsWord("aan", 10, 5),
                    new StatisticsWord("aandoen", 10, 2),
                    new StatisticsWord("aanduiding", 10, 4),
                    new StatisticsWord("aanhouden", 10, 8),
                    new StatisticsWord("aankijken", 10, 10),
                    new StatisticsWord("aanklacht", 10, 10),
                    new StatisticsWord("aanklacht", 10, 10),
                    new StatisticsWord("aanklacht", 10, 10),
                    new StatisticsWord("aanklacht", 10, 10),
                    new StatisticsWord("aanklacht", 10, 10),
                    new StatisticsWord("aanklacht", 10, 10),
                    new StatisticsWord("aanklacht", 10, 10),
                    new StatisticsWord("aanklacht", 10, 10),
                    new StatisticsWord("aanklacht", 10, 10),
                    new StatisticsWord("aanklacht", 10, 10),
                    new StatisticsWord("aanklacht", 10, 10),
                    new StatisticsWord("aanklacht", 10, 10),
                    new StatisticsWord("aanklacht", 10, 10),
                    new StatisticsWord("aanklacht", 10, 10),
                    new StatisticsWord("aanklacht", 10, 10),
                    new StatisticsWord("aanklacht", 10, 10),
                    new StatisticsWord("aanklacht", 10, 10),
                };

        public ObservableCollection<string> sortOpties { get; set; }

        // Constructor voor het UserStatisticsScreen.
        public UserStatisticsScreen()
        {
            InitializeComponent();

            sortOpties = new ObservableCollection<string>
            {
                "Alfabetisch",
                "Aantal goed",
                "Aantal fout",
                "Totaal gespeelt"
            };

            // Stel in dat de logica wordt uitgevoerd zodra het scherm volledig geladen is.
            this.Loaded += (s, e) =>
            {

                PrintButtons();

                // Voorbeeldgegevens om de statistieken te tonen - vervang door echte gegevens.
                AvgScoreText.Text = "85"; // Gemiddelde score.
                TotalScoreText.Text = "340"; // Totale score.
                TotalWordsTypedText.Text = "120"; // Totaal aantal getypte woorden.

                // Voorbeeldgegevens voor grafieken.
                List<int> wpmData = new List<int> { 60, 70, 65, 75, 80 }; // WPM-waarden.
                List<int> wrongPercentageData = new List<int> { 5, 4, 3, 6, 4 }; // Foutpercentages.
                List<int> rightWordsData = new List<int> { 100, 120, 110, 130, 140 }; // Correcte woorden.
                List<int> wrongWordsData = new List<int> { 10, 8, 12, 7, 9 }; // Foute woorden.

                // Teken de verschillende grafieken met de voorbeeldgegevens.
                DrawWPMGraph(wpmData);
                DrawWrongPercentageGraph(wrongPercentageData);
                DrawRightWrongGraph(rightWordsData, wrongWordsData);
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
            MessageBox.Show(clickedButton.Name);
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            PrintButtons();
        }

        private void PrintButtons()
        {
            StatisticsWord[] gesorteerdeWoorden;

            // hier worden de worden gesorteerd en geordert
            if (VolgwordenButton.Content.Equals("▲"))
            {
                if (SortOptiesButton.SelectedItem.Equals("Alfabetisch"))
                {
                    // word gesorteerd op alfabetische volgorde
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.Contains(searchBar.Text)).OrderBy(w => w.woord).ToArray();
                }
                else if (SortOptiesButton.SelectedItem.Equals("Aantal goed"))
                {
                    // word gesorteerd op aantal goed groot naar klein en als er 2 het zelfde aantal goed hebben worden ze op alfabetische volgorde gesorteerd
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.Contains(searchBar.Text)).OrderBy(w => w.woord).OrderByDescending(w => w.totaalGoed).ToArray();
                }
                else if (SortOptiesButton.SelectedItem.Equals("Aantal fout"))
                {
                    // word gesorteerd op aantal fout groot naar klein en als er 2 het zelfde aantal fout hebben worden ze op alfabetische volgorde gesorteerd
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.Contains(searchBar.Text)).OrderBy(w => w.woord).OrderByDescending(w => w.totaalFout).ToArray();
                }
                else if (SortOptiesButton.SelectedItem.Equals("Totaal gespeelt"))
                {
                    // word gesorteerd op totaal gespeelt groot naar klein en als er 2 het zelfde aantal totaal gespeelt hebben worden ze op alfabetische volgorde gesorteerd
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.Contains(searchBar.Text)).OrderBy(w => w.woord).OrderByDescending(w => w.totaalGespeelt).ToArray();
                }
                else // als er geen optie is geselecteert dit is onmogelijk maar als er een bug is is dit handig word gewoon op alfabetisch gedaan
                {
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.Contains(searchBar.Text)).OrderBy(w => w.woord).ToArray();
                }
            }
            else
            {
                if (SortOptiesButton.SelectedItem.Equals("Alfabetisch"))
                {
                    // word gesorteerd op omgekeerde alfabetische volgorde
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.Contains(searchBar.Text)).OrderByDescending(w => w.woord).ToArray();
                }
                else if (SortOptiesButton.SelectedItem.Equals("Aantal goed"))
                {
                    // word gesorteerd op aantal goed klein naar groot en als er 2 het zelfde aantal goed hebben worden ze op alfabetische volgorde gesorteerd
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.Contains(searchBar.Text)).OrderBy(w => w.woord).OrderBy(w => w.totaalGoed).ToArray();
                }
                else if (SortOptiesButton.SelectedItem.Equals("Aantal fout"))
                {
                    // word gesorteerd op aantal fout klein naar groot en als er 2 het zelfde aantal fout hebben worden ze op alfabetische volgorde gesorteerd
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.Contains(searchBar.Text)).OrderBy(w => w.woord).OrderBy(w => w.totaalFout).ToArray();
                }
                else if (SortOptiesButton.SelectedItem.Equals("Totaal gespeelt"))
                {
                    // word gesorteerd op totaal gespeelt klein naar grooten als er 2 het zelfde aantal totaal gespeelt hebben worden ze op alfabetische volgorde gesorteerd
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.Contains(searchBar.Text)).OrderBy(w => w.woord).OrderBy(w => w.totaalGespeelt).ToArray();
                }
                else // als er geen optie is geselecteert dit is onmogelijk maar als er een bug is is dit handig word gewoon op omgekeerd alfabetisch gedaan
                {
                    gesorteerdeWoorden = woorden.Where(woord => woord.woord.Contains(searchBar.Text)).OrderByDescending(w => w.woord).ToArray();
                }

            }


            // maakt het grid weer leeg zodat er weer voor de gesorteerde worden een nieuwe grid gemaakt kan worden 
            DynamicGrid.Children.Clear();
            DynamicGrid.RowDefinitions.Clear();
            DynamicGrid.ColumnDefinitions.Clear();
            int cellCount = gesorteerdeWoorden.Length;
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
                    Name = gesorteerdeWoorden[i].woord,
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
                AbsoluteMinimum = 0
            };

            // Voeg een y-as toe voor de WPM-waarden.
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "WPM",
                Minimum = 0,
                Maximum = maxValue + (maxValue * 0.1), // Voeg 10% marge toe aan de maximumwaarde.
                AbsoluteMinimum = 0
            };

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);

            // Koppel het grafiekmodel aan de WPM-plotweergave.
            WPMPlotView.Model = plotModel;
        }

        // Methode om een grafiek te tekenen voor het gemiddelde foutpercentage.
        private void DrawWrongPercentageGraph(List<int> data)
        {
            // Maak een nieuw grafiekmodel voor foutpercentages.
            var plotModel = new PlotModel { Title = "Gemiddeld Fout Percentage" };

            // Stel stijlen in voor de grafiek.
            plotModel.SubtitleColor = OxyColors.White;
            plotModel.TextColor = OxyColors.White;
            plotModel.PlotAreaBorderColor = OxyColors.White;

            // Maak een lijnserie voor foutpercentages.
            var lineSeries = new LineSeries
            {
                Title = "Fout Percentage",
                Color = OxyColors.Red
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
                AbsoluteMinimum = 0
            };

            // Voeg een y-as toe voor foutpercentages.
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Fout Percentage (%)",
                Minimum = 0,
                Maximum = Math.Min(maxValue + (maxValue * 0.1), 100), // Zorg dat het percentage niet boven 100% komt.
                AbsoluteMinimum = 0,
                AbsoluteMaximum = 100
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
                AbsoluteMinimum = 0
            };

            // Voeg een y-as toe voor het aantal woorden.
            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Aantal Woorden",
                Minimum = 0,
                Maximum = maxValue + (maxValue * 0.1), // Voeg 10% marge toe aan de maximumwaarde.
                AbsoluteMinimum = 0
            };

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);

            // Koppel het grafiekmodel aan de plotweergave voor correcte en foute woorden.
            RightWrongPlotView.Model = plotModel;
        }
    }
}
