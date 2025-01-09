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

        public ObservableCollection<string> BackgroundMusicOptions { get; set; }
        public SettingsScreen()
        {
            InitializeComponent();
            DataContext = this;
            BackgroundMusicOptions = new ObservableCollection<string>();
            LoadBackGroundMusic();
            // dit zorgt voor het automatisch selecteren van de laatst geselecteerde volume
            _musicVolume = Properties.Settings.Default.backgroundMusicVolume;
            _soundEffectVolume = Properties.Settings.Default.soundEffectVolume;
            //dit zorgt voor het automatisch selecteren van de laatst geselecteerde achtergrond muziek
            if (BackgroundMusicOptions.Contains(Properties.Settings.Default.lastSelectedBackgroundMusic))
            {
                BackgroundMusicComboBox.SelectedItem = Properties.Settings.Default.lastSelectedBackgroundMusic;
            }
            else
            {
                BackgroundMusicComboBox.SelectedItem = "Geen achtergrond muziek";
            }
        }

        private int _musicVolume;

        public int MusicVolume
        {
            get => _musicVolume;
            set
            {
                if (value == 0)
                {
                    SoundManager.StopBackgroundSound();
                }
                if (_musicVolume == 0 && value > 0)
                {
                    SoundManager.PlayBackgroundSound("Sounds/Background/" + Properties.Settings.Default.lastSelectedBackgroundMusic);
                }
                _musicVolume = value;
                Properties.Settings.Default.backgroundMusicVolume = _musicVolume;
                Properties.Settings.Default.Save();
                SoundManager.SetBackgroundVulume(_musicVolume);
                OnPropertyChanged(nameof(MusicVolume));
            }
        }

        private int _soundEffectVolume;

        public int SoundEffectVolume
        {
            get => _soundEffectVolume;
            set
            {
                _soundEffectVolume = value;
                Properties.Settings.Default.soundEffectVolume = _soundEffectVolume;
                Properties.Settings.Default.Save();
                SoundManager.SetSoundEffectVulume(_soundEffectVolume);
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
            BackgroundMusicOptions.Clear();
            BackgroundMusicOptions.Add("Geen achtergrond muziek");
            string folderPath = "Sounds/Background";
            if (Directory.Exists(folderPath))
            {
                string[] mp3Files = Directory.GetFiles(folderPath, "*.mp3");
                foreach (string file in mp3Files)
                {
                    BackgroundMusicOptions.Add(Path.GetFileName(file));
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
            // zorgt er voor dat de backgrondmusiccombobox niet null is
            if (BackgroundMusicComboBox.SelectedItem != null)
            {
                string SelectedMusic = BackgroundMusicComboBox.SelectedItem.ToString();
                string LastSelectedBackgroundMusic = Properties.Settings.Default.lastSelectedBackgroundMusic;
                if (!SelectedMusic.Equals(LastSelectedBackgroundMusic))
                {
                    Properties.Settings.Default.lastSelectedBackgroundMusic = SelectedMusic;
                    Properties.Settings.Default.Save();
                    if (!SelectedMusic.Equals("Geen achtergrond muziek"))
                    {
                        SoundManager.PlayBackgroundSound("Sounds/Background/" + SelectedMusic);
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
            // Open een dialoogvenster om een bestand te selecteren.
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();

            dialog.Filters.Add(new CommonFileDialogFilter("MP3 Bestanden", "*.mp3"));
            dialog.Title = "Selecteer een MP3-bestand";

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string selectedPath = dialog.FileName;
                string targetFolder = Path.GetFullPath("Sounds/Background");
                string fileName = Path.GetFileName(selectedPath);
                string destinationPath = Path.Combine(targetFolder, fileName);
                
                if (!System.IO.File.Exists(destinationPath))
                {
                    // Kopieer het bestand naar de map met achtergrondmuziek.
                    System.IO.File.Copy(selectedPath, destinationPath);
                    LoadBackGroundMusic();
                    Properties.Settings.Default.lastSelectedBackgroundMusic = fileName;
                    SoundManager.PlayBackgroundSound("Sounds/Background/" + fileName);
                    BackgroundMusicComboBox.SelectedItem = fileName;
                } 
                else
                {
                    // Als het bestand al bestaat, toon een foutmelding.
                    MessageBox.Show("bestamd bestaat al");
                }
                
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            
            if (BackgroundMusicComboBox.SelectedItem != null)
            {
                // Verwijder het geselecteerde bestand.
                if (!BackgroundMusicComboBox.SelectedItem.ToString().Equals("Geen achtergrond muziek") && !BackgroundMusicComboBox.SelectedItem.ToString().Equals("wizard_theme_music.mp3"))
                {
                    LoadingScreen loadingScreen = new LoadingScreen("Laden");
                    loadingScreen.Show();
                    SoundManager.StopBackgroundSound();
                    System.IO.File.Delete("Sounds/Background/" + BackgroundMusicComboBox.SelectedItem.ToString());
                    LoadBackGroundMusic();
                    BackgroundMusicComboBox.SelectedItem = "Geen achtergrond muziek";
                    loadingScreen.Close();
                }
                else
                {
                    MessageBox.Show("Kan niet verwijderd worden");
                }

            }
        }

        private void SoundEffectVolume_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            SoundManager.PlaySoundEffect("Sounds/wizard_attack.mp3");
        }

    }
}
