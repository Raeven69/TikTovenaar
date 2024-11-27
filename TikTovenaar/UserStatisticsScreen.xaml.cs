using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace TikTovenaar
{
    /// <summary>
    /// Interaction logic for UserStatisticsScreen.xaml
    /// </summary>
    public partial class UserStatisticsScreen : UserControl
    {

        private int width;
        public UserStatisticsScreen()
        {
            InitializeComponent();

            // zorgt er voor dat alles loaded is zodat ik bijvoorbeeld de breete van het scherm kan lezen
            this.Loaded += (s, e) =>
            {

                string[] woorden =
                {
                    "aan", "aandoen", "aanduiding", "aanhouden", "aankijken", "aanklacht", "aankomen", "aankondigen", "aankoop", "aanpassen", "aanslag", "aansluiten", "aansteken", "aantekening", "aanvaarden", "aanvraag", "aanwijzen", "aap", "aardappel", "aardig", "aarde", "aardig", "aardvarken", "aards", "aas", "aasje", "acht", "achter", "achteraf", "achtergrond", "achterin", "achterkant", "achterom", "achterover", "achterstand", "achteruit", "achtervolgen", "achterwerk", "actie", "actief", "activiteit", "acteur", "actrice", "actualiteit", "ad", "adapter", "adem", "ademen", "ademhaling", "ademnood", "ader", "adres", "adresseren", "advies", "advocaat", "af", "afbeelding", "afbraak", "afbraakwerk", "afbellen", "afbetaling", "afdruk", "afdruipen", "afgaan", "afgang", "afgelasten", "afgelegen", "afgeluisterd", "afgeleid", "afhalen", "afhangen", "afhankelijk", "afkorting", "afkruisen", "aflevering", "afleiden", "aflopen", "afmaken", "afmelden", "afnemen", "afreageren", "afrekenen", "afremmen", "afronden", "afscheid", "afscheuren", "afschuwelijk", "afslaan", "afsluiten", "afspelen", "afspraak", "afstand", "afsteken"
                };

                int cellCount = woorden.Length;
                int numCols = 2;
                int numRows = (cellCount + 1) / numCols;

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
                    g.Height = new GridLength(100);
                }

                foreach (var g in DynamicGrid.ColumnDefinitions)
                {
                    g.Width = new GridLength(this.ActualWidth / 2);
                }

                for (int i = 0; i < cellCount; i++)
                {
                    Button button = new Button();

                    button.Click += Button_Click;

                    button.Content = woorden[i];
                    DynamicGrid.Children.Add(button);



                    button.SetValue(Grid.RowProperty, i / numCols);
                    button.SetValue(Grid.ColumnProperty, i % numCols);
                }
            };

        }

        private void Terug_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchToHomeScreen();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            MessageBox.Show(clickedButton.Content.ToString());
        }

    }
}
 