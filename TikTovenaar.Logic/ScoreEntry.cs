namespace TikTovenaar.Logic
{
    public class ScoreEntry
    {
        public int WordsAmount { get; set; } = -1;
        public int Value { get; set; } = -1;
        public List<string> IncorrectWords { get; set; } = [];
        public List<char> IncorrectLetters { get; set; } = [];
        public List<string> CorrectWords { get; set; } = [];
        public DateTime Time { get; set; } = DateTime.MinValue;

        public bool IsValid()
        {
            return WordsAmount > -1 && Value > -1;
        }
    }
}