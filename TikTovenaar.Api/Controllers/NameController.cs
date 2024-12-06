using Microsoft.AspNetCore.Mvc;
using Npgsql;
using TikTovenaar.Logic;

namespace TikTovenaar.Api.Controllers
{
    [Route("names")]
    [ApiController]
    public class NameController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get()
        {
            int? userID = Utils.Authorize(HttpContext.Request);
            if (userID == null)
            {
                return Unauthorized(new { type = "error", message = "Authorization failed." });
            }

            string name;
            using (NpgsqlConnection connection = new Database().GetConnection())
            {
                connection.Open();
                using (NpgsqlCommand cmd = new(@"SELECT name FROM players WHERE id = @userid", connection))
                {
                    cmd.Parameters.AddWithValue("userid", userID);

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            name = reader.GetString(0);
                        }
                        else
                        {
                            return NotFound(new { type = "error", message = "User not found." });
                        }
                    }
                }
            }

            return Ok(name);
        }
    }
}
