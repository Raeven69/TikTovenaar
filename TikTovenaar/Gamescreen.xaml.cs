using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using TikTovenaar.Logic;
using System.Windows.Media.Animation;

namespace TikTovenaar
{
    /// <summary>
    /// Interaction logic for Gamescreen.xaml
    /// </summary>
    public partial class Gamescreen : UserControl
    {
        private Game Game { get; set; }

        private int totalLives = 3;
        private int remainingLives = 3;

        private int _totalPresses = 0;
        private int _incorrectPresses = 0;
        private bool wordWrong = false;
        private int _wordCount = 0;

        private WizardAnimation _wizardAnimation;
        public Gamescreen()
        {
            InitializeComponent();
            SetupGame();
            _wizardAnimation = new(wizardImageBrush, 0.16666, 6);
            _wizardAnimation.StartAnimation(0.16666, 6, "Images/wizard_idle.png");
        }

        private void SetupGame()
        {
            Game = new();
            Game.WordChanged += Game_wordChanged;
            UpdateWord();
            Loaded += (s, e) => Keyboard.Focus(this);
        }

        /// <summary>
        /// Below this line are all the functions for the word updating and keypresses
        /// </summary>
        public void OnKeyPress(object sender, KeyEventArgs args)
        {
            _totalPresses++;
            string key = args.Key.ToString();
            if (key.Equals("Space"))
            {
                _totalPresses--;
                // if the word is wrong, remove a life
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
                        _incorrectPresses++;
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
            _wordCount++;
            ScoreText.Text =  "Score: " + Game.CalculateScore(_incorrectPresses, _totalPresses, _wordCount); //calculate the score all the time after a keypress

            Random random = new();
            int randomNumber = random.Next(0, 2);

            // Stop any ongoing animations
            currentWordText.BeginAnimation(OpacityProperty, null);

            // Set word opacity to 0
            currentWordText.Opacity = 0;

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

        
    }
}
