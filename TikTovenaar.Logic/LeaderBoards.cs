namespace TikTovenaar.Logic
{
    public class Leaderboards(List<PartialScore<int>> scores, List<PartialScore<double>> wpm, List<PartialScore<int>> levels)
    {
        public List<PartialScore<int>> Scores { get; } = scores;
        public List<PartialScore<double>> WPM { get; } = wpm;
        public List<PartialScore<int>> Levels { get; } = levels;
    }
}
