using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TikTovenaar.Logic;

namespace TikTovenaar
{
    public partial class MainWindow : Window
    {
        private Game Game { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Game = new();
            UpdateWord();
        }

        public void OnKeyPress(object sender, KeyEventArgs args)
        {
            string key = args.Key.ToString();
            if (key.Equals("Space"))
            {
                Game.PressKey(' ');
            }
            else if (key.Length == 1)
            {
                if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && !Console.CapsLock)
                {
                    key = key.ToLower();
                }
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
                    }
                    Text.Inlines.Add(run);
                }
            }
        }
    }
}