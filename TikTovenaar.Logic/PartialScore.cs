namespace TikTovenaar.Logic
{
    public class PartialScore<T>(string player, T score)
    {
        public string Player { get; } = player;
        public T Value { get; } = score;
    }
}