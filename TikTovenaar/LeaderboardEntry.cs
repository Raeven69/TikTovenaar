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
        public double WPM { get; set; }
        public int Level { get; set; }
    }
}
