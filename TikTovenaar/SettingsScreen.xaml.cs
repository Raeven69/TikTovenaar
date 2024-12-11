using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace TikTovenaar
{
    /// <summary>
    /// Interaction logic for SettingsScreen.xaml
    /// </summary>
    public partial class SettingsScreen : UserControl, INotifyPropertyChanged
    {
        public SettingsScreen()
        {
            InitializeComponent();
            DataContext = this;
            MusicVolume = Properties.Settings.Default.musicVolume;
            SoundEffectVolume = 100;
        }

        private int musicVolume;

        public int MusicVolume
        {
            get => musicVolume;
            set
            {
                musicVolume = value;
                Properties.Settings.Default.musicVolume = musicVolume;
                Properties.Settings.Default.Save();
                SoundManager.SetBackgroundVulume(musicVolume);
                OnPropertyChanged(nameof(MusicVolume));
            }
        }

        private int soundEffectVolume;

        public int SoundEffectVolume
        {
            get => soundEffectVolume;
            set
            {
                soundEffectVolume = value;
                Properties.Settings.Default.soundEffectVolume = soundEffectVolume;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(SoundEffectVolume));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Terug_Click(object sender, RoutedEventArgs e)
        {
            // Verkrijg een referentie naar het hoofdvenster.
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            // Wissel naar het beginscherm.
            mainWindow.SwitchToHomeScreen();
        }

        
    }
}
