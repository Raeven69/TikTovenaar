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
        }

        public void UpdateWord()
        {
            if (Game.CurrentWord != null)
            {
                currentWordText.Inlines.Clear();
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
            _wizardAnimation.UpdateWizard("Images/wizard_attack_1.png", 0.125, 8, true);
        }
    }
}
