﻿using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TikTovenaar.Logic;

namespace TikTovenaar
{
    /// <summary>
    /// Interaction logic for WordListScreen.xaml
    /// </summary>
    public partial class WordListScreen : UserControl
    {
        private GameStatisticsScreen _statisticsScreen;
        private List<Word> _wordList;
        private List<Word> _wrongWordList;
        private List<Letter> _wrongLetterList;

        public WordListScreen(GameStatisticsScreen statisticsScreen, List<Word> wordList, List<Word> wrongWordList, List<Letter> wrongLetterList)
        {
            InitializeComponent();

            _statisticsScreen = statisticsScreen;
            _wordList = wordList;
            _wrongWordList = wrongWordList;
            _wrongLetterList = wrongLetterList;

            ListBoxWords.Items.Clear();
            foreach (var word in _wordList)
            {
                AddItemToDisplay(word.getWholeWord(), true);
            }
        }

        private void Filter_Checked(object sender, RoutedEventArgs e)
        {
            if (_wordList == null || _wrongWordList == null || _wrongLetterList == null)
                return;

            RadioButton selectedFilter = sender as RadioButton;

            if (selectedFilter.Content.ToString() == "Alle woorden")
            {
                ListBoxWords.Items.Clear();
                foreach (var word in _wordList)
                {
                    AddItemToDisplay(word.getWholeWord(), true);
                }
            }
            else if (selectedFilter.Content.ToString() == "Fout getypte woorden")
            {
                ListBoxWords.Items.Clear();
                foreach (var word in _wrongWordList)
                {
                    AddItemToDisplay(word.getWholeWord(), true);
                }
            }
            else if (selectedFilter.Content.ToString() == "Fout getypte letters")
            {
                ListBoxWords.Items.Clear();
                foreach (var letter in _wrongLetterList)
                {
                    string letterText = letter.Value.ToString();
                    AddItemToDisplay(letterText, false);
                }
            }
        }

        private void AddItemToDisplay(string text, bool details = false)
        {
            if (ListBoxWords == null)
                return;

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
            };

            stackPanel.Children.Add(new TextBlock
            {
                Text = text,
                FontSize = 18,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 40, 0)
            });

            if (details)
            {
                var button = new Button
                {
                    Content = "Bekijk details",
                    Style = (Style)Resources["StyledButton"],
                    FontSize = 18,
                    Width = 150,
                    Height = 50,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0D1117")),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF235AAE"))
                };

                button.Click += (sender, e) =>
                {
                    var detailsDialog = new DetailsDialog(text);
                    detailsDialog.ShowDialog();
                };

                stackPanel.Children.Add(button);
            }

            var item = new ListBoxItem
            {
                Content = stackPanel
            };

            ListBoxWords.Items.Add(item);
        }

        private void ReturnToStatisticsScreen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchBackToStatisticsScreen(_statisticsScreen);
        }
    }
}
