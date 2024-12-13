using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.WindowsAPICodePack.Dialogs;


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
            backgroundMusicOptions = new ObservableCollection<string>();
            LoadBackGroundMusic();
            musicVolume = Properties.Settings.Default.backgroundMusicVolume;
            soundEffectVolume = Properties.Settings.Default.soundEffectVolume;

            if (backgroundMusicOptions.Contains(Properties.Settings.Default.lastSelectedBackgroundMusic))
            {
                BackgroundMusicComboBox.SelectedItem = Properties.Settings.Default.lastSelectedBackgroundMusic;
            }
            else
            {
                BackgroundMusicComboBox.SelectedItem = "Geen achtergrond muziek";
            }
        }

        private int musicVolume;

        public int MusicVolume
        {
            get => musicVolume;
            set
            {
                if (value == 0)
                {
                    SoundManager.StopBackgroundSound();
                }
                if (musicVolume == 0 && value > 0)
                {
                    SoundManager.PlayBackgroundSound("Sounds/Background/" + Properties.Settings.Default.lastSelectedBackgroundMusic);
                }
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
            backgroundMusicOptions.Clear();
            backgroundMusicOptions.Add("Geen achtergrond muziek");
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
            if (BackgroundMusicComboBox.SelectedItem != null)
            {
                string selectedMusic = BackgroundMusicComboBox.SelectedItem.ToString();
                string lastSelectedBackgroundMusic = Properties.Settings.Default.lastSelectedBackgroundMusic;
                if (!selectedMusic.Equals(lastSelectedBackgroundMusic))
                {
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
            }
        }

        private void AddMusic_Click(object sender, RoutedEventArgs e)
        {
            
            
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();

            dialog.Filters.Add(new CommonFileDialogFilter("MP3 Bestanden", "*.mp3"));
            dialog.Title = "Selecteer een MP3-bestand";

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string selectedPath = dialog.FileName;
                string targetFolder = System.IO.Path.GetFullPath("Sounds/Background");
                string fileName = System.IO.Path.GetFileName(selectedPath);
                string destinationPath = System.IO.Path.Combine(targetFolder, fileName);
                
                if (!System.IO.File.Exists(destinationPath))
                {
                    System.IO.File.Copy(selectedPath, destinationPath);
                    LoadBackGroundMusic();
                    Properties.Settings.Default.lastSelectedBackgroundMusic = fileName;
                    SoundManager.PlayBackgroundSound("Sounds/Background/" + fileName);
                    BackgroundMusicComboBox.SelectedItem = fileName;
                } 
                else
                {
                    MessageBox.Show("bestamd bestaat al");
                }
                
            }
            
            
        }

        private void SoundEffectVolume_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            SoundManager.PlaySoundEffect("Sounds/wizard_attack.mp3");
        }

    }
}
