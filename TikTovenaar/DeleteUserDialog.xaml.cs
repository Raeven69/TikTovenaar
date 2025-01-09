
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TikTovenaar.DataAccess;
using TikTovenaar.Logic;

namespace TikTovenaar
{
    /// <summary>
    /// Interaction logic for DeleteUserDialog.xaml
    /// </summary>
    public partial class DeleteUserDialog : Window
    {
        public DeleteUserDialog()
        {
            InitializeComponent();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        { 
                if (UserTextbox.Text == "")
                {
                    MessageBox.Show("Vul het gebruikersveld in om de gebruiker te verwijderen");
                    return;
                }
                else
                {
                    DataHandler dataHandler = new();

                    MessageBoxResult knop = MessageBox.Show($"De gebruiker met naam {UserTextbox.Text} staat op het punt om verwijderd te worden", $"Druk {"yes"} om te bevestigen ", 
                                            MessageBoxButton.YesNo); //add message box
                    if (knop == MessageBoxResult.Yes)
                    {
                        try
                        {
                            dataHandler.DeleteUser(CurrentUser.Instance.Token, UserTextbox.Text);
                        } catch (RequestFailedException ex)
                        {
                                MessageBox.Show("Gebruiker bestaat niet");
                                return;
                        }
                        MessageBox.Show($"De gebruiker met naam {UserTextbox.Text} is verwijderd");
                        this.Close();
                    }
                }
        }

        private void RemovePlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && (textBox.Text == "Gebruikersnaam" || textBox.Text == "Wachtwoord")) //if pressed the text becomes empty
            {
                textBox.Text = string.Empty;
                textBox.Foreground = new SolidColorBrush(Colors.Black);  //change typing font to black
            }
        }


        private void AddPlaceholder(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text)) //if the text already is empty revert it back
            {
                if (textBox.Name == "UserTextbox")
                {
                    textBox.Text = "Gebruikersnaam";
                }
                else if (textBox.Name == "PasswordTextbox")
                {
                    textBox.Text = "Wachtwoord";
                }
                textBox.Foreground = new SolidColorBrush(Colors.Gray); //change the placeholder to gray
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
