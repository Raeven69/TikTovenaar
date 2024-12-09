using TikTovenaar.DataAccess;

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
        [DataRow(10, 1, 5, 2, 2160)]
        [DataRow(500, 4, 90, 60, 3968)]
        [DataRow(50, 50, 10, 5, 0)]

        public void Score_Test_TimeElapsedIs60(int keyPresses, int incorrectPresses, int TimeInSeconds, int wordsAmount, int ExpectedOutcome)
        {
            // Arrange
            for (int i = 0; i < TimeInSeconds; i++)
            {
                game.TimeTimerElapsed(this, null); // Simulate the timer
            }

            // Assert
            game.CalculateScore(incorrectPresses, keyPresses, wordsAmount);
            
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
            for (int i = 0; i < 120; i++)
            {
                game.TimeTimerElapsed(this, null); //simulate the timer
            }
            // Act
            game.CalculateScore(incorrectKeys, totalKeys, totalWords);
            // Assert
            Assert.AreEqual(450, game.Score);
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
            int score = game.CalculateScore(incorrectKeys, totalKeys, 10);

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
            for (int i = 0; i < 300; i++)
            {
                game.TimeTimerElapsed(this, null); //simulate the timer
            }

            // Act
            int score = game.CalculateScore(incorrectKeys, totalKeys, totalWords);
          
            // Assert
            Assert.AreEqual(10000, score);
        }

    }
}
