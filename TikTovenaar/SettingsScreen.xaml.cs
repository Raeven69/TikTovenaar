using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

        public ObservableCollection<string> backgroundMusicOptions { get; set; }
        public SettingsScreen()
        {
            InitializeComponent();
            DataContext = this;
            MusicVolume = Properties.Settings.Default.backgroundMusicVolume;
            SoundEffectVolume = Properties.Settings.Default.soundEffectVolume;
            backgroundMusicOptions = new ObservableCollection<string>
            {
                "Geen achtergrond muziek"
            };
            LoadBackGroundMusic();

            if (backgroundMusicOptions.Contains(Properties.Settings.Default.lastSelectedBackgroundMusic))
            {
                BackgroundMusicComboBox.SelectedItem = Properties.Settings.Default.lastSelectedBackgroundMusic;
            }
            else
            {
                BackgroundMusicComboBox.SelectedItem = "Geen achtergrond muziek";
            }
           SoundManager.CheckPlayingMusic();
        }

        private int musicVolume;

        public int MusicVolume
        {
            get => musicVolume;
            set
            {
                musicVolume = value;
                Properties.Settings.Default.backgroundMusicVolume = musicVolume;
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
                SoundManager.SetSoundEffectVulume(soundEffectVolume);
                OnPropertyChanged(nameof(SoundEffectVolume));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadBackGroundMusic()
        {
            string folderPath = "Sounds/Background";
            if (Directory.Exists(folderPath))
            {
                string[] mp3Files = Directory.GetFiles(folderPath, "*.mp3");
                foreach (string file in mp3Files)
                {
                    backgroundMusicOptions.Add(System.IO.Path.GetFileName(file));
                }
            }
        }

        private void Terug_Click(object sender, RoutedEventArgs e)
        {
            // Verkrijg een referentie naar het hoofdvenster.
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            // Wissel naar het beginscherm.
            mainWindow.SwitchToHomeScreen();
        }

        private void BackgroundMusicComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedMusic = BackgroundMusicComboBox.SelectedItem.ToString();
            Properties.Settings.Default.lastSelectedBackgroundMusic = selectedMusic;
            Properties.Settings.Default.Save();
            if (selectedMusic != "Geen achtergrond muziek")
            {
                SoundManager.PlayBackgroundSound("Sounds/Background/" + selectedMusic);
            }
            else
            {
                SoundManager.StopBackgroundSound();
            }
        }

        private void AddMusic_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", "Sounds/Background");
        }
    }
}
