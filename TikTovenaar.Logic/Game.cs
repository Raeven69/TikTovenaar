
using System.Diagnostics;
using System.Timers;
using TikTovenaar.Logic;

public class Game
{

    // GAME SETTINGS (can be changed)
    public readonly int totalLives = 3;
    private int remainingLives = 3;
    public int TimeToComplete { get; private set; } = 7;
    public bool TimeDecreasing { get; private set; } = false;
    // END OF GAME SETTINGS


    private IDataHandler _dataHandler;

    private Queue<Word> Words { get; set; } = new();
    public Word? CurrentWord { get; private set; }



    // Lists for the words and letters
    public List<Word> WordList { get; private set; } = new();
    public List<Word> WrongWords { get; private set; } = new();
    public List<string> CorrectWords
    {
        get
        {
            //List for the correct words as strings
            List<string> _rightWordsStrings = new();

            //Filter incorrect words and get the correct ones
            List<Word> _rightWords = WordList.Where(word => !WrongWords.Contains(word)).ToList();

            //Convert the Word objects to strings and add them to _rightWords
            foreach (Word word in _rightWords)
            {
                _rightWordsStrings.Add(word.getWholeWord());
            }

            // Return list of correct words as strings
            return _rightWordsStrings;
        }
    }
    public List<Letter> WrongLetters { get; private set; } = new();

    // streak variables
    public int Streak { get; private set; } = 0;
    public int MaxStreak { get; private set; } = 0;


    public bool Finished { get; private set; } = false;

    private System.Timers.Timer _timeTimer;
    private System.Timers.Timer _progressTimer;
    public int TimeElapsed { get; private set; }


    public double WPM { get; private set; }
    public double ErrorPercentage { get; private set; }
    public int WordsCount { get; private set; }
    public int Score { get; private set; }
    private int _totalPresses = 0;
    private int _incorrectPresses = 0;

    private double _progressValue;

    public event EventHandler<StreakEventArgs>? WordChanged;
    public event EventHandler? TimeUpdated;
    public event EventHandler<double>? ProgressUpdated; // Added for progress updates
    public event EventHandler? GameFinished;
    public event EventHandler? WordWrong;
    private string? token;
    public Game(IDataHandler handler)
    {
        _dataHandler = handler;

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
        List<string> words = _dataHandler.GetWords(100);

        foreach (string word in words)
        {
            Words.Enqueue(new Word(word));
        }
    }

    private void NextWord()
    {
        if (!Words.TryDequeue(out Word? word))
        {
            // if words are empty, generate new words
            GenerateWords();
            NextWord();
        }
        else
        {
            UpdateTimeToComplete();
            _progressValue = 100; // Reset progress for the new word
            CurrentWord = word;
            WordList.Add(word);

            WordChanged?.Invoke(this, new StreakEventArgs(Streak));
        }
    }

    private void UpdateTimeToComplete()
    {
        if (TimeDecreasing)
        {
            TimeToComplete = Math.Max(5, (int)(TimeToComplete * 0.9));
        }
    }

    public void PressKey(char key)
    {
        if (CurrentWord != null)
        {
            CurrentWord.EnterChar(key);
            if (key != ' ')
            {
                _totalPresses++;
                if (!CurrentWord.Letters[CurrentWord.Index - 1].IsCorrect)
                {
                    _incorrectPresses++;
                    CurrentWord.IsWrong = true;

                    WrongLetters.Add(CurrentWord.Letters[CurrentWord.Index - 1]);
                }
            }
            else
            {
                WordsCount++;
                if (CurrentWord.IsCompleted)
                {
                    if (CurrentWord.IsWrong == true)
                    {
                        remainingLives--;

                        Streak = 0;

                        WrongWords.Add(CurrentWord);

                        WordWrong?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        Streak++;
                        if(Streak > MaxStreak)
                        {
                            MaxStreak = Streak;
                        }
                    }
                }
                else
                {
                    _incorrectPresses += CurrentWord.Letters.Count - CurrentWord.Index;
                    _totalPresses += CurrentWord.Letters.Count - CurrentWord.Index;

                    remainingLives--;

                    Streak = 0;

                    WrongWords.Add(CurrentWord);

                    WordWrong?.Invoke(this, EventArgs.Empty);
                }
                CalculateScore(_incorrectPresses, _totalPresses, WordsCount, CorrectWords.Count);

                if (remainingLives <= 0)
                {
                    FinishGame();
                }
                else
                {
                    NextWord();
                }
            }
        }
    }

    public int CalculateScore(int incorrectKeys, int totalKeys, int totalWords, int correctWords)
    {
        if (totalKeys < incorrectKeys || totalWords < correctWords|| incorrectKeys < 0 || totalKeys < 0 || totalWords <= 0 || TimeElapsed <= 0)
        {
            Score = 0;
            return Score;
        }
        int correctKeys = totalKeys - incorrectKeys; //calculate how many keystrokes were correct
        double correctPercentage = ((double)correctKeys / totalKeys) * 100; //calculate the percentage of correctnumbers
        double wpm = CalculateWPM(correctKeys, totalWords); //calculate wpm

        Score = (int)(wpm * correctPercentage * correctWords); //calculate the total score
        if (Score < 0) Score = 0;
        return Score;
    }

    public double CalculateWPM(int correctKeys, int totalWords)
    {
        if (totalWords <= 0 || correctKeys <= 0 || TimeElapsed <= 0)
        {
            Debug.WriteLine($"Invalid WPM input detected {totalWords} {correctKeys} {TimeElapsed}");
            return 0;
        }
        WPM = Math.Round(totalWords / (TimeElapsed / 60.0), 2);
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
    public void StopProgressTimer()
    {
        _progressTimer.Stop();
    }
    public void StartProgressTimer()
    {
        _progressTimer.Start();
    }

    public void FinishGame()
    {
        if (Finished) return;

        Finished = true;
        CalculateErrorPercentage(_incorrectPresses, _totalPresses);
        CalculateScore(_incorrectPresses, _totalPresses, WordsCount, CorrectWords.Count);

        List<string> wrongwordsstrings = new(); // List with wrong Words
        List<char> wrongletterchars = new(); // List with wrong Letters
        foreach (Word word in WrongWords)
        {
            if (word.getWholeWord() != null)
            {
                wrongwordsstrings.Add(word.getWholeWord()); // Convert Words to strings
            }
        }
        foreach (Letter letter in WrongLetters)
        {
            if (letter.Value != null)
            {
                wrongletterchars.Add((char)letter.Value); // Convert Letters to Chars
            }
        }
        Score score = new()
        {
            WordsAmount = WordsCount,
            Value = Score,
            IncorrectWords = wrongwordsstrings,
            IncorrectLetters = wrongletterchars,
            CorrectWords = CorrectWords,
            Duration = TimeSpan.FromSeconds(TimeElapsed),
            WPM = WPM
        };
        _dataHandler.AddScore(CurrentUser.Instance.Token!, score);
        _timeTimer.Stop();
        _progressTimer.Stop();
        GameFinished?.Invoke(this, EventArgs.Empty);
    }
}
