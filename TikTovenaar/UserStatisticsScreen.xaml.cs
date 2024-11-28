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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TikTovenaar.Logic;

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

                int cellCount = woorden.Length;
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
                    g.Height = new GridLength(ActualHeight/8);
                }

                foreach (var g in DynamicGrid.ColumnDefinitions)
                {
                    g.Width = new GridLength(ActualWidth / numCols);
                }

                for (int i = 0; i < cellCount; i++)
                {
                    Button button = new Button
                    {
                        Name = woorden[i].woord,
                        FontSize = 20,
                        Content = new TextBlock
                        {
                            Text = woorden[i].ToString(),
                            TextAlignment = TextAlignment.Center
                        }
                    };

                    button.Click += Button_Click;

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
            
            MessageBox.Show(clickedButton.Name);
        }

    }
}
 