using Microsoft.AspNetCore.Mvc;
using Npgsql;
using TikTovenaar.Logic;

namespace TikTovenaar.Api.Controllers
{
    [Route("score")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        [HttpGet]
        public JsonResult GetScores()
        {
            int? userID = Utils.Authorize(HttpContext.Request);
            if (userID == null)
            {
                return new(new { type = "error", message = "Authorization failed." });
            }
            NpgsqlConnection connection = new Database().GetConnection();
            connection.Open();
            using NpgsqlCommand cmd = new(@"SELECT wordsamount, score, incorrectwords, incorrectletters, correctwords, duration FROM scores WHERE userid = @userid", connection);
            cmd.Parameters.AddWithValue("userid", userID);
            using NpgsqlDataReader reader = cmd.ExecuteReader();
            List<Score> scores = [];
            while (reader.Read())
            {
                Score entry = new()
                {
                    
                    WordsAmount = reader.GetInt32(0),
                    Value = reader.GetInt32(1),
                    IncorrectWords = [..reader.GetFieldValue<string[]>(2)],
                    IncorrectLetters = [..reader.GetFieldValue<char[]>(3)],
                    CorrectWords = [.. reader.GetFieldValue<string[]>(4)],
                    Duration = reader.GetTimeSpan(5)
                };
                scores.Add(entry);
            }
            connection.Close();
            return new(scores);
        }

        [HttpPost]
        public JsonResult PostScore([FromForm] Score score)
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
            using NpgsqlCommand cmd = new(@"INSERT INTO scores(userid, wordsamount, score, incorrectwords, incorrectletters, correctwords, duration, wpm) VALUES(@userid, @wordsamount, @score, @incorrectwords, @incorrectletters, @correctwords, @duration, @wpm)", connection);
            cmd.Parameters.AddWithValue("userid", userID);
            cmd.Parameters.AddWithValue("wordsamount", score.WordsAmount);
            cmd.Parameters.AddWithValue("score", score.Value);
            cmd.Parameters.AddWithValue("incorrectwords", score.IncorrectWords);
            cmd.Parameters.AddWithValue("incorrectletters", score.IncorrectLetters);
            cmd.Parameters.AddWithValue("correctwords", score.CorrectWords);
            cmd.Parameters.AddWithValue("duration", score.Duration);
            cmd.Parameters.AddWithValue("wpm", score.WPM);
            cmd.ExecuteNonQuery();
            connection.Close();
            Utils.GainXP((int)userID, score.Value);
            return new(new { type = "success", message = "Score added." });
        }
    }
}
