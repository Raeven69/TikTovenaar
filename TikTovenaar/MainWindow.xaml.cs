using System.Windows;

namespace TikTovenaar
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContentControl.Content = new Loginscreen();
        }

        public void SwitchToHomeScreen()
        {
            MainContentControl.Content = new Homescreen();
        }

        public void SwitchToLoginScreen()
        {
            MainContentControl.Content = new Loginscreen();
        } 
    }
}
