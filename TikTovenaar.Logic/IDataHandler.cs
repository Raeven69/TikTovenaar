namespace TikTovenaar.Logic
{
    public interface IDataHandler
    {
        public string? Login(string username, string password);
        public void Register(string token, string username, string password);
        public List<PartialScore> GetHighscores();
        public List<string> GetWords(int limit = -1, int minLength = 0, int maxLength = -1);
        public List<Score> GetScores(string token);
        public void AddScore(string token, Score score);
    }
}
