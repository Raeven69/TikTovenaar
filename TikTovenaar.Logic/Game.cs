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

        public int CalculateScore(int incorrectKeys, int totalKeys)
        {
            if(totalKeys < incorrectKeys || incorrectKeys < 0 || totalKeys < 0 || TimeElapsed <= 0) //if the input is incorrect it will not continue
            {
                Score = 0;
                return Score;
            }
            double correctPercentage = ((totalKeys - incorrectKeys) / (double)totalKeys) * 100; //calculate percentage
            
            double wpm = (totalKeys / 5.0) / (TimeElapsed / 60.0); //calculate wpm based on TypeMonkey's wpm method
            
            Score = (int)(wpm * correctPercentage); //calculate score by multiplying them and returns it
            return Score;
        }

        public void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            TimeElapsed++;
            TimeUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}