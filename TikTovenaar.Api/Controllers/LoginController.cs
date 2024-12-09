using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace TikTovenaar.Api.Controllers
{
    [Route("login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public JsonResult Login([FromForm] User user)
        {
            NpgsqlConnection connection = new Database().GetConnection();
            connection.Open();
            using NpgsqlCommand cmd = new(@"SELECT * FROM players WHERE name = @name", connection);
            cmd.Parameters.AddWithValue("name", user.Username);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                string password = reader.GetString(2);
                try
                {
                    if (!BCrypt.Net.BCrypt.Verify(user.Password, password))
                    {
                        connection.Close();
                        return new(new { type = "error", message = "Password invalid." });
                    }
                }
                catch (Exception)
                {
                    connection.Close();
                    return new(new { type = "error", message = "Password invalid." });
                }
                string token = Utils.CreateToken(reader.GetInt32(0));
                bool admin = reader.GetString(3) == "admin";
                connection.Close();
                return new(new { type = "success", message = new { authentication = $"Bearer {token}", admin } });
            }
            connection.Close();
            return new(new { type = "error", message = "User not found." });
        }
    }
}
