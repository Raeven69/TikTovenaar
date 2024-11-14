using System.Windows;
using System.Windows.Controls;

namespace TikTovenaar
{
    public partial class Loginscreen : UserControl
    {
        public Loginscreen()
        {
            InitializeComponent();
        }


        /// <summary>
        /// All the code below is for clicking buttons in the Loginscreen
        /// </summary>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchToHomeScreen();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.Close();
            }
        }
    }
}
