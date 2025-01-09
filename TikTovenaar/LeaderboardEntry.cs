using System.Windows.Media;

namespace TikTovenaar
{
    public class LeaderboardEntry
    {
        public int Ranking { get; set; }
        public string Name { get; set; }
        public int Score { get; set; } 
        public double Value { get; set; }
        public Brush Colorcode { get; set; }


        public LeaderboardEntry()
        {
            Value = Score;
        }

    }
}
