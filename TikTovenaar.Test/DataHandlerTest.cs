using TikTovenaar.DataAccess;

namespace TikTovenaar.Test
{
    [TestClass]
    public class DataHandlerTest
    {
        [TestMethod]
        public void GetWords_Limit100_ShouldReturn100Words()
        {
            DataHandler handler = new();
            List<string> words = handler.GetWords(100);
            Assert.AreEqual(100, words.Count);
        }

        [TestMethod]
        public void Login_CorrectCredentials_ShouldReturnToken()
        {
            DataHandler handler = new();
            string? token = handler.Login("Raeven", "Password123!");
            Assert.IsNotNull(token);
        }
    }
}
