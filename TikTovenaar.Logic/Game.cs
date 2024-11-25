
using System.Diagnostics;
using System.Timers;
using TikTovenaar.Logic;

public class Game
{
    private Queue<Word> Words { get; set; } = new();
    public Word? CurrentWord { get; private set; }
    public bool Finished { get; private set; } = false;
    
    public double WPM { get; private set; }

    private System.Timers.Timer _timeTimer;
    private System.Timers.Timer _progressTimer;
    public int TimeElapsed { get; private set; }
    public int TimeToComplete { get; private set; } = 15;
    public double ErrorPercentage { get; private set; }
    public int WordsCount { get; private set; }
    public int Score { get; private set; }

    private int _totalPresses = 0;
    private int _incorrectPresses = 0;
    public event EventHandler? WordChanged;
    public event EventHandler? TimeUpdated;
    public event EventHandler<double>? ProgressUpdated; // Added for progress updates
    public event EventHandler? GameFinished;

    private double _progressValue;

    public Game()
    {
        GenerateWords();
        NextWord();

        // Timer for updating game time (1 second interval)
        _timeTimer = new(1000);
        _timeTimer.Elapsed += TimeTimerElapsed;

        // Timer for updating progress (100 ms interval)
        _progressTimer = new(100);
        _progressTimer.Elapsed += ProgressTimerElapsed;

        _timeTimer.Start();
        _progressTimer.Start();
    }

    private void GenerateWords()
    {
        string[] generated = { "frikandellen", "in", "de", "middag", "met", "bier" };
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
            _timeTimer.Stop();
            _progressTimer.Stop();
            GameFinished?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            UpdateTimeToComplete();
            _progressValue = 100; // Reset progress for the new word
            CurrentWord = word;
            WordChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void UpdateTimeToComplete()
    {
        TimeToComplete = Math.Max(5, (int)(TimeToComplete * 0.9));
    }

    public void PressKey(char key, int lives)
    {
        if (lives <= 0)
        {
            FinishGame();
        }
        else
        {
            if (CurrentWord != null)
            {
                _totalPresses++;
                CurrentWord.EnterChar(key);
                if (key != ' ')
                {
                    if (!CurrentWord.Letters[CurrentWord.Index - 1].IsCorrect)
                    {
                        _incorrectPresses++;
                    }
                }
                if (CurrentWord.IsCompleted)
                {
                    WordsCount++;
                    CalculateScore(_incorrectPresses, _totalPresses, WordsCount);
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
        WPM = Math.Round(wpm, 2);
        return WPM; // Round to 2 decimal places
    }
    public double CalculateErrorPercentage(int incorrectKeys, int totalKeys)
    {
        if (totalKeys == 0) ErrorPercentage = 0; // Avoid division by zero
        else ErrorPercentage = Math.Round((incorrectKeys / (double)totalKeys) * 100, 2);
        return ErrorPercentage;
    }

    public void TimeTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        TimeElapsed++;
        TimeUpdated?.Invoke(this, EventArgs.Empty);
    }

    private void ProgressTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (_progressValue > 0)
        {
            _progressValue -= 100.0 / (TimeToComplete * 10);
            ProgressUpdated?.Invoke(this, _progressValue);
        }
        else
        {
            FinishGame();
        }
    }

    public void FinishGame()
    {
        Finished = true;
        CalculateErrorPercentage(_incorrectPresses, _totalPresses);
        CalculateScore(_incorrectPresses, _totalPresses, WordsCount);
        _timeTimer.Stop();
        _progressTimer.Stop();
        GameFinished?.Invoke(this, EventArgs.Empty);
    }
}
