using System.Windows;
using System.Windows.Controls;
using TikTovenaar.DataAccess;
using TikTovenaar.Logic;

namespace TikTovenaar
{
    public partial class Loginscreen : UserControl
    {
        private DataHandler Handler { get; }
        string token;

        public Loginscreen()
        {
            Handler = new();
            InitializeComponent();
        }

        /// <summary>
        /// All the code below is for clicking buttons in the Loginscreen
        /// </summary>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginResponse login = Handler.Login(UsernameTextBox.Text, PasswordBox.Password);
                CurrentUser.Instance.Set(UsernameTextBox.Text, login.Token, login.Admin);
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.SwitchToHomeScreen();
            }
            catch (RequestFailedException exc)
            {
                ErrorText.Text = exc.Error != null ? "Gebruikersnaam of wachtwoord onjuist." : "Vul alle velden in.";
                ErrorText.Visibility = Visibility.Visible;
            }
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
