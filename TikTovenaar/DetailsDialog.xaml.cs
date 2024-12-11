using System.Net.Http;
using System.Windows;
using Newtonsoft.Json.Linq;
using TikTovenaar.DataAccess;
using TikTovenaar.Logic;

namespace TikTovenaar
{
    /// <summary>
    /// Interaction logic for DetailsDialog.xaml
    /// </summary>
    public partial class DetailsDialog : Window
    {
        private string _word;
        public DetailsDialog(string word)
        {
            InitializeComponent();
            _word = word;
            DataHandler handler = new();
            titleText.Text = "Betekenis van het woord: '" + _word + "'";
            subTitleText.Text = "Even geduld...";
            try
            {
                Definition def = handler.GetDefinition(_word);
                subTitleText.Text = def.Meaning;
            }
            catch (RequestFailedException exc)
            {
                subTitleText.Text = exc.Message;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
