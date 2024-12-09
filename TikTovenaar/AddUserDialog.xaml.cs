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
using System.Windows.Shapes;
using TikTovenaar.DataAccess;
using TikTovenaar.Logic;


namespace TikTovenaar
{
    /// <summary>
    /// Interaction logic for AddUserDialog.xaml
    /// </summary>
    public partial class AddUserDialog : Window
    {
        public AddUserDialog()
        {
            InitializeComponent();
            this.DataContext = this;

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (UserTextbox.Text == "" || PasswordTextbox.Text == "") //check the text boxes if they have content or not
                {
                    MessageBox.Show("Vul alle velden in om de gebruiker toe te voegen");
                    return;
                }
                else
                {
                    DataHandler dataHandler = new();
                    dataHandler.Register(CurrentUser.Instance.Token, UserTextbox.Text, PasswordTextbox.Text); 
                    MessageBox.Show($"De gebruiker met naam {UserTextbox.Text} is toegevoegd");
                    this.Close();
                    //register user, if successful show message box and close the window
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
