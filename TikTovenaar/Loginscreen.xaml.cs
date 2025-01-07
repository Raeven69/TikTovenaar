﻿using System.Windows;
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
            // moest alles eerst inladen anders wou hij niet switchen naar homescreen
            Loaded += (s, e) =>
            {
                LoginResponse? login = AutoLogin();
                if (login != null)
                {
                    // als automatis inloggen gelukt is swicht hij naar home schreen
                    MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                    mainWindow.SwitchToHomeScreen();
                    if (login.GainedXP > 0)
                    {
                        new LoginBonus(login.Streak, login.GainedXP).ShowDialog();
                    }
                }
            };
                
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
                Properties.Settings.Default.inlogToken = login.Token;
                Properties.Settings.Default.Save();
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.SwitchToHomeScreen();
                if (login.GainedXP > 0)
                {
                    new LoginBonus(login.Streak, login.GainedXP).ShowDialog();
                }
            }
            catch (RequestFailedException exc)
            {
                ErrorText.Text = exc.Error != null ? "Gebruikersnaam of wachtwoord onjuist." : "Vul alle velden in.";
                ErrorText.Visibility = Visibility.Visible;
            }
        }

        private LoginResponse? AutoLogin()
        {
            // krijg token die opgeslagen is
            string token = Properties.Settings.Default.inlogToken;
            if (token != null)
            {
                try
                {
                    //probeert in te loggen
                    LoginResponse login = Handler.Authorize(token);
                    CurrentUser.Instance.Set(login.Name, login.Token, login.Admin);
                    return login;
                }
                catch (RequestFailedException) {}
            }
            return null;
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
