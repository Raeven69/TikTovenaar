namespace TikTovenaar.Logic
{
    public interface IDataHandler
    {
        public LoginResponse Login(string username, string password);
        public void Register(string token, string username, string password);
        public List<PartialUser> GetUsers(string token);
        public Leaderboards GetLeaderboards(int limit = -1);
        public List<string> GetWords(int limit = -1, int minLength = 0, int maxLength = -1);
        public List<Score> GetScores(string token);
        public int AddScore(string token, Score score);
        public LoginResponse Authorize(string token);
        public Level GetLevel(string token);
    }
}