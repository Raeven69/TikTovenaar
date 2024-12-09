namespace TikTovenaar.Logic
{
    public class Score(int wordsAmount, int value, List<string> incorrectWords, List<char> incorrectLetters, List<string> correctWords)
    {
        public int WordsAmount { get; } = wordsAmount;
        public int Value { get; } = value;
        public List<string> IncorrectWords { get; } = incorrectWords;
        public List<char> IncorrectLetters { get; } = incorrectLetters;
        public List<string> CorrectWords { get; } = correctWords;
    }
}
