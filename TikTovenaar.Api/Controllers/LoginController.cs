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
                if (!BCrypt.Net.BCrypt.Verify(user.Password, password))
                {
                    return new JsonResult(new { type = "error", message = "Password invalid." });
                }
                return new JsonResult(new { type = "success", message = new { authentication = "Bearer 123" } });
            }
            return new JsonResult(new { type = "error", message = "User not found." });
        }
    }
}
