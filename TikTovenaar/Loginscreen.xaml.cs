using System.Windows;
using System.Windows.Controls;
using TikTovenaar.DataAccess;
using TikTovenaar.Logic;

namespace TikTovenaar
{
    public partial class Loginscreen : UserControl
    {
        private DataHandler Handler { get; }

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
                string token = Handler.Login(UsernameTextBox.Text, PasswordBox.Password);
                CurrentUser.Instance.Set(UsernameTextBox.Text, token);
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.SwitchToHomeScreen();
            }
            catch (RequestFailedException exc)
            {
                ErrorText.Text = exc.Error ?? "Fields cannot be empty.";
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
