using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace TikTovenaar.Api.Controllers
{
    [Route("score")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        [HttpGet]
        public JsonResult GetScores()
        {
            NpgsqlConnection connection = new Database().GetConnection();
            connection.Open();
            using NpgsqlCommand cmd = new(@"SELECT players.name, scores.score FROM players JOIN scores ON players.id = scores.userid", connection);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            List<Score> scores = [];
            while (reader.Read())
            {
                scores.Add(new(reader.GetString(0), reader.GetInt32(1)));
            }
            connection.Close();
            return new(scores);
        }

        [HttpPost]
        public JsonResult PostScore([FromForm] ScoreEntry score)
        {
            int? userID = Utils.Authorize(HttpContext.Request);
            if (userID == null)
            {
                return new(new { type = "error", message = "Authorization failed." });
            }
            if (!score.IsValid())
            {
                return new(new { type = "error", message = "Malformed data." });
            }
            NpgsqlConnection connection = new Database().GetConnection();
            connection.Open();
            using NpgsqlCommand cmd = new(@"INSERT INTO scores(userid, wordsamount, time, date, score, incorrectwords, incorrectletters) VALUES(@userid, @wordsamount, @time, @date, @score, @incorrectwords, @incorrectletters)", connection);
            cmd.Parameters.AddWithValue("userid", userID);
            cmd.Parameters.AddWithValue("wordsamount", score.WordsAmount);
            cmd.Parameters.AddWithValue("time", score.Time);
            cmd.Parameters.AddWithValue("date", score.Date);
            cmd.Parameters.AddWithValue("score", score.Value);
            cmd.Parameters.AddWithValue("incorrectwords", score.GetIncorrectWords());
            cmd.Parameters.AddWithValue("incorrectletters", score.GetIncorrectLetters());
            cmd.ExecuteNonQuery();
            connection.Close();
            return new(new { type = "success", message = "Score added." });
        }
    }
}
