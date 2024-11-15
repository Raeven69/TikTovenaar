using TikTovenaar.Logic;

namespace TikTovenaar.Test
{
    [TestClass]
    public class GameTest
    {
        [TestMethod]
        public void Constructer_CurrentWordShouldBeSet()
        {
            Game game = new();
            Assert.IsNotNull(game.CurrentWord);
        }

        [TestMethod]
        public void PressKey_AllKeysPressed_WordShouldBeSame()
        {
            Game game = new();
            Word word = game.CurrentWord!;
            foreach (Letter letter in game.CurrentWord!.Letters)
            {
                game.PressKey(letter.Value ?? ' ');
            }
            Assert.AreEqual(word, game.CurrentWord);
        }

        [TestMethod]
        public void PressKey_SpacePressed_WordShouldBeDifferent()
        {
            Game game = new();
            Word word = game.CurrentWord!;
            game.PressKey(' ');
            Assert.AreNotEqual(word, game.CurrentWord);
        }
    }
}