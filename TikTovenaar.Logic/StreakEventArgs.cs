
namespace TikTovenaar.Logic
{
    public class StreakEventArgs : EventArgs
    {
        public int Streak { get; }

        public StreakEventArgs(int streak)
        {
            Streak = streak;
        }
    }
}
