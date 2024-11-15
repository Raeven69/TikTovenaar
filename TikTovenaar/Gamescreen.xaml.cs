
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using TikTovenaar.Logic;

namespace TikTovenaar
{
    /// <summary>
    /// Interaction logic for Gamescreen.xaml
    /// </summary>
    public partial class Gamescreen : UserControl
    {
        private Game Game { get; set; }

        private DispatcherTimer animationTimer;
        private int currentFrame = 0;
        private const int TOTAL_FRAMES = 8;
        private const double FRAME_WIDTH = 0.125;
        private int _totalPresses = 0;
        private int _incorrectPresses = 0;
        public Gamescreen()
        {
            InitializeComponent();
            SetupAnimation();
            Game = new();
            UpdateWord();
            Loaded += (s, e) => Keyboard.Focus(this);
        }

        public void OnKeyPress(object sender, KeyEventArgs args)
        {
            _totalPresses++;
            string key = args.Key.ToString();
            if (key.Equals("Space"))
            {
                Game.PressKey(' ');
            }
            else if (key.Length == 1)
            {
                key = !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && !Console.CapsLock
                    ? key.ToLower()
                    : key;
                Game.PressKey(key[0]);
            }
            if (!Game.Finished)
            {
                UpdateWord();
            }
            Game.CalculateScore(_incorrectPresses, _totalPresses, 60);
            System.Diagnostics.Debug.WriteLine($"score: {Game.Score}, total presses: {_totalPresses}, incorrect presses: {_incorrectPresses}");
        }

        public void UpdateWord()
        {
            if (Game.CurrentWord != null)
            {
                Text.Inlines.Clear();
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
                    Text.Inlines.Add(run);
                }
            }
        }
        private void SetupAnimation()
        {
            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(100);
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // Update the Viewbox to show the next frame
            double frameStart = currentFrame * FRAME_WIDTH;
            wizardImageBrush.Viewbox = new Rect(frameStart, 0, FRAME_WIDTH, 1);

            // Move to next frame
            currentFrame = (currentFrame + 1) % TOTAL_FRAMES;
        }

        
    }
}
