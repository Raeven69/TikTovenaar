using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TikTovenaar.Logic;

namespace TikTovenaar
{
    
    public partial class UserStatisticsScreen : UserControl
    {

        StatisticsWord[] woorden =
        {
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

        public UserStatisticsScreen()
        {
            InitializeComponent();

            // dit zijn de opties die er zijn
            sortOpties = new ObservableCollection<string>
            {
                "Alfabetisch",
                "Aantal goed",
                "Aantal fout",
                "Totaal gespeelt"
            };

            // zorgt er voor dat alles loaded is zodat ik bijvoorbeeld de breete van het scherm kan lezen
            this.Loaded += (s, e) =>
            {
                PrintButtons();
            };

            // zorgt er voor dat databinding werkt 
            DataContext = this;

        }

        // functie voor het trug sturen naar homescherm
        private void Terug_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchToHomeScreen();
        }

        // dit word geactieveert waneer er op een van de worden word gedrukt 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // checkt of het een Button was die het geactieveert heeft
            Button clickedButton = sender as Button;
            

            // met clickedButton.Name kan je het word zelf krijgen 
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
    }
}
 