namespace TikTovenaar.Logic
{
    public class Score
    {
        public int WordsAmount { get; set; } = -1;
        public int Value { get; set; } = -1;
        public List<string> IncorrectWords { get; set; } = [];
        public List<char> IncorrectLetters { get; set; } = [];
        public List<string> CorrectWords { get; set; } = [];
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public double WPM { get; set; } = 0.0;

        public bool IsValid()
        {
            return WordsAmount > -1 && Value > -1;
        }
    }
}