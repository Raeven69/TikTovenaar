namespace TikTovenaar.Api
{
    public class ScoreEntry
    {
        public int WordsAmount { get; set; } = -1;
        public TimeOnly Time { get; set; } = TimeOnly.MinValue;
        public DateOnly Date { get; set; } = DateOnly.MinValue;
        public int Value { get; set; } = -1;
        public string IncorrectWords { get; set; } = "";
        public string IncorrectLetters { get; set; } = "";

        public bool IsValid()
        {
            return WordsAmount > -1 && Time != TimeOnly.MinValue && Date != DateOnly.MinValue && Value > -1;
        }

        public List<string> GetIncorrectWords()
        {
            return [..IncorrectWords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];
        }

        public List<char> GetIncorrectLetters()
        {
            return [..IncorrectLetters.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(letter => letter[0])];
        }
    }
}