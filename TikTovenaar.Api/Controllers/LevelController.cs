using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace TikTovenaar.Api.Controllers
{
    [Route("level")]
    [ApiController]
    public class LevelController : ControllerBase
    {
        [HttpGet]
        public JsonResult GetLevel()
        {
            int? userID = Utils.Authorize(HttpContext.Request);
            if (userID == null)
            {
                return new(new { type = "error", message = "Authorization failed." });
            }
            NpgsqlConnection connection = new Database().GetConnection();
            connection.Open();
            using NpgsqlCommand cmd = new(@"SELECT level, xp FROM players WHERE id = @id", connection);
            cmd.Parameters.AddWithValue("id", userID);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                connection.Close();
                return new(new { type = "error", message = "Something went wrong." });
            }
            int level = reader.GetInt32(0);
            int xp = reader.GetInt32(1);
            connection.Close();
            return new(new { type = "success", message = new { level, xp } });
        }
    }
}