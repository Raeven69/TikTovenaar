using System;
using System.Collections.Generic;
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
using TikTovenaar.Logic;

namespace TikTovenaar
{
    /// <summary>
    /// Interaction logic for GameScreen.xaml
    /// </summary>
    public partial class GameScreen : UserControl
    {
        private Game Game { get; set; }

        public GameScreen()
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
