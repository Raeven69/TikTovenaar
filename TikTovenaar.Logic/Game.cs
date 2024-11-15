namespace TikTovenaar.Logic
{
    public class Game
    {
        private Queue<Word> Words { get; set; } = new();
        public Word? CurrentWord { get; private set; }
        public bool Finished { get; private set; } = false;

        public int Score { get; private set; }

        public Game()
        {
            GenerateWords();
            NextWord();
        }

        public void GenerateWords()
        {
            string[] generated = { "random", "words", "for", "testing" };
            foreach (string word in generated)
            {
                Words.Enqueue(new(word));
            }
        }

        public void NextWord()
        {
            if (!Words.TryDequeue(out Word? word))
            {
                Finished = true;
            }
            CurrentWord = word;
        }

        public void PressKey(char key)
        {
            if (CurrentWord != null)
            {
                CurrentWord.EnterChar(key);
                if (CurrentWord.IsCompleted)
                {
                    NextWord();
                }
            }
        }

        public void CalculateScore(int incorrectKeys, int totalKeys, int timeInSeconds)
        {
            double correctPercentage = ((totalKeys - incorrectKeys) / (double)totalKeys) * 100; //calculate percentage
            double wpm = (totalKeys / 5.0) / (timeInSeconds / 60.0); //calculate wpm based on TypeMonkey's wpm method
            Score = (int)(wpm * correctPercentage); //calculate score by multiplying them
        }
    }
}