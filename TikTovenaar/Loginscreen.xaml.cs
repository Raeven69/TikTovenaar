using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace TikTovenaar
{
    public partial class Loginscreen : UserControl
    {
        public Loginscreen()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.SwitchToHomeScreen();
        }
    }
}
