namespace TikTovenaar.Logic
{
    public class PartialScore(string player, int score)
    {
        public string Player { get; } = player;
        public int Value { get; } = score;
    }
}