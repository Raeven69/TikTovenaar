using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TikTovenaar.Logic;

namespace TikTovenaar.Test
{
    [TestClass]
    public class ScoreTest
    {
        Game game;
        [TestInitialize]
        public void Initialise() //creates game because its needed in all the tests
        {
             game = new();
        }
        [TestMethod]
        [DataRow(10, 1, 5, 2160)]
        [DataRow(500, 4, 90, 6613)]
        [DataRow(50, 50, 10, 0)]
        public void Score_Test_TimeElapsedIs60(int keyPresses, int incorrectPresses, int TimeInSeconds, int ExpectedOutcome)
        {
            for (int i = 0; i < TimeInSeconds; i++)
            {
                game.TimerElapsed(this, null); //simulate the timer
            }
            game.CalculateScore(incorrectPresses, keyPresses);
            Assert.AreEqual(ExpectedOutcome, game.Score);
        }

        [TestMethod]
        public void CalculateScore_ShouldReturnCorrectScore_ForValidInputs()
        {
            int incorrectKeys = 5;
            int totalKeys = 50;
            for (int i = 0; i < 120; i++)
            {
                game.TimerElapsed(this, null); //simulate the timer
            }

            game.CalculateScore(incorrectKeys, totalKeys);
            Assert.AreEqual(450, game.Score);
        }

        [TestMethod]
        public void CalculateScore_ShouldReturnZero_WhenTotalKeysIsLessThanIncorrectKeys()
        {
            int incorrectKeys = 10;
            int totalKeys = 5;  // Invalid case where totalKeys < incorrectKeys
            for (int i = 0; i < 120; i++)
            {
                game.TimerElapsed(this, null); //simulate the timer
            }

            int score = game.CalculateScore(incorrectKeys, totalKeys);

            Assert.AreEqual(0, score); 
        }

        [TestMethod]
        public void CalculateScore_ShouldHandleLargeNumbers()
        {
            int incorrectKeys = 5000;
            int totalKeys = 10000;  // 50% correct
            for (int i = 0; i < 300; i++)
            {
                game.TimerElapsed(this, null); //simulate the timer
            }

            int score = game.CalculateScore(incorrectKeys, totalKeys);

            Assert.AreEqual(20000, score);
        }

    }
}
