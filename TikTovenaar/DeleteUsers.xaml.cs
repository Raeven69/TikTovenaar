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
using TikTovenaar.DataAccess;
using TikTovenaar.Logic;
using static System.Net.Mime.MediaTypeNames;

namespace TikTovenaar
{
    /// <summary>
    /// Interaction logic for DeleteUsers.xaml
    /// </summary>
    public partial class DeleteUsers : UserControl
    {
        private List<string> _userList = new List<string>
            {
            };

        private List<PartialUser> _userListRaw = new();

        DataHandler dataHandler = new();
        public DeleteUsers()
        {
            InitializeComponent();
            UpdateUserList();
        }

        private void ReturnToAdminScreen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            mainWindow.SwitchToAdminScreen();
        }

        private void AddItemToDisplay (string userName)
        {
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
            };

            stackPanel.Children.Add(new TextBlock
            {
                Text = userName,
                FontSize = 18,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 40, 0)
            });

            if (userName != CurrentUser.Instance.Name)
            {
                Button button = new Button
                {
                    Content = "Verwijder Gebruiker",
                    Style = (Style)Resources["StyledButton"],
                    FontSize = 18,
                    Width = 220,
                    Height = 50,
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0D1117")),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF235AAE"))
                };

                button.Click += (sender, e) =>
                {
                    ConfirmDialog dialog = new ConfirmDialog(userName, () =>
                    {
                        UpdateUserList();
                    });

                    dialog.ShowDialog();

                };

                stackPanel.Children.Add(button);
            }


            var item = new ListBoxItem
            {
                Content = stackPanel
            };

            ListBoxUsers.Items.Add(item);
        }

        private void BuildUserList()
        {
            // Clear the ListBox before rebuilding
            ListBoxUsers.Items.Clear();

            foreach (String name in _userList)
            {
                AddItemToDisplay(name);
            }
        }

        private void BuildUserList(string searchText)
        {
            // Clear the ListBox before rebuilding
            ListBoxUsers.Items.Clear();

            // If no search text, show all users
            if (string.IsNullOrWhiteSpace(searchText))
            {
                BuildUserList(); // Call the default method
                return;
            }

            // Filter users based on the search text (case-insensitive)
            var filteredUsers = _userList
                .Where(name => name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Add filtered users to the display
            foreach (string name in filteredUsers)
            {
                AddItemToDisplay(name);
            }
        }

        private void UpdateUserList()
        {
            _userListRaw = dataHandler.GetUsers(CurrentUser.Instance.Token);
            _userList.Clear();
            foreach (var user in _userListRaw)
            {
                _userList.Add(user.Name);
            }
            BuildUserList();
        }
        private void SearchUserTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchUserTextBox.Text;

            // Avoid using placeholder text for filtering
            if (searchText == "Zoek een gebruiker...") return;

            BuildUserList(searchText); // Use the overloaded method to filter users
        }

        private void SearchUserTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchUserTextBox.Text == "Zoek een gebruiker...")
            {
                SearchUserTextBox.Text = string.Empty;
                SearchUserTextBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void SearchUserTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchUserTextBox.Text))
            {
                SearchUserTextBox.Text = "Zoek een gebruiker...";
                SearchUserTextBox.Foreground = new SolidColorBrush(Color.FromRgb(136, 136, 136)); // #888888
            }
        }



    }
}
