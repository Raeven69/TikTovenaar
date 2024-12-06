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
        public void Login_CorrectCredentials_ShouldReturnToken()
        {
            DataHandler handler = new();
            string? token = handler.Login("TestPlayer", "password");
            Assert.IsNotNull(token);
        }

        [TestMethod]
        public void Login_IncorrectCredentials_ShouldBeNull()
        {
            DataHandler handler = new();
            string? token = handler.Login("test", "test");
            Assert.IsNull(token);
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
        public void Register_Admin_ShouldCreateUser()
        {
            DataHandler handler = new();
            string? token = handler.Login("TestAdmin", "password");
            string name = CreateName();
            handler.Register(token!, name, "Password123!");
            string? newToken = handler.Login(name, "Password123!");
            handler.DeleteUser(token!, name);
            Assert.IsNotNull(newToken);
        }

        [TestMethod]
        public void Register_NoAdmin_ShouldNotCreateUser()
        {
            DataHandler handler = new();
            string? token = handler.Login("TestPlayer", "password");
            string name = CreateName();
            handler.Register(token!, name, "Password123!");
            Assert.IsNull(handler.Login(name, "Password123!"));
        }

        [TestMethod]
        public void Register_UsernameExists_ShouldNotCreateUser()
        {
            DataHandler handler = new();
            string? token = handler.Login("TestAdmin", "password");
            handler.Register(token!, "TestPlayer", "Password123!");
            Assert.IsNull(handler.Login("TestPlayer", "Password123!"));
        }

        [TestMethod]
        public void Register_InvalidUsername_ShouldNotCreateUser()
        {
            DataHandler handler = new();
            string? token = handler.Login("TestAdmin", "password");
            handler.Register(token!, "a", "Password123!");
            Assert.IsNull(handler.Login("a", "Password123!"));
        }

        [TestMethod]
        public void Register_InvalidPassword_ShouldNotCreateUser()
        {
            DataHandler handler = new();
            string? token = handler.Login("TestAdmin", "password");
            string name = CreateName();
            handler.Register(token!, name, "password");
            Assert.IsNull(handler.Login(name, "password"));
        }

        [TestMethod]
        public void AddScore_ShouldAddScore()
        {
            DataHandler handler = new();
            string? token = handler.Login("TestAdmin", "password");
            List<Score> first = handler.GetScores(token!);
            Score score = new(10, TimeOnly.Parse("00:00:00"), DateOnly.Parse("01/01/2000"), 200, [], []);
            handler.AddScore(token!, score);
            List<Score> last = handler.GetScores(token!);
            Assert.IsTrue(last.Count > first.Count);
        }

        [TestMethod]
        public void GetHighscores_ShouldExist()
        {
            DataHandler handler = new();
            List<PartialScore> scores = handler.GetHighscores();
            Assert.IsTrue(scores.Count > 0 && scores[0].Player.Length > 0);
        }
    }
}
