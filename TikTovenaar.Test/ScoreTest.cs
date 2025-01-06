using TikTovenaar.DataAccess;
using TikTovenaar.Logic;

namespace TikTovenaar.Test
{
    [TestClass]
    public class ScoreTest
    {
        Game game;
        IDataHandler handler;
        [TestInitialize]
        public void Initialise() //creates game because its needed in all the tests
        {
             game = new(new DataHandler());
        }
        [TestMethod]
        [DataRow(10, 1, 5, 2, 1, 2160)]
        [DataRow(500, 4, 90, 60, 2, 7936)]
        [DataRow(50, 50, 10, 5, 5, 0)]

        public void Score_Test_TimeElapsedIs60(int keyPresses, int incorrectPresses, int TimeInSeconds, int wordsAmount, int correctWords, int ExpectedOutcome)
        {
            // Arrange
            for (int i = 0; i < TimeInSeconds; i++)
            {
                game.TimeTimerElapsed(this, null); // Simulate the timer
            }

            // Assert
            game.CalculateScore(incorrectPresses, keyPresses, wordsAmount, correctWords);
            
            // Assert
            Assert.AreEqual(ExpectedOutcome, game.Score);
        }

        [TestMethod]
        public void CalculateScore_ShouldReturnCorrectScore_ForValidInputs()
        {
            // Arrange
            int incorrectKeys = 5;
            int totalKeys = 50;
            int totalWords = 10;
            int correctWords = 8;
            for (int i = 0; i < 120; i++)
            {
                game.TimeTimerElapsed(this, null); //simulate the timer
            }
            // Act
            game.CalculateScore(incorrectKeys, totalKeys, totalWords, correctWords);
            // Assert
            Assert.AreEqual(3600, game.Score);
        }

        [TestMethod]
        public void CalculateScore_ShouldReturnZero_WhenTotalKeysIsLessThanIncorrectKeys()
        {
            // Arrange
            int incorrectKeys = 10;
            int totalKeys = 5;  // Invalid case where totalKeys < incorrectKeys
            for (int i = 0; i < 120; i++)
            {
                game.TimeTimerElapsed(this, null); //simulate the timer
            }

            // Act
            int score = game.CalculateScore(incorrectKeys, totalKeys, 10, 5);

            // Assert
            Assert.AreEqual(0, score); 
        }

        [TestMethod]
        public void CalculateScore_ShouldReturnZero_WhenTotalWordsIsLessThanCorrectWords()
        {
            int correctWords = 10;
            int totalWords = 5;

            for (int i = 0; i < 120; i++)
            {
                game.TimeTimerElapsed(this, null); //simulate the timer
            }

            // Act
            int score = game.CalculateScore(5, 10, totalWords, correctWords);

            // Assert
            Assert.AreEqual(0, score);
        }

        [TestMethod]
        public void CalculateScore_ShouldHandleLargeNumbers()
        {
            // Arrange
            int incorrectKeys = 5000;
            int totalKeys = 10000;  // 50% correct
            int totalWords = 1000;
            int correctWords = 500;
            for (int i = 0; i < 300; i++)
            {
                game.TimeTimerElapsed(this, null); //simulate the timer
            }

            // Act
            int score = game.CalculateScore(incorrectKeys, totalKeys, totalWords, correctWords);
          
            // Assert
            Assert.AreEqual(5000000, score);
        }

    }
}
