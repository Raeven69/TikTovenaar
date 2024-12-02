﻿using System;
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
    /// Interaction logic for LeaderboardScreen.xaml
    /// </summary>
    public partial class LeaderboardScreen : UserControl
    {
        private Dictionary<string, int> scores = new()  {{ "Alice", 150 },
                                                        { "Bob", 120 },
                                                        { "Charlie", 180 },
                                                        { "Daisy", 130 },
                                                        { "Ethan", 110 },
                                                        { "Fiona", 200 },
                                                        { "George", 90 },
                                                        { "Hannah", 175 },
                                                        { "Ian", 95 },
                                                        { "Jasmine", 160 },
                                                        { "Kyle", 105 },
                                                        { "Lily", 140 },
                                                        { "Mason", 85 },
                                                        { "Nina", 155 },
                                                        { "Oliver", 125 },
                                                        { "Penny", 100 },
                                                        { "Quinn", 145 },
                                                        { "Rachel", 170 },
                                                        { "Sam", 135 },
                                                        { "Tina", 190 },
                                                        { "alice", 150 },
                                                        { "bob", 120 },
                                                        { "charlie", 180 },
                                                        { "daisy", 130 },
                                                        { "ethan", 110 },
                                                        { "fiona", 200 },
                                                        { "george", 90 },
                                                        { "hannah", 175 },
                                                        { "ian", 95 },
                                                        { "jasmine", 160 },
                                                        { "kyle", 105 },
                                                        { "lily", 140 },
                                                        { "mason", 85 },
                                                        { "nina", 155 },
                                                        { "oliver", 125 },
                                                        { "penny", 100 },
                                                        { "quinn", 145 },
                                                        { "rachel", 170 },
                                                        { "sam", 135 },
                                                        { "tina", 190 }};
    public LeaderboardScreen()
        {

            InitializeComponent();
            SortLeaderboard(scores);
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchToHomeScreen();
        }

        private void SortLeaderboard(Dictionary<string, int> Scores)
        {
            var rankedData = Scores
             .OrderByDescending(kvp => kvp.Value)
             .Select((kvp, index) => new
             {
                 Ranking = index + 1,
                 Name = kvp.Key,
                 Score = kvp.Value
             })
             .ToList();
             DataContext = rankedData;
        }

        private void DataGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Prevent the DataGrid from selecting a row or cell on click
            //e.Handled = true;
        }

    }
}
