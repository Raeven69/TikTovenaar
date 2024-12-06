namespace TikTovenaar.Logic
{
    public class CurrentUser
    {
        private static readonly CurrentUser instance = new();
        private CurrentUser() {}
        public static CurrentUser Instance => instance;

        public bool IsSet { get; private set; } = false;
        public string Name { get; private set; } = "";
        public string Token { get; private set; } = "";

        public void Set(string name, string token)
        {
            if (!IsSet)
            {
                Name = name;
                Token = token;
                IsSet = true;
            }
        }
    }
}
