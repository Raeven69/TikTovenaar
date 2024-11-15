using System.Diagnostics;
using System.Timers;

namespace TikTovenaar.Logic
{
    public class Game
    {
        private Queue<Word> Words { get; set; } = new();
        public Word? CurrentWord { get; private set; }
        public bool Finished { get; private set; } = false;
        private System.Timers.Timer _timer;
        public int TimeElapsed {  get; private set; }

        public int Score { get; private set; }

        public Game()
        {
            GenerateWords();
            NextWord();
            _timer = new(1000); //tick every second
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
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
                _timer.Stop();
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

        public void CalculateScore(int incorrectKeys, int totalKeys)
        {
            if(totalKeys < incorrectKeys || incorrectKeys < 0 || totalKeys < 0) //if the input is incorrect it will not continue
            {
                throw new ArgumentOutOfRangeException("incorrect input");
            }
            double correctPercentage = ((totalKeys - incorrectKeys) / (double)totalKeys) * 100; //calculate percentage
            double wpm = (totalKeys / 5.0) / (TimeElapsed / 60.0); //calculate wpm based on TypeMonkey's wpm method
            Score = (int)(wpm * correctPercentage); //calculate score by multiplying them
        }

        public void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            TimeElapsed++;
        }
    }
}