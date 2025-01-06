using System.Windows;

namespace TikTovenaar
{
    /// <summary>
    /// Interaction logic for DetailsDialog.xaml
    /// </summary>
    public partial class LoginBonus : Window
    {
        public LoginBonus(int streak, int gainedXP)
        {
            InitializeComponent();
            string day = streak > 1 ? "dagen" : "dag";
            titleText.Text = $"Je bent {streak} {day} op rij ingelogd!";
            subTitleText.Text = $"Je hebt {gainedXP} XP ontvangen.";
        }
        
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
