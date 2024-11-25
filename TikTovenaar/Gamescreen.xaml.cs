using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TikTovenaar.Logic;
using System.Windows.Media.Animation;
using System.Windows;

namespace TikTovenaar
{
    /// <summary>
    /// Interaction logic for Gamescreen.xaml
    /// </summary>
    public partial class Gamescreen : UserControl
    {
        private Game Game { get; set; }

        private readonly int totalLives = 3;
        private int remainingLives = 3;

        private bool wordWrong = false;


        private WizardAnimation _wizardAnimation;

        private MainWindow _MainWindow;
        public Gamescreen()
        {
            _MainWindow = (MainWindow)Application.Current.MainWindow;

            InitializeComponent();
            SetupGame();
            _wizardAnimation = new(wizardImageBrush, 0.16666, 6);
            _wizardAnimation.StartAnimation(0.16666, 6, "Images/wizard_idle.png");

            // call the gamewordchanged for first word to have the animation
            Game_wordChanged(this, EventArgs.Empty);
        }

        private void SetupGame()
        {
            Game = new();
            Game.WordChanged += Game_wordChanged;
            Game.TimeUpdated += Game_timeUpdated;
            Game.ProgressUpdated += Game_progressUpdated;
            Game.GameFinished += Game_finished;

            UpdateWord();
            Loaded += (s, e) => Keyboard.Focus(this);
        }

        /// <summary>
        /// Below this line are all the functions for the word updating and keypresses
        /// </summary>
        public void OnKeyPress(object sender, KeyEventArgs args)
        {
            string key = args.Key.ToString();
            if (key.Equals("Space"))
            {
                
                if (wordWrong)
                {
                    LivesBar.Value -= 100 / totalLives;
                    remainingLives--;
                    wordWrong = false;
                }
                Game.PressKey(' ', remainingLives);
            }
            else if (key.Length == 1)
            {
                key = !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) || !Console.CapsLock
                    ? key.ToLower()
                    : key;
                Game.PressKey(key[0], remainingLives);

            }
            if (!Game.Finished)
            {
                UpdateWord();
            }else
            {
                Game_finished(this, EventArgs.Empty);
            }
        }

        public void UpdateWord()
        {
            if (Game.CurrentWord != null)
            {
                currentWordText.Inlines.Clear();

                // for each letter check if it is correct or not and change the color
                foreach (Letter letter in Game.CurrentWord.Letters)
                {
                    Run run = new(letter.Received != null ? letter.Received.ToString() : letter.Value.ToString());
                    if (letter.IsCorrect)
                    {
                        run.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else if (letter.HasGuessed)
                    {
                        run.Foreground = new SolidColorBrush(Colors.Red);
                        wordWrong = true;
                    }
                    currentWordText.Inlines.Add(run);
                }
            }
        }

        /// <summary>
        /// Below this line are all the event handlers
        /// </summary>
        private void Game_wordChanged(object sender, EventArgs e)
        {
            WordTimer.Value = 100;
            ScoreText.Text = "Score: " + Game.Score; //calculate the score all the time after a keypress

            Random random = new();
            int randomNumber = random.Next(0, 2);

            // Stop any ongoing animations
            currentWordText.BeginAnimation(OpacityProperty, null);

            // Set word opacity to 0
            currentWordText.Opacity = 0;

            // Play sound effect
            SoundManager.PlaySoundEffect("Sounds/wizard_attack.mp3");

            // Create the fade-in animation
            DoubleAnimation fadeIn = new(0, 1, TimeSpan.FromSeconds(0.5));
            if (randomNumber == 0)
            {
                _wizardAnimation.UpdateWizard("Images/wizard_attack_2.png", 0.125, 8, true, 350);
                fadeIn.BeginTime = TimeSpan.FromSeconds(0.8);
            }
            else
            {
                _wizardAnimation.UpdateWizard("Images/wizard_attack_1.png", 0.125, 8, true, 200);
                fadeIn.BeginTime = TimeSpan.FromSeconds(0.7);
            }

            // Start the animation
            currentWordText.BeginAnimation(OpacityProperty, fadeIn);
        }

        private void Game_progressUpdated(object? sender, double progress)
        {
            WordTimer.Dispatcher.Invoke(() =>
            {
                WordTimer.Value = progress;

                // Change the color of the progress bar based on the time left
                if (WordTimer.Value < 20)
                {
                    WordTimer.Foreground = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    WordTimer.Foreground = new SolidColorBrush(Colors.Green);
                }
            });
        }

        private void Game_timeUpdated(object sender, EventArgs e)
        {
            TimeText.Dispatcher.Invoke(() =>
            {
                int minutes = (int)Math.Floor((decimal)Game.TimeElapsed / 60);
                int seconds = Game.TimeElapsed % 60;

                TimeText.Text = "Tijd: " + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
            });
        }

        public void Game_finished(object sender, EventArgs e)
        {
            // use a dispatcher to switch to the game statistics screen
            _MainWindow.Dispatcher.Invoke(() =>
            {
                _MainWindow.SwitchToGameStatisticsScreen($"{Game.TimeElapsed}", $"{Game.WPM}", Game.Score, $"{Game.ErrorPercentage}", $"{Game.WordsCount}");
            });
        }
    }
}