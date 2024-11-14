using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace TikTovenaar
{
    public partial class Homescreen : UserControl
    {
        public Homescreen()
        {
            InitializeComponent();
        }

        /// <summary>
        /// All the code below is for clicking buttons in the Homescreen
        /// </summary>
        /// 
        
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchToGameScreen();
        }
    }
}
