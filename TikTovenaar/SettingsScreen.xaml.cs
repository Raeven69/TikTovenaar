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
    /// Interaction logic for SettingsScreen.xaml
    /// </summary>
    public partial class SettingsScreen : UserControl
    {
        public SettingsScreen()
        {
            InitializeComponent();
        }

        private void Terug_Click(object sender, RoutedEventArgs e)
        {
            // Verkrijg een referentie naar het hoofdvenster.
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            // Wissel naar het beginscherm.
            mainWindow.SwitchToHomeScreen();
        }
    }
}
