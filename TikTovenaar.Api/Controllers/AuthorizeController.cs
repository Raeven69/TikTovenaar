using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace TikTovenaar.Api.Controllers
{
    [Route("authorize")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        [HttpPost]
        public JsonResult Authorize()
        {
            int? id = Utils.Authorize(HttpContext.Request);
            if (id == null)
            {
                return new(new { type = "error", message = "Authorization Failed." });
            }
            NpgsqlConnection connection = new Database().GetConnection();
            connection.Open();
            using NpgsqlCommand cmd = new(@"SELECT * FROM players WHERE id = @id", connection);
            cmd.Parameters.AddWithValue("id", id);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return new(new { type = "error", message = "Authorization failed" });
            }
            string name = reader.GetString(1);
            bool admin = reader.GetString(3) == "admin";
            return new(new { type = "success", message = new { username = name, admin } });
        }
    }
}
