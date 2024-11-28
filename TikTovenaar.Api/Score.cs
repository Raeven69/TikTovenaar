namespace TikTovenaar.Api
{
    public class Score(string name, int score)
    {
        public string Name { get; } = name;
        public int Value { get; } = score;
    }
}