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
        [DataRow(-1, 1)]
        [DataRow(10, 9)]

        public void Assert_Score_ThrowsException_Wrong_Input(int incorrectPresses, int keyPresses) 
        {
            bool thrown = false;
            try
            {
                game.CalculateScore(incorrectPresses, keyPresses);
            } catch (ArgumentOutOfRangeException ex) { //if the exception is caught itll set it on true and the tests is succeeded
                thrown = true;
            }
            Assert.IsTrue(thrown);
        }
    }
}
