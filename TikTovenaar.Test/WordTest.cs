using TikTovenaar.Logic;

namespace TikTovenaar.Test
{
    [TestClass]
    public class WordTest
    {
        [TestMethod]
        public void Constructor_LettersCountEquals()
        {
            // Arrange
            string value = "test";
            Word word = new(1, value);

            // Assert
            Assert.AreEqual(value.Length, word.Letters.Count);
        }

        [TestMethod]
        public void Constructor_FirstLetterEquals()
        {
            // Arrange
            string value = "test";
            Word word = new(1, value);

            // Assert
            Assert.AreEqual(value.First(), word.Letters.Last().Value);
        }

        [TestMethod]
        public void Constructor_LastLetterEquals()
        {
            // Arrange
            string value = "test";
            Word word = new(1, value);

            // Assert
            Assert.AreEqual(value.Last(), word.Letters.Last().Value);
        }

        [TestMethod]
        public void EnterChar_AllKeysPressed_NotCompleted()
        {
            // Arrange
            Word word = new(1, "test");

            // Act
            foreach (Letter letter in word.Letters)
            {
                word.EnterChar(letter.Value ?? ' ');
            }

            // Assert
            Assert.IsFalse(word.IsCompleted);
        }

        [TestMethod]
        public void EnterChar_SpacePressed_Completed()
        {
            // Arrange
            Word word = new(1, "test");
            foreach(Letter letter in word.Letters)
            {
                word.EnterChar((char)letter.Value);
            }
            // Act
            word.EnterChar(' ');

            // Assert
            Assert.IsTrue(word.IsCompleted);
        }

        [TestMethod]
        public void EnterChar_FirstReceived_ShouldEqual()
        {
            // Arrange
            string value = "test";
            Word word = new(1, value);

            // Act
            foreach (Letter letter in word.Letters)
            {
                word.EnterChar(letter.Value ?? ' ');
            }

            // Assert
            Assert.AreEqual(value.First(), word.Letters.First().Received);
        }

        [TestMethod]
        public void EnterChar_FirstReceived_ShouldNotEqual()
        {
            // Arrange
            string value = "teste";
            string rev = new(value.Reverse().ToArray());
            Word word = new(1, value);

            // Act
            for (int i = 0; i < rev.Length; i++)
            {
                word.EnterChar(rev[i]);
            }

            // Assert
            Assert.AreNotEqual(value[0], word.Letters.First().Received);
        }

        [TestMethod]
        public void EnterChar_LastReceived_ShouldEqual()
        {
            // Arrange
            string value = "test";
            Word word = new(1, value);

            // Act
            foreach (Letter letter in word.Letters)
            {
                word.EnterChar(letter.Value ?? ' ');
            }

            // Assert
            Assert.AreEqual(value.Last(), word.Letters.Last().Received);
        }

        [TestMethod]
        public void EnterChar_NothingEntered_AllLettersNotGuessed()
        {
            // Arrange
            Word word = new(1, "test");

            // Assert
            Assert.IsFalse(word.Letters.Where(l => l.HasGuessed).Any());
        }

        [TestMethod]
        public void EnterChar_AllEntered_AllLettersGuessed()
        {
            // Arrange
            Word word = new(1, "test");

            // Act
            foreach (Letter letter in word.Letters)
            {
                word.EnterChar(letter.Value ?? ' ');
            }

            // Assert
            Assert.IsTrue(word.Letters.All(l => l.HasGuessed));
        }

        [TestMethod]
        public void EnterChar_WrongChar_NotCorrect()
        {
            // Arrange
            Word word = new(1, "test");

            // Act
            word.EnterChar('e');

            // Assert
            Assert.IsFalse(word.Letters.First().IsCorrect);
        }

        [TestMethod]
        public void EnterChar_CorrectChar_Correct()
        {
            // Arrange
            Word word = new(1, "test");

            // Act
            word.EnterChar('t');

            // Assert
            Assert.IsTrue(word.Letters.First().IsCorrect);
        }
    }
}
