namespace TikTovenaar.Logic
{
    public class LoginResponse(string name, string token, bool admin, int gainedXP, int streak)
    {
        public string Name { get; } = name;
        public string Token { get; } = token;
        public bool Admin { get; } = admin;
        public int GainedXP { get; } = gainedXP;
        public int Streak { get; } = streak;
    }
}
