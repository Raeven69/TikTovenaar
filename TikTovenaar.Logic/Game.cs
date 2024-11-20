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

        public event EventHandler WordChanged;

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
            WordChanged?.Invoke(this, EventArgs.Empty);
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

        public int CalculateScore(int incorrectKeys, int totalKeys, int totalWords)
        {
            if(totalKeys < incorrectKeys || incorrectKeys < 0 || totalKeys < 0) //if the input is incorrect it will not continue
            {
                throw new ArgumentOutOfRangeException("incorrect input");
            }
            int correctKeys = totalKeys - incorrectKeys;
            double correctPercentage = correctKeys / totalKeys * 100; //calculate percentage
            Score = (int)(CalculateWPM(correctKeys, totalWords) * correctPercentage); //calculate score by multiplying them and returns it
            return Score;
        }

        public double CalculateWPM(int correctKeys, int totalWords)
        {
            double avgLength = (correctKeys) / (totalWords);
            return (correctKeys / avgLength) / (TimeElapsed / 60.0);
        }

        public void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            TimeElapsed++;
        }
    }
}