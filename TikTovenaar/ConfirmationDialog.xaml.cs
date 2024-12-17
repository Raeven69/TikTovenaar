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
using TikTovenaar.DataAccess;
using TikTovenaar.Logic;

namespace TikTovenaar
{
    public partial class ConfirmDialog : Window
    {
        private String _name;
        public ConfirmDialog(String name)
        {
            this._name = name;
            InitializeComponent();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DataHandler dataHandler = new DataHandler();
            try
            {
                dataHandler.DeleteUser(CurrentUser.Instance.Token, _name);
                this.Close();
            }
            catch (RequestFailedException ex)
            {
                subTitleText.Text = $"Kan {_name} niet verwijderen";
                subTitleText.Foreground = Brushes.Red;
            }
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

