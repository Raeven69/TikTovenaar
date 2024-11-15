namespace TikTovenaar.Logic
{
    public class Game
    {
        private Queue<Word> Words { get; set; } = new();
        public Word? CurrentWord { get; private set; }
        public bool Finished { get; private set; } = false;

        public Game()
        {
            GenerateWords();
            NextWord();
        }

        private void GenerateWords()
        {
            string[] generated = { "random", "words", "for", "testing" };
            foreach (string word in generated)
            {
                Words.Enqueue(new(word));
            }
        }

        private void NextWord()
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
    }
}