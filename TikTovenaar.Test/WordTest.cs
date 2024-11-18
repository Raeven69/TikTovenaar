using TikTovenaar.Logic;

namespace TikTovenaar.Test
{
    [TestClass]
    public class WordTest
    {
        [TestMethod]
        public void Constructor_LettersCountEquals()
        {
            string value = "test";
            Word word = new(value);
            Assert.AreEqual(value.Length, word.Letters.Count);
        }

        [TestMethod]
        public void Constructor_FirstLetterEquals()
        {
            string value = "test";
            Word word = new(value);
            Assert.AreEqual(value.First(), word.Letters.Last().Value);
        }

        [TestMethod]
        public void Constructor_LastLetterEquals()
        {
            string value = "test";
            Word word = new(value);
            Assert.AreEqual(value.Last(), word.Letters.Last().Value);
        }

        [TestMethod]
        public void EnterChar_AllKeysPressed_NotCompleted()
        {
            Word word = new("test");
            foreach (Letter letter in word.Letters)
            {
                word.EnterChar(letter.Value ?? ' ');
            }
            Assert.IsFalse(word.IsCompleted);
        }

        [TestMethod]
        public void EnterChar_SpacePressed_Completed()
        {
            Word word = new("test");
            word.EnterChar(' ');
            Assert.IsTrue(word.IsCompleted);
        }

        [TestMethod]
        public void EnterChar_FirstReceived_ShouldEqual()
        {
            string value = "test";
            Word word = new(value);
            foreach (Letter letter in word.Letters)
            {
                word.EnterChar(letter.Value ?? ' ');
            }
            Assert.AreEqual(value.First(), word.Letters.First().Received);
        }

        [TestMethod]
        public void EnterChar_FirstReceived_ShouldNotEqual()
        {
            string value = "teste";
            string rev = new(value.Reverse().ToArray());
            Word word = new(value);
            for (int i = 0; i < rev.Length; i++)
            {
                word.EnterChar(rev[i]);
            }
            Assert.AreNotEqual(value[0], word.Letters.First().Received);
        }

        [TestMethod]
        public void EnterChar_LastReceived_ShouldEqual()
        {
            string value = "test";
            Word word = new(value);
            foreach (Letter letter in word.Letters)
            {
                word.EnterChar(letter.Value ?? ' ');
            }
            Assert.AreEqual(value.Last(), word.Letters.Last().Received);
        }

        [TestMethod]
        public void EnterChar_NothingEntered_AllLettersNotGuessed()
        {
            Word word = new("test");
            Assert.IsFalse(word.Letters.Where(l => l.HasGuessed).Any());
        }

        [TestMethod]
        public void EnterChar_AllEntered_AllLettersGuessed()
        {
            Word word = new("test");
            foreach (Letter letter in word.Letters)
            {
                word.EnterChar(letter.Value ?? ' ');
            }
            Assert.IsTrue(word.Letters.All(l => l.HasGuessed));
        }

        [TestMethod]
        public void EnterChar_WrongChar_NotCorrect()
        {
            Word word = new("test");
            word.EnterChar('e');
            Assert.IsFalse(word.Letters.First().IsCorrect);
        }

        [TestMethod]
        public void EnterChar_CorrectChar_Correct()
        {
            Word word = new("test");
            word.EnterChar('t');
            Assert.IsTrue(word.Letters.First().IsCorrect);
        }
    }
}
