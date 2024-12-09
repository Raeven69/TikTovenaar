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
    /// Interaction logic for AdminScreen.xaml
    /// </summary>
    public partial class AdminScreen : UserControl
    {
        private WizardAnimation _wizardAnimation1;
        private WizardAnimation _wizardAnimation2;

        public AdminScreen()
        {
            InitializeComponent();
            _wizardAnimation1 = new(wizardIdleImageBrush, 0.16666, 6);
            _wizardAnimation1.StartAnimation(0.16666, 6, "Images/wizard_idle.png");
            _wizardAnimation2 = new(wizardJumpImageBrush, 0.16666, 6);
            _wizardAnimation2.StartAnimation(0.16666, 6, "Images/wizard_idle.png");
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddUserDialog addUserDialog = new();
            addUserDialog.ShowDialog();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteUserDialog deleteUserDialog = new();
            deleteUserDialog.ShowDialog();
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchToHomeScreen();
        }
    }
}
