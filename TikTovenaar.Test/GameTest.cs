using TikTovenaar.DataAccess;
using TikTovenaar.Logic;

namespace TikTovenaar.Test
{
    [TestClass]
    public class GameTest
    {
        IDataHandler handler;
        [TestMethod]
        public void Constructer_CurrentWordShouldBeSet()
        {
            // Arrange
            Game game = new(new DataHandler());

            // Assert
            Assert.IsNotNull(game.CurrentWord);
        }

        [TestMethod]
        public void PressKey_AllKeysPressed_WordShouldBeSame()
        {
            // Arrange
            Game game = new(new DataHandler());
            Word word = game.CurrentWord!;

            // Act
            foreach (Letter letter in game.CurrentWord!.Letters)
            {
                game.PressKey(letter.Value ?? ' ');
            }

            // Assert
            Assert.AreEqual(word, game.CurrentWord);
        }

        [TestMethod]
        public void PressKey_SpacePressed_WordShouldBeDifferent()
        {
            // Arrange
            Game game = new(new DataHandler());
            Word word = game.CurrentWord!;

            // Act
            foreach (Letter letter in game.CurrentWord!.Letters)
            {
                game.PressKey((char)letter.Value);
            }
            game.PressKey(' ');

            // Assert
            Assert.AreNotEqual(word, game.CurrentWord);
        }
    }
}