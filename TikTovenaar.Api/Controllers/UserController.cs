using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace TikTovenaar.Api.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public JsonResult GetUsers()
        {
            int? userID = Utils.Authorize(HttpContext.Request, true);
            if (userID == null)
            {
                return new(new { type = "error", message = "Authorization failed." });
            }
            List<PartialUser> users = [];
            NpgsqlConnection connection = new Database().GetConnection();
            connection.Open();
            using NpgsqlCommand cmd = new(@"SELECT * FROM players", connection);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new(reader.GetInt32(0), reader.GetString(1)));
            }
            return new(users);
        }

        [HttpPost]
        public JsonResult Register([FromForm] User user)
        {
            int? userID = Utils.Authorize(HttpContext.Request, true);
            if (userID == null)
            {
                return new(new { type = "error", message = "Authorization failed." });
            }
            if (!user.UsernameValid())
            {
                return new(new { type = "error", message = "Regex mismatch for name." });
            }
            if (!user.PasswordValid())
            {
                return new(new { type = "error", message = "Regex mismatch for password." });
            }
            NpgsqlConnection connection = new Database().GetConnection();
            connection.Open();
            using NpgsqlCommand cmd = new(@"SELECT * FROM players WHERE name = @name", connection);
            cmd.Parameters.AddWithValue("name", user.Username);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                connection.Close();
                return new(new { type = "error", message = "This name is already taken." });
            }
            reader.Close();
            using NpgsqlCommand insert = new(@"INSERT INTO players(name, password) VALUES(@name, @password)", connection);
            insert.Parameters.AddWithValue("name", user.Username);
            insert.Parameters.AddWithValue("password", BCrypt.Net.BCrypt.HashPassword(user.Password));
            insert.ExecuteNonQuery();
            connection.Close();
            return new(new { type = "success", message = "User succesfully created." });
        }

        [HttpDelete]
        public JsonResult DeleteUser([FromForm] string username)
        {
            int? userID = Utils.Authorize(HttpContext.Request, true);
            if (userID == null)
            {
                return new(new { type = "error", message = "Authorization failed." });
            }
            NpgsqlConnection connection = new Database().GetConnection();
            connection.Open();
            using NpgsqlCommand cmd = new(@"SELECT * FROM players WHERE name = @name AND usertype = 'normal'", connection);
            cmd.Parameters.AddWithValue("name", username);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                connection.Close();
                return new(new { type = "error", message = "User does not exist." });
            }
            reader.Close();
            using NpgsqlCommand delete = new(@"DELETE FROM players WHERE name = @name", connection);
            delete.Parameters.AddWithValue("name", username);
            delete.ExecuteNonQuery();
            connection.Close();
            return new(new { type = "success", message = "User deleted." });
        }
    }
}
