namespace TikTovenaar.Logic
{
    public class Score(int wordsAmount, TimeOnly time, DateOnly date, int value, List<string> incorrectWords, List<char> incorrectLetters)
    {
        public int WordsAmount { get; } = wordsAmount;
        public TimeOnly Time { get; } = time;
        public DateOnly Date { get; } = date;
        public int Value { get; } = value;
        public List<string> IncorrectWords { get; } = incorrectWords;
        public List<char> IncorrectLetters { get; } = incorrectLetters;
    }
}
