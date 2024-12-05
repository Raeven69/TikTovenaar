namespace TikTovenaar.Api
{
    public class ScoreEntry
    {
        public int WordsAmount { get; set; } = -1;
        public TimeOnly Time { get; set; } = TimeOnly.MinValue;
        public DateOnly Date { get; set; } = DateOnly.MinValue;
        public int Value { get; set; } = -1;
        public List<string> IncorrectWords { get; set; } = [];
        public List<char> IncorrectLetters { get; set; } = [];

        public bool IsValid()
        {
            return WordsAmount > -1 && Value > -1;
        }
    }
}