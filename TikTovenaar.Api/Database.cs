using dotenv.net;
using Npgsql;

namespace TikTovenaar.Api
{
    public class Database
    {
        public string Host { get; }
        public int Port { get; }
        public string Username { get; }
        public string Password { get; }
        public string DbName { get; }

        private static IDictionary<string, string> GetEnv()
        {
            DotEnv.Load();
            IDictionary<string, string> env = DotEnv.Read();
            List<string> required = ["DB_HOST", "DB_PORT", "DB_USER", "DB_PASS", "DB_NAME"];
            if (!required.All((string item) => env.ContainsKey(item)))
            {
                throw new Exception("Failure");
            }
            return env;
        }

        public Database()
        {
            IDictionary<string, string> env = GetEnv();
            Host = env["DB_HOST"];
            Port = int.Parse(env["DB_PORT"]);
            Username = env["DB_USER"];
            Password = env["DB_PASS"];
            DbName = env["DB_NAME"];
        }

        public NpgsqlConnection GetConnection()
        {
            return new($"Host={Host};Port={Port};Username={Username};Password={Password};Database={DbName}");
        }
    }
}
