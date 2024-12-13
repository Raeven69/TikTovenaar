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
using static System.Net.Mime.MediaTypeNames;

namespace TikTovenaar
{
    /// <summary>
    /// Interaction logic for DeleteUsers.xaml
    /// </summary>
    public partial class DeleteUsers : UserControl
    {
        private List<String> _userList = new();
        public DeleteUsers()
        {
            InitializeComponent();
            ListBoxUsers.Items.Clear();
            foreach (String name in _userList)
            {
                AddItemToDisplay(name);
            }
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

            Button button = new Button
            {
                Content = "Verwijder Gebruiker",
                Style = (Style)Resources["StyledButton"],
                FontSize = 18,
                Width = 150,
                Height = 50,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF0D1117")),
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF235AAE"))
            };

            button.Click += (sender, e) =>
            {

            };

            stackPanel.Children.Add(button);

            var item = new ListBoxItem
            {
                Content = stackPanel
            };

            ListBoxUsers.Items.Add(item);
        }
    }
}
