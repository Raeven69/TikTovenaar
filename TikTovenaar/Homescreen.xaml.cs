using System.Windows;
using System.Windows.Controls;
using TikTovenaar.DataAccess;
using TikTovenaar.Logic;

namespace TikTovenaar
{
    public partial class Homescreen : UserControl
    {
        private WizardAnimation _wizardAnimation1;
        private WizardAnimation _wizardAnimation2;

        public Homescreen()
        {
            InitializeComponent();
            adminButton.Visibility = CurrentUser.Instance.IsAdmin ? Visibility.Visible : Visibility.Collapsed;
            DataHandler handler = new();
            Level level = handler.GetLevel(CurrentUser.Instance.Token);
            LVL.Text = $"Level {level.LVL:n0}";
            Progress.Value = level.XP;
            XP.Text = $"{level.XP:n0} / 100.000";
            _wizardAnimation1 = new(wizardIdleImageBrush, 0.16666, 6);
            _wizardAnimation1.StartAnimation(0.16666, 6, "Images/wizard_idle.png");
            _wizardAnimation2 = new(wizardJumpImageBrush, 0.16666, 6);
            _wizardAnimation2.StartAnimation(0.16666, 6, "Images/wizard_idle.png");

            WelcomeMessage.Text = $"Welkom bij TikTovenaar, {CurrentUser.Instance.Name}!";
            if (WelcomeMessage.Text.Length > 30)
            {
                WelcomeMessage.FontSize = 18;
            }
            else
            {
                WelcomeMessage.FontSize = 24;
            }
        }

        /// <summary>
        /// All the code below is for clicking buttons in the Homescreen
        /// </summary>
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchToGameScreen();
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // zorgt dat je token niet meer word onhouden
            Properties.Settings.Default.inlogToken = null;
            Properties.Settings.Default.Save();
            CurrentUser.Instance.Unset();
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchToLoginScreen();
        }

        private void LeaderboardButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchToLeaderboardscreen();
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchToAdminScreen();
        }
            
        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchToUserStatisticsScreen();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.SwitchToSettingsScreen();
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
