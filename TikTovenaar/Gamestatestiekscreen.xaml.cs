using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace TikTovenaar
{
    public partial class Gamestatestiekscreen : UserControl, INotifyPropertyChanged
    {
        private string _aantalWoorden;
        private string _percentageFouten;
        private string _woordenPerMinuut;
        private string _totaleTijd;

        public event PropertyChangedEventHandler PropertyChanged;

        public string AantalWoorden
        {
            get => _aantalWoorden;
            set
            {
                _aantalWoorden = value;
                OnPropertyChanged();
            }
        }

        public string PercentageFouten
        {
            get => _percentageFouten;
            set
            {
                _percentageFouten = value;
                OnPropertyChanged();
            }
        }

        public string WoordenPerMinuut
        {
            get => _woordenPerMinuut;
            set
            {
                _woordenPerMinuut = value;
                OnPropertyChanged();
            }
        }

        public string TotaleTijd
        {
            get => _totaleTijd;
            set
            {
                _totaleTijd = value;
                OnPropertyChanged();
            }
        }

        public Gamestatestiekscreen()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Sluit_Click(object sender, RoutedEventArgs e)
        {
            // Add logic to close or hide this screen
            MessageBox.Show("Scherm gesloten!");
        }

        private void Opnieuw_Click(object sender, RoutedEventArgs e)
        {
            // Add logic to reset or restart the game
            MessageBox.Show("Opnieuw proberen!");
        }
    }
}
