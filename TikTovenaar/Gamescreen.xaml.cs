using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TikTovenaar.Logic;
using TikTovenaar.DataAccess;
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

        private WizardAnimation _wizardAnimation;
        private FireAnimation _fireAnimation;

        private MainWindow _MainWindow;
        public Gamescreen()
        {
            _MainWindow = (MainWindow)Application.Current.MainWindow;
            _data = new();
            InitializeComponent();
            SetupGame();
            _wizardAnimation = new(wizardImageBrush, 0.16666, 6);
            _wizardAnimation.StartAnimation(0.16666, 6, "Images/wizard_idle.png");

            _fireAnimation = new(fireImageBrush, "Images/burning_start_1.png", 0.25, 4, "Images/burning_loop_1.png", 0.125, 8, "Images/burning_end_1.png", 0.2, 5);

            // call the gamewordchanged for first word to have the animation
            Game_wordChanged(this, new StreakEventArgs(0));
        }
        private DataHandler _data;
        private void SetupGame()
        {
            Game = new(new DataHandler());
            Game.WordChanged += Game_wordChanged;
            Game.TimeUpdated += Game_timeUpdated;
            Game.ProgressUpdated += Game_progressUpdated;
            Game.GameFinished += Game_finished;
            Game.WordWrong += Word_wrong;

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
                Game.PressKey(' ');
            }
            else if (key.Length == 1)
            {
                key = !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) || !Console.CapsLock
                    ? key.ToLower()
                    : key;
                Game.PressKey(key[0]);
            }
            if (!Game.Finished)
            {
                UpdateWord();
            }
            else
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
                        run.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
                    }
                    else if (letter.HasGuessed)
                    {
                        run.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D32F2F"));
                    }
                    currentWordText.Inlines.Add(run);
                }
            }
        }

        /// <summary>
        /// Below this line are all the event handlers
        /// </summary>
        private int lastStreak = 0;
        private void Game_wordChanged(object? sender, StreakEventArgs e)
        {
            // Create the fade-in animation
            DoubleAnimation fadeIn = new(0, 1, TimeSpan.FromSeconds(0.5));

            // Create the fade-out animation
            DoubleAnimation fadeOut = new(1, 0, TimeSpan.FromSeconds(0.5));

            // Update the streak text
            if (e.Streak == 0)
            {
                streakText.Opacity = 0;
                if(_fireAnimation.fireActive)
                {
                    streakText.BeginAnimation(OpacityProperty, fadeOut);
                    _fireAnimation.EndFire(lastStreak);
                }
            }
            else
            { 
                if (!_fireAnimation.fireActive)
                {
                    _fireAnimation.StartFire(e.Streak);
                    streakText.BeginAnimation(OpacityProperty, fadeIn);
                }
                else
                {
                    // add a grow shrink animation
                    DoubleAnimation growShrink = new()
                    {
                        From = 24,
                        To = 28,
                        Duration = TimeSpan.FromSeconds(0.2),
                        AutoReverse = true
                    };
                    streakText.BeginAnimation(FontSizeProperty, growShrink);
                    _fireAnimation.ChangeFireColor(e.Streak);
                }
                streakText.Text = "Streak: " + e.Streak + "x";
                

            }
            lastStreak = e.Streak;

            WordTimer.Value = 100;

            // Stop the timer to prevent the progress bar from going down
            Game.StopProgressTimer();

            ScoreText.Text = "Score: " + Game.Score;

            Random random = new();
            int randomNumber = random.Next(0, 2);

            // Stop any ongoing animations
            currentWordText.BeginAnimation(OpacityProperty, null);

            // Set word opacity to 0
            currentWordText.Opacity = 0;

            // Play sound effect
            SoundManager.PlaySoundEffect("Sounds/wizard_attack.mp3");

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

            // Attach an event handler to the Completed event
            fadeIn.Completed += (s, args) =>
            {
                // Start the timer after the animation is over
                Game.StartProgressTimer();
            };

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
                    WordTimer.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D32F2F"));
                }
                else
                {
                    WordTimer.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50"));
                }
            });
        }

        private void Game_timeUpdated(object? sender, EventArgs e)
        {
            TimeText.Dispatcher.Invoke(() =>
            {
                int minutes = (int)Math.Floor((decimal)Game.TimeElapsed / 60);
                int seconds = Game.TimeElapsed % 60;

                TimeText.Text = "Tijd: " + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
            });
        }

        public void Game_finished(object? sender, EventArgs e)
        {
            // use a dispatcher to switch to the game statistics screen
            _MainWindow.Dispatcher.Invoke(() =>
            {
                _MainWindow.SwitchToGameStatisticsScreen($"{Game.TimeElapsed}", $"{Game.WPM}", Game.Score, $"{Game.ErrorPercentage}", $"{Game.WordsCount}", Game.WordList, Game.WrongWords, Game.WrongLetters);
            });
        }

        public void Word_wrong(object? sender, EventArgs e)
        {
            LivesBar.Value -= 100 / Game.totalLives;
        }
    }
}