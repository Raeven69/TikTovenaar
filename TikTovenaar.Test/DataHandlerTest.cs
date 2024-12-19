using TikTovenaar.DataAccess;
using TikTovenaar.Logic;

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
        public void GetWords_MinLength10_ShouldBeTrue()
        {
            DataHandler handler = new();
            List<string> words = handler.GetWords(10, 10);
            Assert.IsTrue(words.All(word => word.Length >= 10));
        }

        [TestMethod]
        public void GetWords_MaxLength20_ShouldBeTrue()
        {
            DataHandler handler = new();
            List<string> words = handler.GetWords(10, 0 , 20);
            Assert.IsTrue(words.All(word => word.Length <= 20));
        }

        [TestMethod]
        public void Login_CorrectCredentials_ShouldNotThrow()
        {
            DataHandler handler = new();
            handler.Login("TestPlayer", "password");
        }

        [TestMethod]
        public void Login_IncorrectCredentials_ShouldThrow()
        {
            DataHandler handler = new();
            Assert.ThrowsException<RequestFailedException>(() => {  handler.Login("test", "test"); });
        }

        public static string CreateName()
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string name = "test";
            for (int i = 0; i < 32; i++)
            {
                name += chars[Random.Shared.Next(chars.Length)];
            }
            Console.WriteLine(name);
            return name;
        }

        [TestMethod]
        public void Register_Admin_DoesNotThrow()
        {
            DataHandler handler = new();
            LoginResponse login = handler.Login("TestAdmin", "password");
            string name = CreateName();
            handler.Register(login.Token, name, "Password123!");
            handler.Login(name, "Password123!");
            handler.DeleteUser(login.Token, name);
        }

        [TestMethod]
        public void Register_NoAdmin_ShouldThrow()
        {
            DataHandler handler = new();
            LoginResponse login = handler.Login("TestPlayer", "password");
            string name = CreateName();
            Assert.ThrowsException<RequestFailedException>(() => { handler.Register(login.Token, name, "Password123!"); });
        }

        [TestMethod]
        public void Register_UsernameExists_ShouldThrow()
        {
            DataHandler handler = new();
            LoginResponse login = handler.Login("TestAdmin", "password");
            Assert.ThrowsException<RequestFailedException>(() => { handler.Register(login.Token, "TestPlayer", "Password123!"); });
        }

        [TestMethod]
        public void Register_InvalidUsername_ShouldThrow()
        {
            DataHandler handler = new();
            LoginResponse login = handler.Login("TestAdmin", "password");
            Assert.ThrowsException<RequestFailedException>(() => { handler.Register(login.Token, "a", "Password123!"); });
        }

        [TestMethod]
        public void Register_InvalidPassword_ShouldThrow()
        {
            DataHandler handler = new();
            LoginResponse login = handler.Login("TestAdmin", "password");
            string name = CreateName();
            Assert.ThrowsException<RequestFailedException>(() => { handler.Register(login.Token, name, "password"); });
        }

        [TestMethod]
        public void AddScore_ShouldNotThrow()
        {
            DataHandler handler = new();
            LoginResponse login = handler.Login("TestPlayer", "password");
            Score score = new()
            {
                WordsAmount = 10,
                Value = 200,
                Duration = TimeSpan.FromSeconds(10)
            };
            handler.AddScore(login.Token, score);
        }

        [TestMethod]
        public void GetHighscores_ShouldExist()
        {
            DataHandler handler = new();
            Leaderboards scores = handler.GetLeaderboards();
            Assert.IsTrue(scores.Scores.Count > 0 && scores.Scores[0].Player.Length > 0);
        }

        [TestMethod]
        public void Login_IsAdmin_ShouldBeTrue()
        {
            DataHandler handler = new();
            LoginResponse login = handler.Login("TestAdmin", "password");
            Assert.IsTrue(login.Admin);
        }

        [TestMethod]
        public void Login_NotAdmin_ShouldBeFalse()
        {
            DataHandler handler = new();
            LoginResponse login = handler.Login("TestPlayer", "password");
            Assert.IsFalse(login.Admin);
        }

        [TestMethod]
        public void GetDefinition_Sigma_ShouldEqualWiskunde()
        {
            DataHandler handler = new();
            Definition def = handler.GetDefinition("sigma");
            Assert.AreEqual("wiskunde", def.Category);
        }

        [TestMethod]
        public void Authorize_IncorrectToken_ShouldThrow()
        {
            DataHandler handler = new();
            Assert.ThrowsException<RequestFailedException>(() => handler.Authorize("test"));
        }

        [TestMethod]
        public void Authorize_CorrectToken_ShouldEqualUsername()
        {
            DataHandler handler = new();
            LoginResponse login = handler.Login("TestPlayer", "password");
            LoginResponse auth = handler.Authorize(login.Token);
            Assert.AreEqual("TestPlayer", auth.Name);
        }
    }
}
