using System.Text.RegularExpressions;

namespace TikTovenaar.Api
{
    public partial class User
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";

        public bool UsernameValid()
        {
            return NameRegex().IsMatch(Username);
        }

        public bool PasswordValid()
        {
            return PassRegex().IsMatch(Password);
        }

        [GeneratedRegex(@"^[a-zA-Z0-9]{2,50}$")]
        private static partial Regex NameRegex();

        [GeneratedRegex(@"^(?=.*[A-Z])(?=.*[\W_])[a-zA-Z0-9\W_]{8,50}$")]
        private static partial Regex PassRegex();
    }
}
