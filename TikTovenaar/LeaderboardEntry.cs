using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace TikTovenaar
{
    public class LeaderboardEntry
    {
        public int Ranking { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public int WordsTyped { get; set; }
        public int WPM { get; set; }
        public int Streak { get; set; }
        public int TotalScore { get; set; }
        public Brush Colorcode { get; set; }
    }
}
