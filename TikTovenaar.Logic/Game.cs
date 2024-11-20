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

        public event EventHandler? WordChanged;
        public event EventHandler? TimeUpdated;
        public event EventHandler? WordTypedWrong;
        public event EventHandler? GameFinished;

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
            string[] generated = { "random", "words", "for", "testing", "tering", "homo" };
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
                GameFinished?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                CurrentWord = word;
                WordChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public void PressKey(char key, int lives)
        {
            if (lives <= 0)
            {
                Finished = true;
                _timer.Stop();
                GameFinished?.Invoke(this, EventArgs.Empty);
            }
            else
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

        public int CalculateScore(int incorrectKeys, int totalKeys, int totalWords)
        {
            if (totalKeys < incorrectKeys || incorrectKeys < 0 || totalKeys < 0 || totalWords <= 0 || TimeElapsed <= 0)
            {
                Score = 0;
                return Score;
            }
            int correctKeys = totalKeys - incorrectKeys; //calculate how many keystrokes were correct
            Debug.WriteLine($"Correct Keys: {correctKeys}");
            double correctPercentage = ((double)correctKeys / totalKeys) * 100; //calculate the percentage of correctnumbers
            double wpm = CalculateWPM(correctKeys, totalWords); //calculate wpm
            Score = (int)(wpm * correctPercentage); //calculate the total score
            return Score;
        }

        public double CalculateWPM(int correctKeys, int totalWords)
        {
            if (totalWords <= 0 || correctKeys <= 0 || TimeElapsed <= 0)
            {
                Debug.WriteLine($"Invalid WPM input detected {totalWords} {correctKeys} {TimeElapsed}");
                return 0;
            }
            double avgLength = (double)correctKeys / totalWords; //calculate average length for the wpm calculation
            double wpm = (correctKeys / avgLength) / (TimeElapsed / 60.0);
            return Math.Round(wpm, 2); // Round to 2 decimal places
        }

        public double CalculateErrorPercentage(int incorrectKeys, int totalKeys)
        {
            if (totalKeys == 0) return 0; // Avoid division by zero
            return Math.Round((incorrectKeys / (double)totalKeys) * 100, 2);
        }

        public void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            TimeElapsed++;
            TimeUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}